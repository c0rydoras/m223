import { HttpHandlerFn, HttpRequest } from '@angular/common/http';
import { AuthService } from '../services/auth.service';
import { inject } from '@angular/core';
import { tap } from 'rxjs';

export function authInterceptor(req: HttpRequest<unknown>, next: HttpHandlerFn) {
    // Inject the current `AuthService` and use it to get an authentication token:
    const authService = inject(AuthService);
    const authToken = authService.getToken();

    if (!authToken) {
        return next(req);
    }

    // Clone the request to add the authentication header.
    console.log(`Bearer ${authToken.toString()}`);
    const newReq = req.clone({
        setHeaders: { Authorization: `Bearer ${authToken.toString()}` },
    });

    // Clear invalid tokens
    return next(newReq).pipe(
        tap({
            error: (e) => {
                if (e.status === 401) {
                    authService.clearToken();
                }
            },
        }),
    );
}
