import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { AuthService } from '../../services/auth.service';
import { Subject, take, takeUntil } from 'rxjs';
import { SidebarComponent } from '../sidebar/sidebar.component';
import { CancelSubscribeDialogComponent } from './cancel-subscribe-dialog/cancel-subscribe-dialog.component';
import { MatDialog } from '@angular/material/dialog';
import { paymentCancelAlert } from '../../models/shared.model';
import { User } from '../../models/auth.model';

@Component({
  selector: 'app-subscription',
  standalone: true,
  imports: [CommonModule, TranslateModule, SidebarComponent],
  templateUrl: './subscription.component.html',
  styleUrl: './subscription.component.scss'
})

export class SubscriptionComponent implements OnInit, OnDestroy {
  subscriptionStatus: string | null = null;
  trialEnd: string | Date | null = null;
  hasSubscription = false;
  trialDetails: string | null = null;
  onCloseAlert: paymentCancelAlert = new paymentCancelAlert();
  private destroy$ = new Subject<void>();
  user: User | null = null;

  constructor(
    private _authService: AuthService,
    private dialog: MatDialog
  ) {}

  ngOnInit(): void {
    this._authService.currentUser$
    .pipe(takeUntil(this.destroy$))
    .subscribe(user => {
      this.user = user;
      this.loadSubscriptionStatus();
    });

    this._authService.onSubscribePayment.subscribe(data => {
      if (data && this._authService.isSubsribedClicked) {
        this._authService.isSubsribedClicked = false;
        this.subscribe();
      }
    });
  }

  loadSubscriptionStatus() {
    this._authService.getSubscriptionStatus().pipe(take(1)).subscribe({
      next: (res) => {
        let isSubscribed: string | null;
        if (this.user && this.user.email != 'administrator@localhost') {
          localStorage.setItem('subscriptionStatus', res.status);
          isSubscribed = res.status;
        } else {
          localStorage.setItem('subscriptionStatus', 'active');
          isSubscribed = 'active';
        }
        this.trialEnd = res.currentPeriodEnd ? new Date(res.currentPeriodEnd) : res.trialEnd ? new Date(res.trialEnd) : null;
        this.hasSubscription = res.status === 'trialing' || res.status === 'active' && res.status !== null;
        if (this.trialEnd) {
          const today = new Date();
          const diffTime = this.trialEnd.getTime() - today.getTime();
          const daysRemaining = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
          if (daysRemaining > 0) {
            localStorage.setItem('subscriptionStatus', 'active');
          }
          this.trialDetails = `${daysRemaining} days remaining • Expires ${this.trialEnd.toLocaleDateString()}`;
        }
      },
      error: (err) => {
        console.error('Failed to load subscription status:', err);
        this.hasSubscription = false;
      }
    });
  }

  getSubscriptionDescriptionKey(status: string | null): string {
    switch (status) {
      case 'trialing': return 'ACCOUNT.CURRENT_SUBSCRIPTION_DESC_TRIALING';
      case 'active': return 'ACCOUNT.CURRENT_SUBSCRIPTION_DESC_ACTIVE';
      case 'past_due': return 'ACCOUNT.CURRENT_SUBSCRIPTION_DESC_PAST_DUE';
      case 'canceled': return 'ACCOUNT.CURRENT_SUBSCRIPTION_DESC_CANCELED';
      case 'incomplete': return 'ACCOUNT.CURRENT_SUBSCRIPTION_DESC_INCOMPLETE';
      case 'incomplete_expired': return 'ACCOUNT.CURRENT_SUBSCRIPTION_DESC_INCOMPLETE_EXPIRED';
      case 'unpaid': return 'ACCOUNT.CURRENT_SUBSCRIPTION_DESC_UNPAID';
      default: return '';
    }
  }

  subscribe() {
    this._authService.createCheckout().pipe(take(1)).subscribe({
      next: (response) => {
        window.location.href = response.url;
      },
      error: (error) => {
        console.error('Create checkout failed:', error);
      }
    });
  }

  cancelSubscription() {
    const dialogRef = this.dialog.open(CancelSubscribeDialogComponent, {
      width: window.innerWidth > 768 ? '1150px' : '90%',
      maxWidth: '95vw',
      maxHeight: '95vh',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.onCloseAlert = result;
      }
    });
  }

  closeCancelAlert() {
    this.onCloseAlert = new paymentCancelAlert();
  }

  manageBilling() {
    this._authService.manageBilling().pipe(take(1)).subscribe({
      next: (response) => {
        window.location.href = response.url;
      },
      error: (error) => {
        console.error('Manage billing failed:', error);
      }
    });
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
