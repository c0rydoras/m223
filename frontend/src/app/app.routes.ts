import { Routes } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { LedgerComponent } from './components/ledger/ledger.component';
import { AuthGuard } from './guards/auth.guard';
import { BookingComponent } from './components/booking/booking.component';
import { CreateLedgerComponent } from './components/create/create.component';

export const routes: Routes = [
    {
        path: 'ledgers',
        component: LedgerComponent,
        canActivate: [AuthGuard],
    },
    {
        path: 'booking',
        component: BookingComponent,
        canActivate: [AuthGuard],
    },
    {
        path: 'create',
        component: CreateLedgerComponent,
        canActivate: [AuthGuard],
    },
    {
        path: 'login',
        component: LoginComponent,
    },
    {
        path: '',
        redirectTo: '/login',
        pathMatch: 'full',
    },
];
