// src/app/guards/auth.guard.ts
import { Injectable } from '@angular/core';
import { CanActivate, Router, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthService } from '../services/auth.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean> | Promise<boolean> | boolean {
    if (this.authService.isAuthenticated()) {
      return true;
    } else
    if (localStorage.getItem('TokenAcc')) {
      this.authService.getUserData().subscribe({
        next: (user) => {
          this.authService.currentUserSubject.next(user);
          this.authService.isAuthenticatedSubject.next(true);
          return true;
        }, 
        error: (err) => {
          this.authService.logout();
          this.router.navigate(['/auth/login']);
          return false;
        }
      });
    } else {
      this.router.navigate(['/auth/login']);
      return false;
    }

    // Store the attempted URL for redirecting after login
    this.authService.redirectUrl = state.url;

    // Navigate to login page
    this.router.navigate(['/auth/login']);
    return false;
  }
}
