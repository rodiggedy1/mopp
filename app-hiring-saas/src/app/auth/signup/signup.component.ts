import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { AuthService } from '../../services/auth.service';
import { take } from 'rxjs';
import { LoginRequest, User } from '../../models/auth.model';

@Component({
  selector: 'app-signup',
  imports: [TranslateModule, RouterModule, CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './signup.component.html',
  styleUrl: './signup.component.scss'
})
export class SignupComponent {
  form: FormGroup;
  showPassword = false;
  loading: boolean = false;
  errorMessages: string[] = [];

  constructor(private _fb: FormBuilder, private _authService: AuthService) {
    this.form = this._fb.group({
      email: ['', [Validators.required, Validators.email]],
      firstName: ['', Validators.required],
      password: ['', Validators.required],
    });
  }

  togglePasswordVisibility() {
    this.showPassword = !this.showPassword;
  }

  onSubmit() {
    if (this.form.valid) {
      this.loading = true;
      const { firstName, email, password } = this.form.value
      const payload = {
        ...this.form.value,
        email: email.toLowerCase().trim()
      };

      this._authService.createUser(payload).pipe(take(1)).subscribe({
        next: (response) => {
          const credentials = {
            username: email,
            password: password
          };
          this.login(credentials);
        },
        error: (error) => {
          console.log('Handling register errors')
          this.errorMessages = []; // clear previous errors

          if (error?.error?.errors) {
            for (const key in error.error.errors) {
              if (error.error.errors.hasOwnProperty(key)) {
                const messages = error.error.errors[key];
                messages.forEach((message: string) => {
                  if (message == 'userEmailUniqueValidator.message') {
                    this.errorMessages.push('The email address you entered is already registered.');
                  } else {
                    this.errorMessages.push(message);
                  }
                });
              }
            }

            // Hide all messages after 5 seconds
            setTimeout(() => {
              this.errorMessages = [];
            }, 5000);
          }

          this.loading = false;
        }
      });
    } else {
      this.form.markAllAsTouched();
    }
  }

  login(credentials: LoginRequest) {
    this._authService.login(credentials).pipe(take(1)).subscribe({
      next: (response) => {
        this._authService.saveTokens(response);
        this._authService.getUserData().pipe(take(1)).subscribe((data: User) => {
        this._authService.currentUserSubject.next(data);
        this._authService.isAuthenticatedSubject.next(true);
        });
        this.createCheckout()
      },
      error: (error) => {
        console.error('Login failed:', error);
        this.loading = false;
      }
    });
  }

  createCheckout() {
    this._authService.createCheckout().pipe(take(1)).subscribe({
      next: (response) => {
        window.location.href = response.url;
        this.loading = false;
      },
      error: (error) => {
        console.error('Create checkout failed:', error);
        this.loading = false;
      }
    });
  }

  isFieldInvalid(field: string) {
    const control = this.form.get(field);
    return control?.invalid && (control?.touched || control?.dirty);
  }

  getFieldError(field: string) {
    const control = this.form.get(field);
    if (!control) return '';
    if (control.hasError('required')) return 'This field is required';
    if (control.hasError('email')) return 'Invalid email address';
    return '';
  }
}
