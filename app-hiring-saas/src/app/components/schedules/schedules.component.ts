import { AfterViewInit, Component, inject, OnDestroy, OnInit } from '@angular/core';
import { SidebarComponent } from '../sidebar/sidebar.component';
import { AuthService } from '../../services/auth.service';
import { catchError, Subject, take, takeUntil, throwError } from 'rxjs';
import { User } from '../../models/auth.model';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { CalendlyUserCredentials } from '../../models/job-stepper.model';
import { environment } from '../../../environments/environment';
import { TranslateModule } from '@ngx-translate/core';
declare const Calendly: any;

interface CalendlyTokenResponse {
  token_type: string;
  access_token: string;
  refresh_token?: string;
  expires_in?: number;
  scope?: string;
}

interface CalendlyUserResponse {
  resource: {
    uri: string;
    name: string;
    slug: string;
    email: string;
    scheduling_url: string;
    timezone: string;
    avatar_url: string;
    created_at: string;
    updated_at: string;
  };
}

interface CalendlyScheduledEventsResponse {
  collection: ScheduledEvent[];
  pagination: {
    count: number;
    next_page?: string;
    previous_page?: string;
  };
}

interface ScheduledEvent {
  uri: string;
  name: string;
  status: string;
  start_time: string;
  end_time: string;
  event_type: string;
  location?: {
    type: string;
    location?: string;
  };
  invitees_counter: {
    total: number;
    active: number;
    limit: number;
  };
  created_at: string;
  updated_at: string;
}

@Component({
  selector: 'app-schedules',
  imports: [SidebarComponent, CommonModule, FormsModule, MatProgressSpinnerModule, TranslateModule],
  templateUrl: './schedules.component.html',
  styleUrl: './schedules.component.scss'
})
export class SchedulesComponent implements OnInit, AfterViewInit, OnDestroy {

  inputUrl: string = '';
  calendlyUrl: string | null = null;
  hasCalendlyUrl: boolean = false;
  isLoading: boolean = true;
  showSaveBtn: boolean = true;

  private http = inject(HttpClient);

  tokenType: string | null = null;
  calendlyAccessToken: string | null = null;
  calendlyRefreshToken: string | null = null;
  calendlyExpiresAt: number | null = null;

  userData: any = null;
  activeUser: User | null = null;

  userUri: string | null = null;
  errorMessage: string = '';
  scheduledEvents: any = null;

  externalCalendarUrl: string = "";
  isSavingExternalUrl: boolean = false;
  externalUrlSaved: boolean = false;

  private destroy$ = new Subject<void>();

  constructor(
    private _authService: AuthService,
    private route: ActivatedRoute,
    private router: Router,
  ) {}

  ngOnInit(): void {

    // If redirected from Calendly (first-time OAuth)
    this.route.queryParams.subscribe(params => {
      const code = params['code'];
      if (code) {
        this.handleAuthorizationCode(code);
      }
    });

    // Load user's stored Calendly credentials
    this._authService.currentUser$
      .pipe(takeUntil(this.destroy$))
      .subscribe(data => {
        if (data && data.calendlyDetails) {
          this.activeUser = data;

          if (data.calendlyDetails.accessToken) {
            this.calendlyAccessToken = data.calendlyDetails.accessToken;
            this.calendlyRefreshToken = data.calendlyDetails.refreshToken;
            this.calendlyExpiresAt = data.calendlyDetails.tokenExpiresAt ? Number(data.calendlyDetails.tokenExpiresAt) : null;
            this.tokenType = "Bearer";

            this.tryLoadCalendlySession();
          }
        }
      });
  }

  ngAfterViewInit(): void {
    this._authService.getUserData()
      .pipe(take(1))
      .subscribe((data: User) => {
        if (data && data.calendlyProfileUrl) {
          this.hasCalendlyUrl = true;
          this.calendlyUrl = data.calendlyProfileUrl;
          this.inputUrl = data.calendlyProfileUrl;
          this.showSaveBtn = false;
        }
        this.isLoading = false;
      });
  }

