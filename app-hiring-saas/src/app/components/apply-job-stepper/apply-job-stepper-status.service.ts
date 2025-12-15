import { Injectable } from '@angular/core';
import { BehaviorSubject, catchError, Observable, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { SafeHtml } from '@angular/platform-browser';
import { JobResponse, JobStepperRequest, JobStepperResponse } from '../../models/job-stepper.model';
import { JobApplicationSearchRequest } from '../../models/applicant.model';

@Injectable({
  providedIn: 'root'
})
export class ApplyJobStepperStatusService {

  private readonly API_URL = environment.apiUrl;

  // Admin
  public addedSteps: { name: string; icon: SafeHtml; isSelected: boolean, previewMode: boolean, fields: any[], description: string, isDefault: boolean }[] = [];
  public activeStep: number = 0;
  public formTitle: string = '';
  public formDescription: string = '';
  public formId: number | null = null;
  public isLoading: boolean = false;

  // Customer
  public jobSteps: {isSelected: boolean, isActive: boolean, isValid: boolean, isDefault: boolean}[] = [];
  public activeJobStep: number = 0;
  public calendlyUrl: string | null = null;
  public externalCalendarUrl: string | null = null;

  selectedJob!: JobResponse | undefined;

  constructor(
    private http: HttpClient,
  ) {
  }

  // Basic Info
  private basicInfoFormValidSource = new BehaviorSubject<boolean>(false);
  basicInfoformValid$ = this.basicInfoFormValidSource.asObservable();

  private basicInfoFormValuesSource = new BehaviorSubject<any>(null);
  basicInfoFormValues$ = this.basicInfoFormValuesSource.asObservable();

  setBasicInfoFormValid(isValid: boolean) {
    this.basicInfoFormValidSource.next(isValid);
  }

  setBasicInfoFormValues(values: any) {
    this.basicInfoFormValuesSource.next(values);
  }

  getBasicInfoFormValues() {
    return this.basicInfoFormValuesSource.getValue();
  }

  // Requirements
  private requirementsFormValidSource = new BehaviorSubject<boolean>(false);
  requirementsValid$ = this.requirementsFormValidSource.asObservable();

  private requirementsFormValuesSource = new BehaviorSubject<any>(null);
  requirementsFormValues$ = this.requirementsFormValuesSource.asObservable();

  setRequirementsFormValid(isValid: boolean) {
    this.requirementsFormValidSource.next(isValid);
  }

  setRequirementsFormValues(values: any) {
    this.requirementsFormValuesSource.next(values);
  }

  getRequirementsFormValues() {
    return this.requirementsFormValuesSource.getValue();
  }

  // Income
  private incomeFormValidSource = new BehaviorSubject<boolean>(false);
  incomeValid$ = this.incomeFormValidSource.asObservable();

  setIncomeFormValid(isValid: boolean) {
    this.incomeFormValidSource.next(isValid);
  }

  // Your Bio
  private yourBioFormValidSource = new BehaviorSubject<boolean>(false);
  yourBioValid$ = this.yourBioFormValidSource.asObservable();

  private yourBioFormValuesSource = new BehaviorSubject<any>(null);
  yourBioFormValues$ = this.yourBioFormValuesSource.asObservable();

  setYourBioFormValid(isValid: boolean) {
    this.yourBioFormValidSource.next(isValid);
  }

  setYourBioFormValues(values: any) {
    this.yourBioFormValuesSource.next(values);
  }

  getYourBioFormValues() {
    return this.yourBioFormValuesSource.getValue();
  }

  // Videos
  private videosFormValidSource = new BehaviorSubject<boolean>(false);
  videosValid$ = this.videosFormValidSource.asObservable();

  private videosFormValuesSource = new BehaviorSubject<any>(null);
  videosFormValues$ = this.videosFormValuesSource.asObservable();

  setVideosFormValid(isValid: boolean) {
    this.videosFormValidSource.next(isValid);
  }

  setVideosFormValues(values: any) {
    this.videosFormValuesSource.next(values);
  }

  getVideosFormValues() {
    return this.videosFormValuesSource.getValue();
  }

  sendJobApplication(data: FormData): Observable<any> {
    return this.http
      .post<any>(`${this.API_URL}Job/application`, data)
      .pipe(
        catchError((err) => {
          return throwError(() => err);
        })
      );
  }

  saveJobStepper(data: JobStepperRequest): Observable<any> {
    return this.http
      .post<any>(`${this.API_URL}Job/form`, data)
      .pipe(
        catchError((err) => {
          return throwError(() => err);
        })
      );
  }

  updateJobStepper(data: JobStepperRequest): Observable<any> {
    return this.http
      .put<any>(`${this.API_URL}Job/form`, data)
      .pipe(
        catchError((err) => {
          return throwError(() => err);
        })
      );
  }

  getJobSteppers(data: JobApplicationSearchRequest): Observable<JobStepperResponse> {
    return this.http
      .post<JobStepperResponse>(`${this.API_URL}Job/form/search`, data)
      .pipe(
        catchError((err) => {
          return throwError(() => err);
        })
      );
  }

  getLandingJobsById(id: number): Observable<Array<JobResponse>> {
    return this.http
      .get<Array<JobResponse>>(`${this.API_URL}Job/details/from/` + id)
      .pipe(
        catchError((err) => {
          return throwError(() => err);
        })
      );
  }

  getLandingJobs(hash: string): Observable<JobResponse> {
    return this.http
      .get<JobResponse>(`${this.API_URL}Job/details/` + hash)
      .pipe(
        catchError((err) => {
          return throwError(() => err);
        })
      );
  }
}
