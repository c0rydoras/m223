import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { AuthService } from './auth.service';

@Injectable({
    providedIn: 'root',
})
export class BookingService {
    private apiUrl = 'http://localhost:5000/api/v1';

    constructor(
        private http: HttpClient,
        private authService: AuthService,
    ) {}

    makeBooking(fromLedgerId: number, toLedgerId: number, amount: number): Observable<unknown> {
        const payload = {
            fromLedgerId,
            toLedgerId,
            amount,
        };
        const token = this.authService.getToken();
        if (token) {
            return this.http.post(`${this.apiUrl}/ledgers/transfer`, payload);
        }
        return of(null);
    }
}
