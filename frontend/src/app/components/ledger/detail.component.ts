import { Component, OnInit } from '@angular/core';
import { LedgerService } from '../../services/ledger.service';
import { Ledger } from '../../models/ledger.interface';
import { CommonModule, formatCurrency } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { map } from 'rxjs';

@Component({
    selector: 'app-ledger',
    templateUrl: './detail.component.html',
    imports: [CommonModule, FormsModule, RouterModule],
    providers: [LedgerService, HttpClient],
})
export class LedgerDetailComponent implements OnInit {
    ledger: Ledger | null = null;

    constructor(
        private ledgerService: LedgerService,
        private routerService: Router,
        private route: ActivatedRoute,
    ) {}

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

    ngOnInit(): void {
        this.route.params.pipe(map((params) => params['id'])).subscribe({
            next: (value) => {
                this.ledgerService.getLedger(value).subscribe({
                    next: (ledger) => {
                        this.ledger = ledger;
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
}
