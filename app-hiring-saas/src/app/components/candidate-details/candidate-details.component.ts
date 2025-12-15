import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { Applicant } from '../../models/applicant.model';
import { CandidatesService } from '../../services/candidates.service';
import { take } from 'rxjs';
import { VideoDialogComponent, VideoDialogData } from '../video-dialog/video-dialog.component';
import { MatDialog } from '@angular/material/dialog';

@Component({
  selector: 'app-candidate-details',
  imports: [CommonModule, TranslateModule],
  templateUrl: './candidate-details.component.html',
  styleUrl: './candidate-details.component.scss'
})
export class CandidateDetailsComponent implements OnInit{
  applicant: Applicant | null = null;
  loading: boolean = false;
  error: string | null = null;
  isMovingToInterview = false;
  isRejecting = false;
  shortPropertyKeys = [
    'CANDIDATES.PROFESSIONAL_EXPERIENCE',
    'CANDIDATES.BANK_ACCOUNT',
    'CANDIDATES.WORK_AUTHORIZATION',
    'CANDIDATES.BACKGROUND_CHECK_CONSENT'
  ];
  constructor(
    private _route: ActivatedRoute,
    private _router: Router,
    private _candidatesService: CandidatesService,
    private _dialog: MatDialog,
  ) {}

  ngOnInit(): void {
    const id = this._route.snapshot.paramMap.get('id');
    if (id) {
      this.getCandidateById(Number(id));
    }
  }

  private getCandidateById(id: number): void {
    this.loading = true;
    this._candidatesService.getApplicant(id).pipe(take(1)).subscribe({
      next: (candidate) => {
        this.applicant = candidate;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading candidate:', error);
        this.error = 'Failed to load candidate details';
        this.loading = false;
      }
    });
  }

  goBack(): void {
    this._router.navigate(['/candidates']);
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

  getTimelineStatusClass(status: string | undefined): string {
    switch (status) {
      case 'completed': return 'bg-green-500';
      case 'current': return 'bg-yellow-500';
      case 'pending': return 'bg-gray-300';
      default: return 'bg-gray-300';
    }
  }

  getTimelineStatusBadgeClass(status: string | undefined): string {
    switch (status) {
      case 'completed': return 'bg-green-100 text-green-800';
      case 'current': return 'bg-yellow-100 text-yellow-800';
      case 'pending': return 'bg-gray-100 text-gray-800';
      default: return 'bg-gray-100 text-gray-800';
    }
  }

  moveToInterview(applicant: Applicant) {
    this.isMovingToInterview = true;
    this._candidatesService.updateCandidateStatus(applicant.id, 3).pipe(take(1))
      .subscribe({
        next: () => {
          this.getCandidateById(applicant.id);
          this.isMovingToInterview = false;
        },
        error: (err) => {
          console.error('Failed to update candidate status', err);
          this.isMovingToInterview = false;
        }
      });
  }

  rejectApplication(applicant: Applicant): void {
    this.isRejecting = true;
    this._candidatesService.updateCandidateStatus(applicant.id, 6).pipe(take(1))
      .subscribe({
        next: () => {
          this.getCandidateById(applicant.id);
          this.isRejecting = false;
        },
        error: (err) => {
          console.error('Failed to update candidate status', err);
          this.isRejecting = false;
        }
      });
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

  viewVideoApplication(applicant: Applicant) {
    if (!applicant.applicationVideo?.url) {
          console.warn('No video available for this candidate');
          return;
        }

        const dialogData: VideoDialogData = {
          candidateName: `${applicant.firstName} ${applicant.lastName}`,
          videoUrl: applicant.applicationVideo.url,
        };

        const dialogRef = this._dialog.open(VideoDialogComponent, {
          width: '90vw',
          maxWidth: '800px',
          maxHeight: '90vh',
          data: dialogData,
          panelClass: 'video-dialog-container'
        });
  }

  getCandidateSkillsTags() {
    let tagsString!: string;
    if (this.applicant?.jobApplicationSections){
      this.applicant?.jobApplicationSections.forEach((section: any) => {
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

  getRelevantProperties(section: any) {
    return section.jobApplicationSectionProperties
      .filter((p: any, index: number) => index < 4 && (p.stringValue === 'Yes' || p.stringValue === 'No'));
  }

  getDynamicSections() {
    if (!this.applicant?.jobApplicationSections) {
      return [];
    }

    // Filter out sections that are already displayed
    return this.applicant.jobApplicationSections.filter((section: any, index: number) => {
      // Skip the first section (index 0) which is shown in "Requirements Check"
      if (index === 0) return false;
      
      // Skip sections that contain "tags" type properties (shown in "Experience & Skills")
      const hasTagsProperty = section.jobApplicationSectionProperties?.some(
        (prop: any) => prop.type === 'tags'
      );
      
      // Skip sections that are empty or have no properties with values
      const hasValues = section.jobApplicationSectionProperties?.some(
        (prop: any) => prop.stringValue || prop.integerValue !== null || 
                     prop.dateTimeValue !== null || prop.booleanValue !== null
      );
      
      return !hasTagsProperty && hasValues;
    });
  }

  // Helper method to check if a checkbox option is selected
  isOptionSelected(selectedValues: string, option: string): boolean {
    if (!selectedValues) return false;
    return selectedValues.includes(option);
  }

  // Helper method to parse tags string into array
  getTagsArray(tagsString: string): string[] {
    if (!tagsString) return [];
    return tagsString.split(',').map(tag => tag.trim());
  }
}