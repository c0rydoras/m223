import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Ledger } from '../models/ledger.interface';
import { AuthService } from './auth.service';

@Injectable({
    providedIn: 'root',
})
export class LedgerService {
    private apiUrl = 'http://localhost:5000/api/v1';

    constructor(
        private http: HttpClient,
        private authService: AuthService,
    ) {}

    getLedgers(): Observable<Ledger[]> {
        return this.http.get<Ledger[]>(`${this.apiUrl}/ledgers`);
    }

    getLedger(id: string | number): Observable<Ledger> {
        return this.http.get<Ledger>(`${this.apiUrl}/ledgers/${id}`);
    }

    transferFunds(fromLedgerId: number, toLedgerId: number, amount: number): Observable<unknown> {
        const payload = {
            fromLedgerId,
            toLedgerId,
            amount,
        };
        return this.http.post(`${this.apiUrl}/ledgers/transfer`, payload);
    }

    createNew(name: string): Observable<unknown> {
        const payload = {
            name,
        };
        return this.http.post(`${this.apiUrl}/ledgers`, payload);
    }

    deleteLedger(id: number) {
        return this.http.delete(`${this.apiUrl}/ledgers/${id}`);
    }
}
