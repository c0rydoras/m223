import { Component, input } from '@angular/core';
import { Booking } from '../models/booking.interface';
import { CommonModule } from '@angular/common';
import { Ledger } from '../models/ledger.interface';
import { RouterModule } from '@angular/router';

@Component({ selector: 'app-booking-table', templateUrl: './booking-table.component.html', imports: [CommonModule, RouterModule] })
export class BookingTableComponent {
    bookings = input<Booking[]>([]);
    bias = input<Ledger | null>(null);
}
