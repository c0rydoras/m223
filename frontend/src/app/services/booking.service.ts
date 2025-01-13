import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
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

    makeBooking(sourceId: number, destinationId: number, amount: number): Observable<unknown> {
        const payload = {
            sourceId,
            destinationId,
            amount,
        };
        return this.http.post(`${this.apiUrl}/Bookings`, payload);
    }
}
