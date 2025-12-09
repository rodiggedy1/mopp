import { Component, OnInit } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { AuthService } from '../../services/auth.service';
import { take } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-payment-success',
  imports: [TranslateModule],
  templateUrl: './payment-success.component.html',
  styleUrl: './payment-success.component.scss',
})
export class PaymentSuccessComponent implements OnInit {
  sessionId: string | null = null;

  constructor(
    private _authService: AuthService,
    private _router: Router,
    private _route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this._route.queryParamMap.pipe(take(1)).subscribe((params) => {
      this.sessionId = params.get('session_id');
      if (this.sessionId) {
        this.verifyPayment();
      } else {
        console.error('No session ID in URL.');
      }
    });
  }

  verifyPayment() {
    this._authService.verifyPayment(this.sessionId!).pipe(take(1)).subscribe({
      next: (response) => {
        this.loadSubscriptionStatus();
      },
      error: (error) => {
        console.error('Payment verification failed:', error);
      },
    });
  }

  loadSubscriptionStatus() {
    this._authService.getSubscriptionStatus().pipe(take(1)).subscribe({
      next: (res) => {
        localStorage.setItem('subscriptionStatus', res.status);
        setTimeout(() => {
          this._router.navigate(['/dashboard']);
        }, 3000);
      }
    });
  }
}
