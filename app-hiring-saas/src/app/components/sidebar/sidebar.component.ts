import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { NavigationEnd, Router, RouterModule } from '@angular/router';
import { filter, takeUntil } from 'rxjs/operators';
import { AuthService } from '../../services/auth.service';
import { Subject } from 'rxjs';
import { TranslateModule } from '@ngx-translate/core';
import { User } from '../../models/auth.model';
import { MatDialog } from '@angular/material/dialog';
import { UnsubscribedAlertDialogComponent } from '../subscription/unsubscribed-alert-dialog/unsubscribed-alert-dialog.component';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule, RouterModule, TranslateModule],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.scss'
})
export class SidebarComponent implements OnInit, OnDestroy {
  currentRoute: string = '';
  private destroy$ = new Subject<void>();
  user: User | null = null;
  showUnsubscribedDialog: boolean = false;

  constructor(private router: Router, private _authService: AuthService, private dialog: MatDialog) {}

  ngOnInit() {
    this.currentRoute = this.router.url;

    this.router.events
      .pipe(filter(event => event instanceof NavigationEnd), takeUntil(this.destroy$))
      .subscribe((event: NavigationEnd) => {
        this.currentRoute = event.url;
      });

      this._authService.currentUser$
      .pipe(takeUntil(this.destroy$))
      .subscribe(user => {
        this.user = user;
      });

    this._authService.showUnsubscribedMessage.pipe(takeUntil(this.destroy$)).subscribe(data => {
      if (data && this._authService.isSubscribeDialogOpen) {
        this._authService.isSubscribeDialogOpen = false;
        if (this.user && this.user.email != 'administrator@localhost') {
          this.showUnsubscribedMessageDialog();
        }
      }
    })
  }

  showUnsubscribedMessageDialog() {
    const dialogRef = this.dialog.open(UnsubscribedAlertDialogComponent, {
      width: '600px',
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result && result == 'subscribe') {
        this._authService.isSubsribedClicked = true;
        this._authService.onSubscribePayment.next(true);
      }
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  isActiveRoute(route: string): boolean {
    return this.currentRoute === route;
  }

  logout() {
    this._authService.logout();
  }
}
