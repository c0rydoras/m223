import { Injectable } from '@angular/core';

@Injectable({
    providedIn: 'root',
})
export class AuthService {
    private tokenKey = 'authToken';
    private isAuthenticated: boolean | null = null;

    isLoggedIn() {
        if (this.isAuthenticated == null) {
            this.isAuthenticated = this.getToken() != null;
        }

        return this.isAuthenticated;
    }

    setToken(token: string) {
        localStorage.setItem(this.tokenKey, token);
        this.isAuthenticated = true;
    }

    getToken(): string | null {
        return localStorage.getItem(this.tokenKey);
    }

    clearToken() {
        localStorage.removeItem(this.tokenKey);
        this.isAuthenticated = false;
    }

    get user() {
        const token = this.getToken();
        if (!token) return;
        try {
            return JSON.parse(atob(token.split('.')[1]));
        } catch {
            this.clearToken();
        }
    }

    get isAdmin() {
        const user = this.user;
        if (!user) return;
        return this.user.role === 'Administrators';
    }
}
