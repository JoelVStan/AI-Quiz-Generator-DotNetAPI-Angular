import { CanActivateFn } from '@angular/router';
import { Auth } from './services/auth';
import { inject } from '@angular/core';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(Auth);
  const token = authService.getToken();
  return !!token; // only allow access if token exists
};
