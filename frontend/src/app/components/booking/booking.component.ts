import { Component, OnInit } from '@angular/core';
import { LedgerService } from '../../services/ledger.service';
import { HttpClient } from '@angular/common/http';
import { Ledger } from '../../models/ledger.interface';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { BookingService } from '../../services/booking.service';

@Component({
    selector: 'app-booking',
    imports: [CommonModule, FormsModule],
    templateUrl: './booking.component.html',
    styleUrl: './booking.component.css',
    providers: [LedgerService, BookingService, HttpClient],
})
export class BookingComponent implements OnInit {
    ledgers: Ledger[] = [];

    to: string | null = null;
    from: string | null = null;
    amount: string | null = null;
    errorMessage = '';
    loading = false;

    constructor(
        private ledgerService: LedgerService,
        private bookingService: BookingService,
    ) {}

    ngOnInit(): void {
        this.loadLedgers();
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

    get fromOptions() {
        return this.ledgers.filter((l) => l.id !== Number.parseInt(this.to ?? '-1'));
    }

    get toOptions() {
        return this.ledgers.filter((l) => l.id !== Number.parseInt(this.from ?? '-1'));
    }

    get isValid() {
        return this.amount && this.from && this.to;
    }

    onSubmit(): void {
        if (!(this.amount && this.from && this.to)) return;
        this.errorMessage = '';
        this.loading = true;
        this.bookingService.makeBooking(Number.parseInt(this.from), Number.parseInt(this.to), Number.parseInt(this.amount)).subscribe({
            next: () => {
                this.loadLedgers();
                this.loading = false;
            },
            error: (error) => {
                this.errorMessage = `Transfer failed: ${error.error}`;
                this.loading = false;
            },
        });
    }
}
