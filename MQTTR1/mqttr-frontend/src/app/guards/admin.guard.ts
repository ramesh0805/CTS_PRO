import { inject } from '@angular/core';
import { Router, CanActivateFn } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const adminGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  const isAuthenticated = authService.isAuthenticated();
  const isAdmin = authService.isAdmin();
  const currentUser = authService.getCurrentUser();

  console.log('[AdminGuard] Checking admin access for:', state.url);
  console.log('[AdminGuard] Is authenticated:', isAuthenticated);
  console.log('[AdminGuard] Is admin:', isAdmin);
  console.log('[AdminGuard] Current user:', currentUser);
  console.log('[AdminGuard] User role:', currentUser?.role);

  if (!isAuthenticated) {
    console.log('[AdminGuard] Not authenticated, redirecting to login');
    router.navigate(['/login'], { queryParams: { returnUrl: state.url } });
    return false;
  }

  if (isAdmin) {
    console.log('[AdminGuard] Admin access granted');
    return true;
  }

  console.log('[AdminGuard] Admin access denied, redirecting to dashboard');
  // Redirect to dashboard if not admin
  router.navigate(['/dashboard']);
  alert('Access denied. Admin privileges required.');
  return false;
};
