import { Component } from '@angular/core';
import { LedgerService } from '../../services/ledger.service';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { BookingService } from '../../services/booking.service';
import { Title } from '@angular/platform-browser';
import { Router } from '@angular/router';

@Component({
    selector: 'app-booking',
    imports: [CommonModule, FormsModule],
    templateUrl: './create.component.html',
    styleUrl: './create.component.css',
    providers: [LedgerService, BookingService, HttpClient],
})
export class CreateLedgerComponent {
    name: string | null = null;
    errorMessage = '';
    loading = false;

    constructor(
        private ledgerService: LedgerService,
        private routerService: Router,
        private titleService: Title,
    ) {
        this.titleService.setTitle('Create new ledger');
    }

    get isValid() {
        return !!this.name;
    }

    onSubmit(): void {
        if (!this.name) return;
        this.errorMessage = '';
        this.loading = true;
        this.ledgerService.createNew(this.name).subscribe({
            next: () => {
                this.routerService.navigate(['/ledgers']);
                this.loading = false;
            },
            error: (error) => {
                this.errorMessage = `Creating leder failed: ${error.error}`;
                this.loading = false;
            },
        });
    }
}
