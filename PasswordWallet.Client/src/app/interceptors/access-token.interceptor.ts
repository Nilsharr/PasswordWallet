import { inject } from '@angular/core';
import { HttpInterceptorFn, HttpHandlerFn, HttpEvent, HttpRequest } from '@angular/common/http';
import { EMPTY, Observable } from 'rxjs';
import { UserService } from '../services/user.service';
import { TokenDataService } from '../services/token-data.service';
import { TokenInfo } from '../models/token-info';
import { AuthenticationConstants } from '../constants/authentication.constant';

export const accessTokenInterceptor: HttpInterceptorFn = (
  request: HttpRequest<unknown>,
  next: HttpHandlerFn
): Observable<HttpEvent<unknown>> => {
  if (request.context.get(AuthenticationConstants.IS_PUBLIC_API)) {
    return next(request);
  }

  const userService: UserService = inject(UserService);
  const tokenDataService: TokenDataService = inject(TokenDataService);
  const now: Date = new Date();
  let accessToken: TokenInfo | null = tokenDataService.getJwtToken();

  if (!accessToken) {
    userService.logout();
    return EMPTY;
  }

  if (accessToken.expiry < now) {
    const refreshToken: TokenInfo | null = tokenDataService.getRefreshToken();
    if (!refreshToken || refreshToken.expiry < now) {
      userService.logout();
      return EMPTY;
    }

    tokenDataService
      .refreshToken({
        accessToken: accessToken.token,
        refreshToken: refreshToken.token,
      })
      .subscribe((response) => (accessToken = response));
  }

  request = request.clone({
    setHeaders: { Authorization: `Bearer ${accessToken.token}` },
  });
  return next(request);
};
