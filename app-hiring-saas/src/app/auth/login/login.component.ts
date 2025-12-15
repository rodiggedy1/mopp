import { Component, OnDestroy } from '@angular/core';
import { ReactiveFormsModule, FormsModule, FormGroup, FormBuilder, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { Subject, take, takeUntil } from 'rxjs';
import { LoginRequest, User } from '../../models/auth.model';

@Component({
  selector: 'app-login',
  imports: [TranslateModule, ReactiveFormsModule, CommonModule, RouterModule, FormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent implements OnDestroy {

  loginForm!: FormGroup;
  isLoading = false;
  errorMessage = '';
  showPassword = false;
  private destroy$ = new Subject<void>();

  constructor(
    private _fb: FormBuilder,
    private _authService: AuthService,
    private _router: Router,
    private _translate: TranslateService,
  ) {
    this.initializeForm();
  }

  private initializeForm(): void {
    this.loginForm = this._fb.group({
      email: ['', [
        Validators.required,
        Validators.email,
        Validators.minLength(5),
        Validators.maxLength(100)
      ]],
      password: ['', [
        Validators.required,
        Validators.maxLength(50)
      ]],
      rememberMe: [false]
    });
  }
  onSubmit(): void {
    if (this.loginForm.valid && !this.isLoading) {
      this.isLoading = true;
      this.errorMessage = '';

      const loginData: LoginRequest = {
        username: this.loginForm.value.email.trim().toLowerCase(),
        password: this.loginForm.value.password
      };

      this._authService.login(loginData)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: (response) => {
            this._authService.saveTokens(response);
            this._authService.getUserData().pipe(take(1)).subscribe((data: User) => {
              this._authService.currentUserSubject.next(data);
              this._authService.isAuthenticatedSubject.next(true);
              this._router.navigate(['/dashboard']);
            });
          },
          error: (error) => {
            this.isLoading = false;
            if (error.error.detail && error.error.detail === 'Account not verified'){
            }
            error.error.detail && !error.error.detail.includes('error.message') ? this.errorMessage = error.error.detail : this.handleLoginError(error);
          }
        });
    } else {
      this.markFormGroupTouched();
    }
  }

  private markFormGroupTouched(): void {
    Object.keys(this.loginForm.controls).forEach(key => {
      const control = this.loginForm.get(key);
      control?.markAsTouched();
    });
  }

  togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword;
  }

  navigateToSignup(): void {
    this._router.navigate(['/auth/signup']);
  }

  navigateToForgotPassword(): void {
    this._router.navigate(['/auth/forgot-password']);
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.loginForm.get(fieldName);
    return !!(field && field.invalid && field.touched);
  }

  getFieldError(fieldName: string): string {
    const field = this.loginForm.get(fieldName);
    if (field && field.errors && field.touched) {
      if (field.errors['required']) {
        return this._translate.instant('AUTH.VALIDATION.REQUIRED', { field: fieldName });
      }
      if (field.errors['email']) {
        return this._translate.instant('AUTH.VALIDATION.EMAIL');
      }
      if (field.errors['minlength']) {
        return this._translate.instant('AUTH.VALIDATION.MIN_LENGTH', {
          field: fieldName,
          min: field.errors['minlength'].requiredLength
        });
      }
      if (field.errors['maxlength']) {
        return this._translate.instant('AUTH.VALIDATION.MAX_LENGTH', {
          field: fieldName,
          max: field.errors['maxlength'].requiredLength
        });
      }
    }
    return '';
  }

  private handleLoginError(error: any): void {
    if (error.status === 401) {
      this.errorMessage = 'Invalid email or password. Please try again.';
    } else if (error.status === 429) {
      this.errorMessage = 'Too many login attempts. Please try again later.';
    } else if (error.status === 0) {
      this.errorMessage = 'Unable to connect to server. Please check your internet connection.';
    } else {
      this.errorMessage = error.error?.message || 'Login failed. Please try again.';
    }
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
