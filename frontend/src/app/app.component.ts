import { Component } from '@angular/core';
import { RouterModule, RouterOutlet } from '@angular/router';
import { AuthService } from './services/auth.service';

@Component({
    selector: 'app-root',
    imports: [RouterOutlet, RouterModule],
    templateUrl: './app.component.html',
})
export class AppComponent {
    title = 'Bank';
    constructor(private authService: AuthService) {}

    get isAdmin() {
        return this.authService.isAdmin;
    }
}
