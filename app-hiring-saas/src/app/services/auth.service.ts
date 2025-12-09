import { Inject, Injectable, PLATFORM_ID } from '@angular/core';
import { environment } from '../../environments/environment';
import { BehaviorSubject, catchError, map, Observable, of, Subject, tap, throwError } from 'rxjs';
import { ChangePasswordRequest, ChangePasswordResponse, LoginRequest, PasswordResetRequest, ResendEmail, Token, TokenPayload, User, UserBase, UserRequest, UserResponse, UserRole, VerifyData } from '../models/auth.model';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { LookupValue } from '../models/lookup.model';
import { Router } from '@angular/router';
import { ApiResponse, paymentSendImprovement } from '../models/shared.model';
import { isPlatformBrowser } from '@angular/common';
import { CalendlyUserCredentials } from '../models/job-stepper.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly API_URL = environment.apiUrl;

  public currentUserSubject = new BehaviorSubject<User | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  public isAuthenticatedSubject = new BehaviorSubject<boolean>(false);
  public isAuthenticated$ = this.isAuthenticatedSubject.asObservable();

  public redirectUrl: string | null = null;
  public showUnsubscribedMessage: BehaviorSubject<boolean> = new BehaviorSubject(false);
  public isSubscribeDialogOpen: boolean = false;
  public onSubscribePayment: BehaviorSubject<boolean> = new BehaviorSubject(false);
  public isSubsribedClicked: boolean = false;

  constructor(
    private http: HttpClient,
    private router: Router,
    @Inject(PLATFORM_ID) private platformId: Object,) {
    this.initializeAuthState();
  }

  private initializeAuthState(): void {
    setTimeout(() => {
      const token = this.getToken();
      if (token) {
        this.getUserData().pipe(
          map((user: User) => {
            this.currentUserSubject.next(user);
            this.isAuthenticatedSubject.next(true);
            return user;
          }),
          catchError((err) => {
            console.error('[Auth] getUserData error:', err);
            this.logout();
            return of(null);
          })
        ).subscribe();
      }
    }, 0);
  }

  login(credentials: LoginRequest): Observable<Token> {
    return this.http
      .post<Token>(`${this.API_URL}Authenticate/login`, credentials)
      .pipe(
        catchError((err) => {
          return throwError(() => err);
        })
      );
  }

  resetPassword(data: PasswordResetRequest): Observable<void> {
    return this.http
      .post<void>(`${this.API_URL}User/reset-password`, data)
      .pipe(
        catchError((err) => {
          return throwError(() => err);
        })
      );
  }
  getUserData(): Observable<User> {
    return this.http.get<User>(`${this.API_URL}User/me`).pipe(
      catchError((err) => {
        return throwError(() => err);
      })
    );
  }

  createUser(user: UserBase): Observable<UserResponse> {
    return this.http.post<UserResponse>(`${environment.apiUrl}User`, user);
  }

  updateUserProfile(formData: FormData): Observable<UserResponse> {
    const headers = new HttpHeaders({
      'Skip-Content-Type': 'true',
    });

    return this.http
      .put<UserResponse>(`${this.API_URL}User`, formData, { headers })
      .pipe(
        tap(() => {
          this.getUserData().subscribe({
            next: (user) => this.currentUserSubject.next(user),
            error: (err) => console.error('Failed to refresh user data:', err)
          });
        })
      );
  }

  saveCalendlyCredentials(request: CalendlyUserCredentials): Observable<UserResponse> {
    return this.http
      .put<any>(`${this.API_URL}User/calendly-credentials`, request)
      .pipe(
        tap(() => {
          this.getUserData().subscribe({
            next: (user) => this.currentUserSubject.next(user),
            error: (err) => console.error('Failed to refresh user data:', err)
          });
        })
      );
  }

  uploadProfilePicture(file: File): Observable<any> {
    const formData = new FormData();
    formData.append('Picture', file);

    const headers = new HttpHeaders({
      'Skip-Content-Type': 'true',
      Language: 'en',
    });

    return this.http.put(`${this.API_URL}User/profile-picture`, formData, {
      headers,
    });
  }

  changePassword(
    request: ChangePasswordRequest
  ): Observable<ChangePasswordResponse> {
    return this.http.put<ChangePasswordResponse>(
      `${this.API_URL}User/change-password`,
      request
    );
  }
  getUserRoles(): Observable<LookupValue[]> {
    return this.http.get<LookupValue[]>(`${this.API_URL}User/role`);
  }

  getCompanySizes(): Observable<LookupValue[]> {
    return this.http.get<LookupValue[]>(`${this.API_URL}User/company/size`);
  }

  forgotPassword(forgotPassword: { email: string }): Observable<void> {
    return this.http
      .post<void>(`${this.API_URL}User/forgot-password`, forgotPassword)
      .pipe(
        catchError((err) => {
          return throwError(() => err);
        })
      );
  }

  public putVerify(postDataResetPass: VerifyData): Observable<VerifyData> {
    return this.http
      .put<VerifyData>(`${this.API_URL}Authenticate/verify`, postDataResetPass)
      .pipe(
        catchError((err) => {
          return throwError(() => err);
        })
      );
  }

  public resendCode(request: ResendEmail): Observable<VerifyData> {
    return this.http
      .put<VerifyData>(`${this.API_URL}Authenticate/verify/resend-code`, request)
      .pipe(
        catchError((err) => {
          return throwError(() => err);
        })
      );
  }

  logout(): void {
    this.clearAuthData();
    this.router.navigate(['/auth/login']);
  }

  saveTokens(authTokens: Token): void {
    if (isPlatformBrowser(this.platformId)) {
      localStorage.setItem('TokenAcc', authTokens.accessToken);
      localStorage.setItem('TokenRef', authTokens.refreshToken);
    }
  }

  private clearAuthData(): void {
    this.currentUserSubject.next(null);
    this.isAuthenticatedSubject.next(false);

    if (isPlatformBrowser(this.platformId)) {
      localStorage.removeItem('TokenAcc');
      localStorage.removeItem('TokenRef');
      localStorage.removeItem('subscriptionStatus');
    }
  }

  getToken(): string | null {
    if (isPlatformBrowser(this.platformId)) {
      return localStorage.getItem('TokenAcc');
    }
    return null;
  }

  getRefreshToken() {
    if (isPlatformBrowser(this.platformId)) {
      return localStorage.getItem('TokenRef');
    }
    return null;
  }

  isAuthenticated(): boolean {
    const token = this.getToken();
    return token !== null && !this.isTokenExpired(token);
  }

  private isTokenExpired(token: string): boolean {
    try {
      const payload = this.decodeToken(token);
      const currentTime = Math.floor(Date.now() / 1000);
      return payload.exp < currentTime;
    } catch (error) {
      return true;
    }
  }

  private decodeToken(token: string): TokenPayload {
    const base64Url = token.split('.')[1];
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    const jsonPayload = decodeURIComponent(
      atob(base64)
        .split('')
        .map((c) => {
          return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
        })
        .join('')
    );

    return JSON.parse(jsonPayload);
  }

  getCurrentUser(): User | null {
    return this.currentUserSubject.value;
  }

  hasRole(role: UserRole): boolean {
    const user = this.getCurrentUser();
    return user?.type === role;
  }

  isVendor(): boolean {
    return this.hasRole(UserRole.VENDOR);
  }

  isClient(): boolean {
    return this.hasRole(UserRole.CLIENT);
  }

 generateRefreshToken(): Observable<any> {
    let token = new Token(this.getToken()!, this.getRefreshToken()!);
    return this.http.post<Token>(
      `${environment.apiUrl}Authenticate/refresh-token`,
      token
    ).pipe(
      catchError(err => {
        return throwError(() => err);
      })
    );
  }

  // Update user profile
  updateProfile(updates: Partial<User>): Observable<User> {
    return this.http
      .put<ApiResponse<User>>(`${this.API_URL}/auth/profile`, updates)
      .pipe(
        map((response) => {
          if (response.success && response.data) {
            return response.data;
          }
          throw new Error(response.message || 'Profile update failed');
        }),
        tap((user) => {
          this.currentUserSubject.next(user);
        })
      );
  }

  createCheckout(): Observable<any> {
    return this.http.get<any>(`${this.API_URL}Payment/create-checkout`).pipe(
      catchError((err) => {
        return throwError(() => err);
      })
    );
  }

  verifyPayment(sessionId: string): Observable<any> {
    return this.http.get<any>(`${this.API_URL}Payment/verify-payment?sessionId=${sessionId}`).pipe(
      catchError((err) => {
        return throwError(() => err);
      })
    );
  }

  getSubscriptionStatus() {
    return this.http.get<any>(`${this.API_URL}Payment/subscription-status`).pipe(
      catchError((err) => {
        return throwError(() => err);
      })
    );
  }

  getPaymentStatus() {
    return this.http.get<any>(`${this.API_URL}Payment/status`).pipe(
      catchError((err) => {
        return throwError(() => err);
      })
    );
  }

  pausePaymentForTwoMonths() {
    return this.http.post<any>(`${this.API_URL}Payment/pause-subscription`, null).pipe(
      catchError((err) => {
        return throwError(() => err);
      })
    );
  }

  claimFiftyPercentOff() {
    return this.http.post<any>(`${this.API_URL}Payment/apply-discount`, null).pipe(
      catchError((err) => {
        return throwError(() => err);
      })
    );
  }

  cancelPaymentSubscription() {
    return this.http.post<any>(`${this.API_URL}Payment/cancel-subscription`, null).pipe(
      catchError((err) => {
        return throwError(() => err);
      })
    );
  }

  extendTrialOneMonth() {
    return this.http.post<any>(`${this.API_URL}Payment/extend-trial`, null).pipe(
      catchError((err) => {
        return throwError(() => err);
      })
    );
  }

  sendImprove(reqest: paymentSendImprovement) {
    return this.http.post<any>(`${this.API_URL}User/note`, reqest).pipe(
      catchError((err) => {
        return throwError(() => err);
      })
    );
  }

  getNoteTypes() {
    return this.http.get<LookupValue[]>(`${this.API_URL}User/note/type`).pipe(
      catchError((err) => {
        return throwError(() => err);
      })
    );
  }

  saveCalendlyUrl(url: string) {
    return this.http.put<any>(`${this.API_URL}User/calendly-profile-url?CalendlyProfileUrl=` + url, null).pipe(
      catchError((err) => {
        return throwError(() => err);
      })
    );
  }

  manageBilling(): Observable<any> {
    return this.http.get<any>(`${this.API_URL}Payment/billing-portal-session`).pipe(catchError((err) => {
        return throwError(() => err);
      })
    );
  }

}
