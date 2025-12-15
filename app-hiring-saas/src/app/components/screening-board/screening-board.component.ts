import { Component, OnInit, OnDestroy } from '@angular/core';
import { SidebarComponent } from '../sidebar/sidebar.component';
import { CdkDragDrop, moveItemInArray, transferArrayItem } from '@angular/cdk/drag-drop';
import { MaterialModule } from '../../modules/material/material.module';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { CandidatesService } from '../../services/candidates.service';
import { Subscription, take } from 'rxjs';
import { Applicant, JobApplicationSearchRequest } from '../../models/applicant.model';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

type ColumnName = 'applied' | 'screening' | 'interview' | 'reference' | 'hired';

@Component({
  selector: 'app-screening-board',
  imports: [SidebarComponent, MaterialModule, CommonModule, TranslateModule],
  templateUrl: './screening-board.component.html',
  styleUrl: './screening-board.component.scss'
})
export class ScreeningBoardComponent implements OnInit, OnDestroy {
  applied: Applicant[] = [];
  screening: Applicant[] = [];
  interview: Applicant[] = [];
  reference: Applicant[] = [];
  hired: Applicant[] = [];
  loading: boolean = false;
  error: string | null = null;

  columnPages: Record<ColumnName, number> = {
    applied: 1,
    screening: 1,
    interview: 1,
    reference: 1,
    hired: 1
  };

  columnLoading: Record<ColumnName, boolean> = {
    applied: false,
    screening: false,
    interview: false,
    reference: false,
    hired: false
  };

  columnHasMore: Record<ColumnName, boolean> = {
    applied: true,
    screening: true,
    interview: true,
    reference: true,
    hired: true
  };

  isDragOverMap: Record<ColumnName, boolean> = {
    applied: false,
    screening: false,
    interview: false,
    reference: false,
    hired: false
  };

  draggingCandidateId: number | null = null;
  private subscription: Subscription = new Subscription();

  private statusMapping = {
    'applied': 1,
    'screening': 2,
    'interview': 3,
    'reference': 4,
    'hired': 5
  };

  private statusIdToName: { [key: number]: string } = {
    1: 'applied',
    2: 'screening',
    3: 'interview',
    4: 'reference',
    5: 'hired'
  };

  columnTotalCount: Record<ColumnName, number> = {
    applied: 0,
    screening: 0,
    interview: 0,
    reference: 0,
    hired: 0
  };

  constructor(private candidatesService: CandidatesService, private _router: Router, private _authService: AuthService) {}