  // --------------------------------------------
  // ✅ 1. Handle OAuth Authorization Code Exchange
  // --------------------------------------------
  handleAuthorizationCode(code: string) {

    const url = 'https://auth.calendly.com/oauth/token';
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });

    const body = {
      grant_type: 'authorization_code',
      client_id: environment.calendlyClientId,
      client_secret: environment.calendlyClientSecret,
      redirect_uri: environment.calendlyRedirectUri,
      code: code
    };

    this.http.post<CalendlyTokenResponse>(url, body, { headers })
      .pipe(catchError(err => throwError(() => err)))
      .subscribe({
        next: data => {
          this.tokenType = data.token_type;
          this.calendlyAccessToken = data.access_token;
          this.calendlyRefreshToken = data.refresh_token || null;
          this.calendlyExpiresAt = Date.now() + (data.expires_in || 3600) * 1000;
          this.router.navigate([], {
            queryParams: {},
            replaceUrl: true
          });

          this.saveCalendlyCredentials(data, code);
          this.getCalendlyPublicUrl();
        },
        error: err => console.error(err)
      });
  }

  // --------------------------------------------
  // ✅ 2. Load Existing Session (from DB)
  // --------------------------------------------
  tryLoadCalendlySession() {
    if (!this.calendlyRefreshToken || !this.calendlyExpiresAt || !this.calendlyAccessToken) {
      this.isLoading = false;
      return;
    }

    const now = Date.now();

    if (now < this.calendlyExpiresAt) {
      this.getCalendlyPublicUrl();
    } else {
      this.refreshAccessToken(this.calendlyRefreshToken);
    }
  }

  // --------------------------------------------
  // ✅ 3. Refresh Calendly Access Token
  // --------------------------------------------
  refreshAccessToken(refreshToken: string) {
    const url = 'https://auth.calendly.com/oauth/token';
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });

    const body = {
      grant_type: 'refresh_token',
      client_id: environment.calendlyClientId,
      client_secret: environment.calendlyClientSecret,
      refresh_token: refreshToken
    };

    this.http.post<CalendlyTokenResponse>(url, body, { headers })
      .pipe(catchError(err => throwError(() => err)))
      .subscribe({
        next: data => {

          // ✅ Update local values
          this.tokenType = data.token_type;
          this.calendlyAccessToken = data.access_token;
          this.calendlyRefreshToken = data.refresh_token || refreshToken;
          this.calendlyExpiresAt = Date.now() + (data.expires_in || 3600) * 1000;

          // ✅ Save updated tokens in DB
          this.saveCalendlyCredentials(data, null);

          this.getCalendlyPublicUrl();
        },
        error: err => console.error((err: any) => {
          console.error("Error refreshing token:", err);
        })
      });
  }

  // --------------------------------------------
  // ✅ 4. Save Calendly credentials to backend
  // --------------------------------------------
  saveCalendlyCredentials(data: CalendlyTokenResponse, code: string | null) {

    const credentials: CalendlyUserCredentials = {
      calendlyAccessToken: data.access_token,
      calendlyRefreshToken: data.refresh_token ?? this.calendlyRefreshToken,
      calendlyClientId: environment.calendlyClientId,
      calendlyClientSecret: environment.calendlyClientSecret,
      calendlyCode: code,
      calendlyRedirectUri: environment.calendlyRedirectUri,
      CalendlyTokenExpiresAt: data.expires_in
        ? (Date.now() + data.expires_in * 1000).toString()
        : this.calendlyExpiresAt?.toString() || null
    };

    this._authService.saveCalendlyCredentials(credentials)
      .pipe(take(1))
      .subscribe({
        next: () => {
          this.calendlyAccessToken = data.access_token;
          this.calendlyRefreshToken = data.refresh_token ?? this.calendlyRefreshToken;
          this.calendlyExpiresAt = data.expires_in ? (Date.now() + data.expires_in * 1000) : this.calendlyExpiresAt;
          this.tokenType = "Bearer";
          this._authService.getUserData().pipe(take(1)).subscribe(user => {
          this.tryLoadCalendlySession();
          });
        },
        error: err => console.error("Error saving Calendly credentials:", err)
      });
  }

  // --------------------------------------------
  // ✅ 5. Get Calendly User Info
  // --------------------------------------------
  getCalendlyPublicUrl(): void {
    const headers = new HttpHeaders({
      'Authorization': `${this.tokenType} ${this.calendlyAccessToken}`,
      'Content-Type': 'application/json',
      'Skip-Auth': 'true'
    });

    this.http.get<CalendlyUserResponse>('https://api.calendly.com/users/me', { headers })
      .pipe(catchError(err => throwError(() => err)))
      .subscribe({
        next: data => {
          this.userData = data;
          this.userUri = data.resource.uri;
          this.getScheduledEvents();
        },
        error: err => console.error("Error fetching user:", err)
      });
  }

  // --------------------------------------------
  // ✅ 6. Fetch Scheduled Events
  // --------------------------------------------
  getScheduledEvents(): void {

    const url = `https://api.calendly.com/scheduled_events?user=${this.userUri}`;

    const headers = new HttpHeaders({
      'Authorization': `Bearer ${this.calendlyAccessToken}`,
      'Content-Type': 'application/json',
      'Skip-Auth': 'true'
    });

    this.http.get<CalendlyScheduledEventsResponse>(url, { headers })
      .pipe(catchError(err => throwError(() => err)))
      .subscribe({
        next: data => this.scheduledEvents = data,
        error: err => console.error("Error fetching events:", err)
      });
  }

  // --------------------------------------------
  // ✅ Save profile URL
  // --------------------------------------------
  saveUrl() {
    if (!this.inputUrl) return alert('Please enter a valid URL');

    this.isLoading = true;

    this._authService.saveCalendlyUrl(this.inputUrl)
      .pipe(take(1))
      .subscribe({
        next: () => {
          this.calendlyUrl = this.inputUrl;
          this.showSaveBtn = false;
          this.isLoading = false;
        }
      });
  }

  // --------------------------------------------

  loadCalendlyWidget() {
    const container = document.getElementById('calendly-embed');
    if (!container) return;

    container.innerHTML = '';

    if (typeof Calendly !== 'undefined' && this.calendlyUrl) {
      Calendly.initInlineWidget({
        url: this.calendlyUrl,
        parentElement: container
      });
    }
  }

  calendlyLogin() {
    window.open(
      `https://auth.calendly.com/oauth/authorize?client_id=${environment.calendlyClientId}&response_type=code&redirect_uri=${environment.calendlyRedirectUri}`,
      '_blank'
    );
  }

  saveExternalCalendarUrl(url: string) {
    if (!url) return;

    this.isSavingExternalUrl = true;
    this.externalUrlSaved = false;

    this._authService.saveExternalCalendarUrl(url)
      .pipe(take(1))
      .subscribe({
        next: () => {
          this.isSavingExternalUrl = false;
          this.externalUrlSaved = true;
          this._authService.getUserData()
          .pipe(take(1))
          .subscribe({
            next: (user) => {
              this._authService.currentUserSubject.next(user);
            },
            error: (err) => console.error('Failed to refresh user data', err)
          });
        },
        error: () => {
          this.isSavingExternalUrl = false;
        }
      });
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }
}