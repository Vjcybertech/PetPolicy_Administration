import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PolicyDto, CreatePolicyDto, RenewPolicyDto, CancelPolicyDto, UpdatePolicyDto } from '../models/policy.model';
import { DashboardDto } from '../models/dashboard.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class PolicyService {
  private baseUrl = `${environment.apiUrl}/api/policies`;

  constructor(private http: HttpClient) {}

  getDashboard(): Observable<DashboardDto> {
    return this.http.get<DashboardDto>(`${this.baseUrl}/dashboard`);
  }

  getPolicies(status?: string, pageNumber: number = 1, pageSize: number = 10): Observable<PolicyDto[]> {
    let params = new HttpParams()
      .set('pageNumber', pageNumber)
      .set('pageSize', pageSize);
    if (status) params = params.set('status', status);
    return this.http.get<PolicyDto[]>(this.baseUrl, { params });
  }

  getPolicyByNumber(policyNumber: string): Observable<PolicyDto> {
    return this.http.get<PolicyDto>(`${this.baseUrl}/${policyNumber}`);
  }

  getExpiringPolicies(withinDays: number = 30): Observable<PolicyDto[]> {
    const params = new HttpParams().set('withinDays', withinDays);
    return this.http.get<PolicyDto[]>(`${this.baseUrl}/expiring`, { params });
  }

  createPolicy(dto: CreatePolicyDto): Observable<PolicyDto> {
    return this.http.post<PolicyDto>(this.baseUrl, dto);
  }

  updatePolicy(policyNumber: string, dto: UpdatePolicyDto): Observable<PolicyDto> {
    return this.http.put<PolicyDto>(`${this.baseUrl}/${policyNumber}`, dto);
  }

  renewPolicy(policyNumber: string, dto: RenewPolicyDto): Observable<PolicyDto> {
    return this.http.post<PolicyDto>(`${this.baseUrl}/${policyNumber}/renew`, dto);
  }

  cancelPolicy(policyNumber: string, dto: CancelPolicyDto): Observable<PolicyDto> {
    return this.http.post<PolicyDto>(`${this.baseUrl}/${policyNumber}/cancel`, dto);
  }

  exportPolicies(): Observable<Blob> {
    return this.http.get(`${environment.apiUrl}/api/excel/policies/export`, { responseType: 'blob' });
  }
}
