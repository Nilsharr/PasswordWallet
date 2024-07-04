import { inject } from '@angular/core';
import {
  HttpInterceptorFn,
  HttpHandlerFn,
  HttpEvent,
  HttpRequest,
} from '@angular/common/http';
import { EMPTY, Observable, catchError, throwError } from 'rxjs';
import { UserService } from '../services/user.service';
import { AuthenticationConstants } from '../constants/authentication.constant';

export const unauthorizedErrorInterceptor: HttpInterceptorFn = (
  request: HttpRequest<unknown>,
  next: HttpHandlerFn
): Observable<HttpEvent<unknown>> => {
  if (
    request.context.get(AuthenticationConstants.IS_PUBLIC_API) ||
    request.context.get(
      AuthenticationConstants.OMIT_DEFAULT_UNAUTHORIZED_ERROR_HANDLING
    )
  ) {
    return next(request);
  }

  const userService: UserService = inject(UserService);
  return next(request).pipe(
    catchError((error) => {
      if (error.status === 401 || error.status === 403) {
        userService.logout();
        return EMPTY;
      }
      return throwError(() => error);
    })
  );
};