  ngOnInit() {
    this.loadInitialData();

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

  private loadInitialData() {
    Object.keys(this.statusMapping).forEach(columnName => {
      this.loadMoreForColumn(columnName as ColumnName, true);
    });
  }

  loadMoreForColumn(column: ColumnName, isInitial = false) {
    if (this.columnLoading[column] || (!this.columnHasMore[column] && !isInitial)) {
      return;
    }

    this.columnLoading[column] = true;

    const statusId = this.statusMapping[column];
    const loadedCandidateIds = this[column].map(c => c.id).filter(id => id !== undefined);
    const request: JobApplicationSearchRequest = {
      query: null,
      statusId: statusId,
      jobDetailsCreatedById: null,
      jobDetailsId: null,
      status: [statusId],
      dateFrom: null,
      dateTo: null,
      paging: {
        pageNumber: this.columnPages[column],
        pageSize: 5
      },
      sorting: {
        field: 1,
        sortOrder: 0
      }
    };

    this.subscription.add(
      this.candidatesService.searchJobApplications(request).pipe(
        take(1)
      ).subscribe({
        next: (response) => {
          const newItems = response.items || [];

          const filteredItems = newItems.filter(item =>
            !loadedCandidateIds.includes(item.id)
          );

          if (isInitial || Math.abs(this.columnTotalCount[column] - (response.totalCount ?? 0)) > 1) {
            this.columnTotalCount[column] = response.totalCount ?? filteredItems.length;
          }

          if (filteredItems.length === 0) {
            this.columnHasMore[column] = false;
          } else {
            this[column] = [...this[column], ...filteredItems];
            this.columnPages[column]++;

            if (filteredItems.length < 5) {
              this.columnHasMore[column] = false;
            }
          }

          this.columnLoading[column] = false;
        },
        error: (error) => {
          console.error(`Error loading more for ${column}:`, error);
          this.columnLoading[column] = false;

          if (isInitial) {
            this.error = `Failed to load ${column} candidates`;
          }
        }
      })
    );
  }

  onColumnScroll(event: Event, column: ColumnName) {
    const element = event.target as HTMLElement;
    const threshold = 50;

    if (element.scrollHeight - element.scrollTop <= element.clientHeight + threshold) {
      this.loadMoreForColumn(column);
    }
  }
  onDragStarted(candidateId: number | undefined) {
    if (candidateId !== undefined) {
      this.draggingCandidateId = candidateId;
    }
  }

  onDragEnded() {
    this.draggingCandidateId = null;
    Object.keys(this.isDragOverMap).forEach(key => {
      this.isDragOverMap[key as ColumnName] = false;
    });
  }

  onDragEntered(column: ColumnName) {
    this.isDragOverMap[column] = true;
  }

  onDragExited(column: ColumnName) {
    this.isDragOverMap[column] = false;
  }

  drop(event: CdkDragDrop<Applicant[]>) {
    if (event.previousContainer === event.container) {
      moveItemInArray(event.container.data, event.previousIndex, event.currentIndex);
      this.updateSortOrders(event.container.data);
    } else {
      const candidate = event.previousContainer.data[event.previousIndex];
      const newStatusName = this.getStatusFromContainerId(event.container.id);
      const oldStatusName = this.getStatusFromContainerId(event.previousContainer.id);
      const newStatusId = this.statusMapping[newStatusName as keyof typeof this.statusMapping];

      if (candidate && candidate.id && newStatusId) {
        const originalContainer = event.previousContainer.data;
        const originalIndex = event.previousIndex;
        const originalStatus = { ...candidate.status };

        transferArrayItem(
          event.previousContainer.data,
          event.container.data,
          event.previousIndex,
          event.currentIndex
        );

        this.updateColumnCounts(oldStatusName as ColumnName, newStatusName as ColumnName);
        this.resetColumnPagination(oldStatusName as ColumnName, newStatusName as ColumnName);

        candidate.status = {
          id: newStatusId,
          name: this.getStatusDisplayName(newStatusName)
        };

        this.updateCandidateStatusAPI(candidate, newStatusId, event.currentIndex, {
          originalContainer,
          originalIndex,
          originalStatus,
          newContainer: event.container.data,
          newIndex: event.currentIndex,
          oldStatusName: oldStatusName as ColumnName,
          newStatusName: newStatusName as ColumnName
        });
      }
    }

    this.onDragEnded();
  }

  private updateColumnCounts(fromColumn: ColumnName, toColumn: ColumnName) {
    if (this.columnTotalCount[fromColumn] > 0) {
      this.columnTotalCount[fromColumn]--;
    }
    this.columnTotalCount[toColumn]++;
  }

 updateCandidateStatusAPI(
    candidate: Applicant,
    newStatusId: number,
    sortOrder: number,
    rollbackData: any
  ) {
    this.loading = true;
    this.candidatesService.updateCandidateStatus(
      candidate.id,
      newStatusId,
      sortOrder
    ).pipe(take(1)).subscribe({
      next: () => {
        this.loading = false;
      },
      error: (error) => {
        this.loading = false;
        console.error('Failed to update candidate status:', error);

        transferArrayItem(
          rollbackData.newContainer,
          rollbackData.originalContainer,
          rollbackData.newIndex,
          rollbackData.originalIndex
        );

        this.rollbackColumnCounts(rollbackData.oldStatusName, rollbackData.newStatusName);

        candidate.status = rollbackData.originalStatus;
        this.error = 'Failed to update candidate status. Please try again.';
      }
    });
  }

  private rollbackColumnCounts(fromColumn: ColumnName, toColumn: ColumnName) {
    if (this.columnTotalCount[toColumn] > 0) {
      this.columnTotalCount[toColumn]--;
    }
    this.columnTotalCount[fromColumn]++;
  }

  private updateSortOrders(columnData: Applicant[]) {
    columnData.forEach((candidate, index) => {
      if (candidate.id && candidate.status?.id) {
        this.loading = true;
        this.candidatesService.updateCandidateStatus(
          candidate.id,
          candidate.status.id,
          index
        ).pipe(take(1)).subscribe({
          next: () => this.loading = false,
          error: (error) => console.error('Failed to update sort order:', error)
        });
      }
    });
  }

  private getStatusFromContainerId(containerId: string): string {
    const cleanId = containerId.replace(/^cdk-drop-list-\d+-/, '');
    const containerStatusMap: { [key: string]: string } = {
      'applied': 'applied',
      'screening': 'screening',
      'interview': 'interview',
      'reference': 'reference',
      'hired': 'hired'
    };

    return containerStatusMap[cleanId] || 'applied';
  }

  private getStatusDisplayName(statusName: string): string {
    const displayNames: { [key: string]: string } = {
      'applied': 'Applied',
      'screening': 'Screening',
      'interview': 'Interview',
      'reference': 'ReferenceCheck',
      'hired': 'Hired'
    };

    return displayNames[statusName] || 'Applied';
  }

  shouldShowPlaceholder(column: ColumnName): boolean {
    const columnData = this[column] as Applicant[];
    return columnData.length === 0 && !this.isDragOverMap[column] && !this.columnLoading[column];
  }

  trackByFn(index: number, item: Applicant) {
    return item.id;
  }

  refreshBoard() {
    this.applied = [];
    this.screening = [];
    this.interview = [];
    this.reference = [];
    this.hired = [];

    Object.keys(this.columnPages).forEach(key => {
      this.columnPages[key as ColumnName] = 1;
      this.columnHasMore[key as ColumnName] = true;
    });

    this.loadInitialData();
  }

  private resetColumnPagination(fromColumn: ColumnName, toColumn: ColumnName) {
    // Handle source column (where candidates were removed)
    const fromColumnCurrentCount = this[fromColumn].length;
    const fromColumnTotalCount = this.columnTotalCount[fromColumn];

    // If we have fewer loaded items than total, and space to load more
    if (fromColumnCurrentCount < fromColumnTotalCount && fromColumnCurrentCount < 5) {
      // Load remaining candidates to fill the visible space
      this.loadRemainingCandidatesForColumn(fromColumn);
    }

    // Reset pagination state for source column
    const fromColumnPageSize = 5;
    const fromColumnExpectedPage = Math.floor(fromColumnCurrentCount / fromColumnPageSize) + 1;

    if (this.columnPages[fromColumn] > fromColumnExpectedPage) {
      this.columnPages[fromColumn] = fromColumnExpectedPage;
    }

    this.columnHasMore[fromColumn] = this.columnTotalCount[fromColumn] > fromColumnCurrentCount;

    // For destination column, ensure hasMore is correct
    const toColumnCurrentCount = this[toColumn].length;
    this.columnHasMore[toColumn] = this.columnTotalCount[toColumn] > toColumnCurrentCount;
  }

  private loadRemainingCandidatesForColumn(column: ColumnName) {
    const currentCount = this[column].length;
    const totalCount = this.columnTotalCount[column];
    const pageSize = 5;

    // Calculate how many more we can load to fill the visible space
    const spaceAvailable = pageSize - (currentCount % pageSize);
    const remainingItems = totalCount - currentCount;

    if (remainingItems > 0 && spaceAvailable > 0 && !this.columnLoading[column]) {
      console.log(`Loading remaining candidates for ${column}: current=${currentCount}, total=${totalCount}, space=${spaceAvailable}`);

      this.columnLoading[column] = true;
      const statusId = this.statusMapping[column];
      const loadedCandidateIds = this[column].map(c => c.id).filter(id => id !== undefined);

      const request: JobApplicationSearchRequest = {
        query: null,
        statusId: statusId,
        jobDetailsCreatedById: null,
        jobDetailsId: null,
        status: [statusId],
        dateFrom: null,
        dateTo: null,
        paging: {
          pageNumber: 1, // Start from page 1 to get all remaining
          pageSize: Math.min(remainingItems, 10) // Load up to 20 remaining items
        },
        sorting: {
          field: 1,
          sortOrder: 0
        }
      };

      this.subscription.add(
        this.candidatesService.searchJobApplications(request).pipe(
          take(1)
        ).subscribe({
          next: (response) => {
            const newItems = response.items || [];

            // Filter out already loaded candidates
            const filteredItems = newItems.filter(item =>
              !loadedCandidateIds.includes(item.id)
            );

            if (filteredItems.length > 0) {
              // Add the missing candidates
              this[column] = [...this[column], ...filteredItems];

              // Update pagination state
              const newCurrentCount = this[column].length;
              this.columnPages[column] = Math.floor(newCurrentCount / pageSize) + 1;
              this.columnHasMore[column] = newCurrentCount < this.columnTotalCount[column];
            }

            this.columnLoading[column] = false;
          },
          error: (error) => {
            console.error(`Error loading remaining candidates for ${column}:`, error);
            this.columnLoading[column] = false;
          }
        })
      );
    }
  }

}
