import { inject } from '@angular/core';
import { Router, CanActivateFn, UrlTree } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { Observable, of } from 'rxjs';
import { map, catchError } from 'rxjs/operators';

export const rootGuard: CanActivateFn = (route, state): Observable<boolean | UrlTree> => {
  const authService = inject(AuthService);
  const router = inject(Router);

  console.log('[RootGuard] Checking authentication for root path');

  if (!authService.isAuthenticated()) {
    console.log('[RootGuard] User not authenticated, redirecting to login');
    return of(router.createUrlTree(['/login']));
  }

  return authService.validateTokenWithServer().pipe(
    map(isValid => {
      if (isValid) {
        console.log('[RootGuard] User authenticated, redirecting to dashboard');
        return router.createUrlTree(['/dashboard']);
      }

      console.log('[RootGuard] Server validation failed, redirecting to login');
      return router.createUrlTree(['/login']);
    }),
    catchError(() => {
      console.log('[RootGuard] Validation error, redirecting to login');
      return of(router.createUrlTree(['/login']));
    })
  );
};
