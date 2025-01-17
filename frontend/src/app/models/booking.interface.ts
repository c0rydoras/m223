import { Ledger } from './ledger.interface';

export interface Booking {
    id: number;
    sourceId: number;
    destinationId: number;
    amount: number;
    timestamp: string;

    source?: Ledger;
    destination?: Ledger;
}
