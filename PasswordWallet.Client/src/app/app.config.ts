import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { accessTokenInterceptor } from './interceptors/access-token.interceptor';
import { unauthorizedErrorInterceptor } from './interceptors/unauthorized-error.interceptor';
import { internalServerErrorInterceptor } from './interceptors/internal-server-error.interceptor';
import { MAT_FORM_FIELD_DEFAULT_OPTIONS } from '@angular/material/form-field';
import { MatPaginatorIntl } from '@angular/material/paginator';
import { PaginatorIntl } from './services/paginator-intl.service';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideAnimationsAsync(),
    provideHttpClient(
      withInterceptors([
        accessTokenInterceptor,
        unauthorizedErrorInterceptor,
        internalServerErrorInterceptor,
      ])
    ),
    {
      provide: MAT_FORM_FIELD_DEFAULT_OPTIONS,
      useValue: { appearance: 'outline', subscriptSizing: 'dynamic' },
    },
    { provide: MatPaginatorIntl, useClass: PaginatorIntl },
  ],
};
