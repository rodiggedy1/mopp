import { HttpInterceptorFn, HttpRequest, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, switchMap, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { Token } from '../models/auth.model';

export const httpHeaderInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const token = authService.getToken();

  let headers = req.headers;

  const skipContentType = req.headers.get('Skip-Content-Type') === 'true';
  const skipAuth = req.headers.get('Skip-Auth') === 'true'; // 👈 Added flag for external APIs
  const isFormData = req.body instanceof FormData;
  const shouldSetJson = !skipContentType && !isFormData && req.method !== 'GET';

  if (shouldSetJson) {
    headers = headers.set('Content-Type', 'application/json').set('Accept', 'application/json');
  }

  if (skipContentType) {
    headers = headers.delete('Skip-Content-Type');
  }

  if (skipAuth) {
    headers = headers.delete('Skip-Auth'); // 👈 Clean up custom header
  }

  const excludeUrls = [
    { url: 'Authenticate/login', methods: ['POST'] },
    { url: 'Authenticate/verify', methods: ['PUT'] },
    { url: 'Authenticate/verify/resend-code', methods: ['PUT'] },
    { url: 'Authenticate/refresh-token', methods: ['POST'] },
    { url: 'User', methods: ['POST'] },
    { url: 'User/forgot-password', methods: ['POST'] },
    { url: 'User/reset-password', methods: ['POST'] },
    { url: 'users/me', methods: ['GET'] },
    { url: 'scheduled_events', methods: ['GET'] },
    { url: 'calendly.com/oauth/token', methods: ['POST'] },
  ];

  const isExcluded = excludeUrls.some(e => req.url.endsWith(e.url) && e.methods.includes(req.method));

  // ✅ Only set Authorization if not excluded and not marked as Skip-Auth
  if (!isExcluded && token && !skipAuth) {
    headers = headers.set('Authorization', `Bearer ${token}`);
  }

  const cloned = req.clone({ headers });

  return next(cloned).pipe(
    catchError((originalError: HttpErrorResponse) => {
      const hasRefresh = !!authService.getRefreshToken?.();
      const isRefreshCall = req.url.endsWith('Authenticate/refresh-token');

      if (originalError.status === 401 && !isExcluded && !isRefreshCall && hasRefresh) {
        return authService.generateRefreshToken().pipe(
          switchMap((newTokens: Token) => {
            authService.saveTokens(newTokens);
            return next(
              req.clone({
                setHeaders: {
                  Authorization: `Bearer ${newTokens.accessToken}`,
                },
              })
            );
          }),
          catchError(error => {
            authService.logout();
            return throwError(() => error);
          })
        );
      }

      return throwError(() => originalError);
    })
  );
};