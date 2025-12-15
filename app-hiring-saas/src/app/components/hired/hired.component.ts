import { Component, OnDestroy, OnInit } from '@angular/core';
import { SidebarComponent } from '../sidebar/sidebar.component';
import { TranslateModule } from '@ngx-translate/core';
import { CandidatesService } from '../../services/candidates.service';
import { CommonModule } from '@angular/common';
import { Subscription, take } from 'rxjs';
import { Applicant, HiredStatistics, JobApplicationSearchRequest } from '../../models/applicant.model';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-hired',
  imports: [SidebarComponent, TranslateModule, CommonModule],
  templateUrl: './hired.component.html',
  styleUrl: './hired.component.scss'
})
export class HiredComponent implements OnInit, OnDestroy {
  hiredCandidates: Applicant[] = [];
  statistics: HiredStatistics | null = null;
  private subscription: Subscription = new Subscription();

  constructor(private _candidatesService: CandidatesService, private _authService: AuthService, private _router: Router) {}

ngOnInit() {
    this.loadCandidates();
    this.getHiredStatistics();

    this._authService.currentUserSubject.subscribe(data => {
      if (data) {
        let isSubscribed = localStorage.getItem('subscriptionStatus');
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
  }

  loadCandidates() {

    const request: JobApplicationSearchRequest = {
      query: "",
      statusId: 5,
      jobDetailsCreatedById: null,
      jobDetailsId: null,
      status: [],
      dateFrom: null,
      dateTo: null,
      paging: {
        pageNumber: 1,
        pageSize: 50
      },
      sorting: {
        field: 1,
        sortOrder: 0
      }
    };

    this.subscription.add(
      this._candidatesService.searchJobApplications(request).subscribe({
        next: (response) => {
          this.hiredCandidates = response.items || [];
        },
        error: (error) => {
          console.error('Error loading candidates:', error);
        }
      })
    );
  }

  getHiredStatistics() {
    this._candidatesService.getHiredStatistics().pipe(take(1)).subscribe({
      next: (statistics) => {
        this.statistics = statistics;
      },
      error: (error) => {
        console.error('Error fetching dashboard statistics:', error);
      }
    })
  }

  trackByFn(index: number, item: Applicant): number {
    return item.id;
  }
}
