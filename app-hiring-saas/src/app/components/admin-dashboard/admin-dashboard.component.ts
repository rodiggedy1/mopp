import { Component, OnDestroy, OnInit, Renderer2 } from '@angular/core';
import { SidebarComponent } from '../sidebar/sidebar.component';
import { TranslateModule } from '@ngx-translate/core';
import { CommonModule } from '@angular/common';
import { CandidatesService } from '../../services/candidates.service';
import { Subject, Subscription, take, takeUntil } from 'rxjs';
import { Applicant, JobApplicationSearchRequest, JobApplicationStatistics } from '../../models/applicant.model';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { User } from '../../models/auth.model';

@Component({
  selector: 'app-admin-dashboard',
  imports: [SidebarComponent, TranslateModule, CommonModule, RouterModule],
  templateUrl: './admin-dashboard.component.html',
  styleUrl: './admin-dashboard.component.scss'
})
export class AdminDashboardComponent implements OnInit, OnDestroy {
  candidates: Applicant[] = [];
  statistics: JobApplicationStatistics | null = null;
  private subscription: Subscription = new Subscription();
  recentApplications: Applicant[] = [];
  showVideoPopup = false;
  private destroy$ = new Subject<void>();
  user: User | null = null;

  constructor(private _candidatesService: CandidatesService, private _authService: AuthService, private _router: Router, private renderer: Renderer2) {}

  ngOnInit() {
    this._authService.currentUser$
    .pipe(takeUntil(this.destroy$))
    .subscribe(user => {
      this.user = user;
      this.loadSubscriptionStatus();
    });

    this.getDashboardStatistics();
    this.loadRecentApplications();
    const videoPlayed = localStorage.getItem('dashboardVideoPlayed');
    if (!videoPlayed) {
      this.openVideoPopup();
    }
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

        let trialEnd = res.currentPeriodEnd ? new Date(res.currentPeriodEnd) : res.trialEnd ? new Date(res.trialEnd) : null;
        if (trialEnd) {
          const today = new Date();
          const diffTime = trialEnd.getTime() - today.getTime();
          const daysRemaining = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
          if (daysRemaining > 0) {
            localStorage.setItem('subscriptionStatus', 'active');
            isSubscribed = 'active';
          }
        }
        
        if (!isSubscribed || isSubscribed == 'canceled' || isSubscribed == 'incomplete' || isSubscribed == 'incomplete_expired' || isSubscribed == 'unpaid' || isSubscribed == 'none') {
          this._router.navigate(['./subscription']);
          this._authService.isSubscribeDialogOpen = true;
          this._authService.showUnsubscribedMessage.next(true);
        }
      }
    });
  }

  ngOnDestroy() {
    this.subscription.unsubscribe();
    this.destroy$.next();
    this.destroy$.complete();
  }

  getDashboardStatistics() {
    this._candidatesService.getDashboardStatistics().pipe(take(1)).subscribe({
      next: (statistics) => {
        this.statistics = statistics;
      },
      error: (error) => {
        console.error('Error fetching dashboard statistics:', error);
      }
    })
  }

  loadRecentApplications() {
    const request: JobApplicationSearchRequest = {
      query: null,
      statusId: null,
      jobDetailsCreatedById: null,
      jobDetailsId: null,
      status: null,
      dateFrom: null,
      dateTo: null,
      paging: {
        pageNumber: 1,
        pageSize: 10
      },
      sorting: {
        field: 1,
        sortOrder: 0
      }
    };

    this.subscription.add(
      this._candidatesService.searchJobApplications(request).pipe(take(1)).subscribe({
        next: (response) => {
          const sortedByDate = response.items?.sort((a, b) =>
            new Date(b.created).getTime() - new Date(a.created).getTime()
          ) || [];

          this.recentApplications = sortedByDate.slice(0, 4);
        },
        error: (error) => {
          console.error('Error loading recent applications:', error);
        }
      })
    );
  }

  formatDate(dateString: string | Date): string {
    const date = typeof dateString === 'string' ? new Date(dateString) : dateString;
    return date.toLocaleDateString('en-US', {
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  getStatusClass(statusName?: string | null): string {
    if (!statusName) return 'bg-gray-100 text-gray-800';

    switch (statusName.toLowerCase()) {
      case 'applied':
        return 'bg-gray-100 text-gray-800';
      case 'screening':
        return 'bg-blue-100 text-blue-800';
      case 'interview':
        return 'bg-yellow-100 text-yellow-800';
      case 'hired':
        return 'bg-green-100 text-green-800';
      case 'reference':
        return 'bg-purple-100 text-purple-800';
        case 'rejected':
        return 'bg-red-100 text-red-800';
      default:
        return 'bg-gray-100 text-gray-800';
    }
  }

  trackByFn(index: number, item: Applicant): number {
    return item.id;
  }

  formatComparison(value: number): string {
    if (value === 0) return '0%';
    const sign = value > 0 ? '+' : '';
    return `${sign}${value}%`;
  }

  openVideoPopup() {
    this.showVideoPopup = true;
    setTimeout(() => {
      const script1 = this.renderer.createElement('script');
      script1.src = 'https://fast.wistia.com/embed/medias/k3lliaxdjl.jsonp';
      script1.async = true;
      this.renderer.appendChild(document.body, script1);

      const script2 = this.renderer.createElement('script');
      script2.src = 'https://fast.wistia.com/assets/external/E-v1.js';
      script2.async = true;
      this.renderer.appendChild(document.body, script2);
    }, 0);
  }

  closePopup() {
    this.showVideoPopup = false;
    localStorage.setItem('dashboardVideoPlayed', 'true');
  }
}
