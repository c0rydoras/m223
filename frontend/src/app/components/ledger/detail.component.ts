import { Component, OnInit } from '@angular/core';
import { LedgerService } from '../../services/ledger.service';
import { Ledger } from '../../models/ledger.interface';
import { CommonModule, formatCurrency } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { map } from 'rxjs';
import { BookingService } from '../../services/booking.service';
import { Booking } from '../../models/booking.interface';
import { BookingTableComponent } from '../booking-table.component';
import { AuthService } from '../../services/auth.service';

@Component({
    selector: 'app-ledger',
    templateUrl: './detail.component.html',
    imports: [CommonModule, FormsModule, RouterModule, BookingTableComponent],
    providers: [LedgerService, HttpClient],
})
export class LedgerDetailComponent implements OnInit {
    ledger: Ledger | null = null;
    bookings: Booking[] = [];
    ledgers: Ledger[] = [];

    transferTo: string | null = null;
    amount: string | null = null;
    loading = false;
    errorMessage = '';

    constructor(
        private ledgerService: LedgerService,
        private bookingService: BookingService,
        private routerService: Router,
        private authService: AuthService,
        private route: ActivatedRoute,
    ) {}

    get transferOptions() {
        return this.ledgers.filter((l) => l.id !== this.ledger?.id);
    }

    get isValid() {
        return this.ledger && this.amount && this.transferTo && parseInt(this.amount) <= this.ledger.balance && parseInt(this.amount) > 0;
    }

    delete() {
        if (!this.ledger) {
            return;
        }

        const resp = confirm(
            `Are you sure you want to delete ${this.ledger.name}?\nAll money associated with it (${formatCurrency(this.ledger.balance, 'en-US', '$')}) will be lost forever!`,
        );
        if (!resp) {
            return;
        }
        this.ledgerService.deleteLedger(this.ledger.id).subscribe(() => {
            this.routerService.navigate(['ledgers']);
        });
    }

    loadLedger(): void {
        this.route.params.pipe(map((params) => params['id'])).subscribe({
            next: (value) => {
                this.ledgerService.getLedger(value).subscribe({
                    next: (ledger) => {
                        this.ledger = ledger;
                        this.loadBookings();
                    },
                    error: () => {
                        this.routerService.navigate(['ledgers']);
                    },
                });
            },
            error: () => {
                this.routerService.navigate(['ledgers']);
            },
        });
    }

    loadBookings(): void {
        if (!this.ledger) return;
        this.bookingService.getBookingsForLedger(this.ledger.id).subscribe({
            next: (data: Booking[]) => {
                this.bookings = data;
            },
            error: (error) => {
                console.error('Error fetching bookings', error);
            },
        });
    }

    get latestBookings() {
        return this.bookings.toSorted((a, b) => Date.parse(b.timestamp) - Date.parse(a.timestamp)).slice(0, 15);
    }
    loadLedgers(): void {
        this.ledgerService.getLedgers().subscribe({
            next: (data: Ledger[]) => {
                this.ledgers = data;
            },
            error: (error) => {
                console.error('Error fetching ledgers', error);
            },
        });
    }

    ngOnInit(): void {
        this.loadLedger();
        this.loadLedgers();
    }

    transfer(): void {
        this.errorMessage = '';
        this.loading = true;
        if (!this.isValid) return;
        this.bookingService.makeBooking(this.ledger!.id, parseInt(this.transferTo!), parseInt(this.amount!)).subscribe({
            next: () => {
                this.loadLedger();
                this.loadLedgers();
                this.loading = false;
            },
            error: (error) => {
                this.errorMessage = `Transfer failed: ${error.error}`;
                this.loading = false;
            },
        });
    }

    get isAdmin() {
        return this.authService.isAdmin;
    }
}
