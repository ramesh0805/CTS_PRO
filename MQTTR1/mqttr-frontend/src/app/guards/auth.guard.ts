import { inject } from '@angular/core';
import { Router, CanActivateFn } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  const isAuthenticated = authService.isAuthenticated();

  console.log('[AuthGuard] Checking authentication for:', state.url);
  console.log('[AuthGuard] Is authenticated:', isAuthenticated);
  console.log('[AuthGuard] Current user:', authService.getCurrentUser());
  console.log('[AuthGuard] Token exists:', !!authService.getToken());

  if (isAuthenticated) {
    console.log('[AuthGuard] Access granted');
    return true;
  }

  console.log('[AuthGuard] Access denied, redirecting to login');
  // Redirect to login with return URL
  router.navigate(['/login'], { queryParams: { returnUrl: state.url } });
  return false;
};
