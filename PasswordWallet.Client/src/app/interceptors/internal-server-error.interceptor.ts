import { HttpEvent, HttpHandlerFn, HttpInterceptorFn, HttpRequest } from '@angular/common/http';
import { inject } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Observable, tap } from 'rxjs';

export const internalServerErrorInterceptor: HttpInterceptorFn = (
  request: HttpRequest<unknown>,
  next: HttpHandlerFn
): Observable<HttpEvent<unknown>> => {
  const snackBar: MatSnackBar = inject(MatSnackBar);
  return next(request).pipe(
    tap({
      error: (err) => {
        if (err.status === 500) {
          snackBar.open('An unexpected error ocurred. Try again later.', 'X', {
            duration: 3000,
          });
        }
      },
    })
  );
};
