import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { SidebarComponent } from '../sidebar/sidebar.component';
import { MatDialog } from '@angular/material/dialog';
import { VideoDialogComponent, VideoDialogData } from '../video-dialog/video-dialog.component';
import { TranslateModule } from '@ngx-translate/core';
import { Subscription } from 'rxjs';
import { CandidatesService } from '../../services/candidates.service';
import { Applicant, JobApplicationSearchRequest } from '../../models/applicant.model';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-candidates',
  imports: [CommonModule, FormsModule, SidebarComponent, TranslateModule],
  templateUrl: './candidates.component.html',
  styleUrl: './candidates.component.scss'
})
export class CandidatesComponent implements OnInit, OnDestroy {
  searchTerm: string = '';
  selectedStatus: string = 'all';
  candidates: Applicant[] = [];
  filteredCandidates: Applicant[] = [];
  loading: boolean = false;
  error: string | null = null;
  expandedExperience: { [candidateId: number]: boolean } = {};
  private subscription: Subscription = new Subscription();

  constructor(
    private dialog: MatDialog,
    private _candidatesService: CandidatesService,
    private _router: Router,
    private _authService: AuthService
  ) {}

  ngOnInit() {
    this.loadCandidates();

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
    this.loading = true;
    this.error = null;

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
          this.candidates = response.items || [];
          this.applyFilters();
          this.loading = false;
        },
        error: (error) => {
          console.error('Error loading candidates:', error);
          this.error = 'Failed to load candidates';
          this.loading = false;
        }
      })
    );
  }

  applyFilters() {
    this.filteredCandidates = [...this.candidates];
      this.filteredCandidates.forEach(candidate => {
        candidate.firstName = candidate.firstName || 'Unknown';
        candidate.lastName = candidate.lastName || 'User'; 
      })

    if (this.searchTerm && this.searchTerm.trim()) {
      const searchLower = this.searchTerm.toLowerCase().trim();
      this.filteredCandidates = this.filteredCandidates.filter(candidate => {
        const fullName = `${candidate.firstName || ''} ${candidate.lastName || ''}`.toLowerCase();
        const email = (candidate.emailAddress || '').toLowerCase();
        return fullName.includes(searchLower) || email.includes(searchLower);
      });
    }

    if (this.selectedStatus !== 'all') {
      this.filteredCandidates = this.filteredCandidates.filter(candidate => {
        const statusName = candidate.status?.name?.toLowerCase() || 'applied';
        return statusName === this.selectedStatus.toLowerCase();
      });
    }
  }

  onSearchChange() {
    this.applyFilters();
  }

  onStatusChange() {
    this.applyFilters();
  }

  get statusCounts() {
    const getStatusName = (candidate: any) => candidate.status?.name?.toLowerCase() || 'applied';

    return {
      all: this.candidates.length,
      applied: this.candidates.filter(c => getStatusName(c) === 'applied').length,
      screening: this.candidates.filter(c => getStatusName(c) === 'screening').length,
      interview: this.candidates.filter(c => getStatusName(c) === 'interview').length,
      reference: this.candidates.filter(c => getStatusName(c) === 'reference').length,
      hired: this.candidates.filter(c => getStatusName(c) === 'hired').length
    };
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
      default:
        return 'bg-gray-100 text-gray-800';
    }
  }

  viewVideoApplication(candidate: Applicant): void {
    if (!candidate.applicationVideo?.url) {
      console.warn('No video available for this candidate');
      return;
    }

    const dialogData: VideoDialogData = {
      candidateName: `${candidate.firstName} ${candidate.lastName}`,
      videoUrl: candidate.applicationVideo.url,
    };

    const dialogRef = this.dialog.open(VideoDialogComponent, {
      width: '90vw',
      maxWidth: '800px',
      maxHeight: '90vh',
      data: dialogData,
      panelClass: 'video-dialog-container'
    });
  }

  toggleExperience(candidate: any): void {
    this.expandedExperience[candidate.id] = !this.expandedExperience[candidate.id];
  }

  isExperienceExpanded(candidateId: number): boolean {
    return !!this.expandedExperience[candidateId];
  }

  getCandidateSkills(candidate: any) {
    let tagsString!: string;
    if (candidate.jobApplicationSections){
      candidate.jobApplicationSections.forEach((section: any) => {
        if (section.jobApplicationSectionProperties) {
          section.jobApplicationSectionProperties.forEach((field: any) => {
            if (field.type == 'tags') {
              tagsString = field.stringValue;
            }
          })
        }
      })
    }

    if (tagsString) {
      let list = tagsString.split(',').map((item: any) => item.trim());
      return list
    } else {
      return []
    }
  }

  getCandidateExperience(candidate: Applicant) {
    let expereince!: string;
    candidate.jobApplicationSections.forEach((section: any) => {
      let findExperience = section.jobApplicationSectionProperties.find((field: any) => field.type == 'about-textarea' );
      if (findExperience) {
        expereince = findExperience.stringValue;
      }
    })
    return expereince;
  }
}
