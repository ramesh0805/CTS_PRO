using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MQTTR1.Data;
using MQTTR1.Models;
using BCrypt.Net;

namespace MQTTR1.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")] // Only admins can access user management
public class UsersController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UsersController> _logger;

    public UsersController(ApplicationDbContext context, ILogger<UsersController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: api/users
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
    {
        try
        {
            var users = await _context.Users
                .AsNoTracking()
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();

            var userDtos = users.Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                Role = u.Role,
                IsActive = u.IsActive,
                CreatedAt = DateTime.SpecifyKind(u.CreatedAt, DateTimeKind.Utc),
                UpdatedAt = u.UpdatedAt.HasValue ? DateTime.SpecifyKind(u.UpdatedAt.Value, DateTimeKind.Utc) : null
            }).ToList();

            return Ok(userDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching users");
            return StatusCode(500, "Internal server error");
        }
    }

    // GET: api/users/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUser(int id)
    {
        try
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            var userDto = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                IsActive = user.IsActive,
                CreatedAt = DateTime.SpecifyKind(user.CreatedAt, DateTimeKind.Utc),
                UpdatedAt = user.UpdatedAt.HasValue ? DateTime.SpecifyKind(user.UpdatedAt.Value, DateTimeKind.Utc) : null
            };

            return Ok(userDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching user {UserId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    // POST: api/users
    [HttpPost]
    public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserDto createUserDto)
    {
        try
        {
            // Check if username already exists
            if (await _context.Users.AnyAsync(u => u.Username == createUserDto.Username))
            {
                return BadRequest(new { message = "Username already exists" });
            }

            // Check if email already exists
            if (await _context.Users.AnyAsync(u => u.Email == createUserDto.Email))
            {
                return BadRequest(new { message = "Email already exists" });
            }

            var user = new User
            {
                Username = createUserDto.Username,
                Email = createUserDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password),
                Role = createUserDto.Role ?? "User",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var userDto = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                IsActive = user.IsActive,
                CreatedAt = DateTime.SpecifyKind(user.CreatedAt, DateTimeKind.Utc),
                UpdatedAt = user.UpdatedAt.HasValue ? DateTime.SpecifyKind(user.UpdatedAt.Value, DateTimeKind.Utc) : null
            };

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, userDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");
            return StatusCode(500, "Internal server error");
        }
    }

    // PUT: api/users/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult<UserDto>> UpdateUser(int id, [FromBody] UpdateUserDto updateUserDto)
    {
        try
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            // Prevent admin from deactivating themselves
            var currentUserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (id == currentUserId && updateUserDto.IsActive == false)
            {
                return BadRequest(new { message = "You cannot deactivate your own account" });
            }

            // Update fields
            if (!string.IsNullOrEmpty(updateUserDto.Email) && updateUserDto.Email != user.Email)
            {
                // Check if new email already exists
                if (await _context.Users.AnyAsync(u => u.Email == updateUserDto.Email && u.Id != id))
                {
                    return BadRequest(new { message = "Email already exists" });
                }
                user.Email = updateUserDto.Email;
            }

            if (!string.IsNullOrEmpty(updateUserDto.Role))
            {
                user.Role = updateUserDto.Role;
            }

            if (updateUserDto.IsActive.HasValue)
            {
                user.IsActive = updateUserDto.IsActive.Value;
            }

            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var userDto = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                IsActive = user.IsActive,
                CreatedAt = DateTime.SpecifyKind(user.CreatedAt, DateTimeKind.Utc),
                UpdatedAt = user.UpdatedAt.HasValue ? DateTime.SpecifyKind(user.UpdatedAt.Value, DateTimeKind.Utc) : null
            };

            return Ok(userDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user {UserId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    // DELETE: api/users/{id}
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteUser(int id)
    {
        try
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            // Prevent deletion of the main admin account
            if (user.Email == "admin@example.com")
            {
                return BadRequest(new { message = "Cannot delete the main admin account" });
            }

            // Prevent admin from deleting themselves
            var currentUserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (id == currentUserId)
            {
                return BadRequest(new { message = "You cannot delete your own account" });
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user {UserId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    // PUT: api/users/{id}/reset-password
    [HttpPut("{id}/reset-password")]
    public async Task<ActionResult> ResetPassword(int id, [FromBody] ResetPasswordDto resetPasswordDto)
    {
        try
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(resetPasswordDto.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Password reset successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting password for user {UserId}", id);
            return StatusCode(500, "Internal server error");
        }
    }
}

// DTOs
public class UserDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateUserDto
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? Role { get; set; }
}

public class UpdateUserDto
{
    public string? Email { get; set; }
    public string? Role { get; set; }
    public bool? IsActive { get; set; }
}

public class ResetPasswordDto
{
    public string NewPassword { get; set; } = string.Empty;
}
