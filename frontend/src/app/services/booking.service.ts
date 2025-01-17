import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { AuthService } from './auth.service';
import { Booking } from '../models/booking.interface';

@Injectable({
    providedIn: 'root',
})
export class BookingService {
    private apiUrl = 'http://localhost:5000/api/v1';

    constructor(
        private http: HttpClient,
        private authService: AuthService,
    ) {}

    getBookings(): Observable<Booking[]> {
        const token = this.authService.getToken();
        if (token) {
            return this.http.get<Booking[]>(`${this.apiUrl}/bookings`);
        }

        return new Observable<Booking[]>();
    }

    getBookingsForLedger(id: number): Observable<Booking[]> {
        const token = this.authService.getToken();
        if (token) {
            return this.http.get<Booking[]>(`${this.apiUrl}/bookings/for/${id}`);
        }

        return new Observable<Booking[]>();
    }

    makeBooking(sourceId: number, destinationId: number, amount: number): Observable<unknown> {
        const payload = {
            sourceId,
            destinationId,
            amount,
        };
        return this.http.post(`${this.apiUrl}/Bookings`, payload);
    }
}
