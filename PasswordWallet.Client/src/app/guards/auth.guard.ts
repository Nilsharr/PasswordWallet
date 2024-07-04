import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { UserService } from '../services/user.service';

export const authGuard: CanActivateFn = () => {
  const userService: UserService = inject(UserService);
  const router: Router = inject(Router);

  if (userService.isLoggedInValue) {
    return true;
  }
  return router.parseUrl('/login');
};
