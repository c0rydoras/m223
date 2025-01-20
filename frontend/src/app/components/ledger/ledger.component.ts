import { Component, OnInit } from '@angular/core';
import { LedgerService } from '../../services/ledger.service';
import { Ledger } from '../../models/ledger.interface';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { Title } from '@angular/platform-browser';
import { AuthService } from '../../services/auth.service';

@Component({
    selector: 'app-ledger',
    templateUrl: './ledger.component.html',
    imports: [CommonModule, FormsModule, RouterModule],
    providers: [LedgerService, HttpClient],
})
export class LedgerComponent implements OnInit {
    ledgers: Ledger[] = [];
    fromLedgerId: number | null = null;
    toLedgerId: number | null = null;
    amount: number | null = null;
    transferMessage = '';

    constructor(
        private ledgerService: LedgerService,
        private authService: AuthService,
        private titleService: Title,
    ) {
        this.titleService.setTitle('Ledgers');
    }

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

    get isAdmin() {
        return this.authService.isAdmin;
    }
}
