import { TestBed } from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { HttpClient, provideHttpClient, withInterceptors } from '@angular/common/http';
import { AuthService } from '../services/auth.service';
import { authInterceptor } from './auth.interceptor';

describe('AuthInterceptor', () => {
    let httpMock: HttpTestingController;
    let httpClient: HttpClient;
    let authServiceSpy: jasmine.SpyObj<AuthService>;

    beforeEach(() => {
        authServiceSpy = jasmine.createSpyObj('AuthService', ['getToken', 'clearToken']);

        TestBed.configureTestingModule({
            providers: [
                { provide: AuthService, useValue: authServiceSpy },
                provideHttpClient(withInterceptors([authInterceptor])),
                provideHttpClientTesting(),
            ],
        });

        httpMock = TestBed.inject(HttpTestingController);
        httpClient = TestBed.inject(HttpClient);
    });

    afterEach(() => {
        httpMock.verify();
    });

    it('should add Authorization header if token exists', () => {
        const mockToken = 'mock-token';
        authServiceSpy.getToken.and.returnValue(mockToken);

        httpClient.get('/test').subscribe();

        const req = httpMock.expectOne('/test');
        expect(req.request.headers.get('Authorization')).toBe(`Bearer ${mockToken}`);
        req.flush({});
    });

    it('should not add Authorization header if token is missing', () => {
        authServiceSpy.getToken.and.returnValue(null);

        httpClient.get('/test').subscribe();

        const req = httpMock.expectOne('/test');
        expect(req.request.headers.has('Authorization')).toBeFalse();
        req.flush({});
    });

    it('should handle 401 error', () => {
        authServiceSpy.getToken.and.returnValue('mock-token');

        httpClient.get('/test').subscribe({
            error: () => {
                // ensure errors are handled
            },
        });

        const req = httpMock.expectOne('/test');
        req.flush({}, { status: 401, statusText: 'Unauthorized' });

        expect(authServiceSpy.clearToken).toHaveBeenCalled();
    });
});
