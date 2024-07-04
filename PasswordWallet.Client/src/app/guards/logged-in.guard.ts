import { CanActivateFn, Router } from '@angular/router';
import { UserService } from '../services/user.service';
import { inject } from '@angular/core';

export const loggedInGuard: CanActivateFn = () => {
  const userService: UserService = inject(UserService);
  const router: Router = inject(Router);

  if (!userService.isLoggedInValue) {
    return true;
  }
  return router.parseUrl('');
};
