import { Routes } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { LedgerComponent } from './components/ledger/ledger.component';
import { LedgerDetailComponent } from './components/ledger/detail.component';
import { AuthGuard } from './guards/auth.guard';
import { BookingComponent } from './components/booking/booking.component';
import { CreateLedgerComponent } from './components/create/create.component';

export const routes: Routes = [
    {
        path: 'ledgers',
        component: LedgerComponent,
        canActivate: [AuthGuard],
        title: 'Ledgers',
    },
    {
        path: 'ledgers/:id',
        component: LedgerDetailComponent,
        canActivate: [AuthGuard],
    },
    {
        path: 'booking',
        component: BookingComponent,
        canActivate: [AuthGuard],
        title: 'Transfer Funds',
    },
    {
        path: 'create',
        component: CreateLedgerComponent,
        canActivate: [AuthGuard],
        title: 'New Ledger',
    },
    {
        path: 'login',
        component: LoginComponent,
        title: 'Login',
    },
    {
        path: '',
        redirectTo: '/login',
        pathMatch: 'full',
    },
];
