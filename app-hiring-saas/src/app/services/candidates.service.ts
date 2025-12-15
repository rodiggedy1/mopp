import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HiredStatistics, JobApplicationSearchRequest, JobApplicationSearchResponse, JobApplicationStatistics } from '../models/applicant.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class CandidatesService {

  constructor(private http: HttpClient) {}

  searchJobApplications(request: JobApplicationSearchRequest): Observable<JobApplicationSearchResponse> {
    return this.http.post<JobApplicationSearchResponse>(`${environment.apiUrl}Job/application/search`, request);
  }

  getApplicant(id: number): Observable<any> {
    return this.http.get<any>(`${environment.apiUrl}Job/application/${id}`);
  }

  updateCandidateStatus(jobApplicationId: number, statusId: number, kibanaSortOrder?: number): Observable<any> {
    const requestBody = new FormData();
    requestBody.append('JobApplicationId', jobApplicationId.toString());
    requestBody.append('Status', statusId.toString());
    if (kibanaSortOrder !== undefined) {
      requestBody.append('KibanaSortOrder', kibanaSortOrder.toString());
    }

    return this.http.put(`${environment.apiUrl}Job/application/status`, requestBody);
  }

  getStatuses() {
    return this.http.get(`${environment.apiUrl}Job/application/statuses`);
  }

  getDashboardStatistics(): Observable<JobApplicationStatistics> {
    return this.http.get<JobApplicationStatistics>(`${environment.apiUrl}Job/application/dashboard/statistics`);
  }

  getHiredStatistics(): Observable<HiredStatistics> {
    return this.http.get<HiredStatistics>(`${environment.apiUrl}Job/application/hired/statistics`);
  }
}

