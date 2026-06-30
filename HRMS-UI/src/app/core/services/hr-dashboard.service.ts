import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { HrDashboardStats, DepartmentSummary, LeaveSummary } from '../models/hr-dashboard.model';

@Injectable({
  providedIn: 'root'
})
export class HrDashboardService {

  constructor(private http: HttpClient) {}

  getStats(): Observable<HrDashboardStats> {
    return this.http.get<HrDashboardStats>(`${environment.apiUrl}/hr-dashboard/stats`);
  }

  getDepartmentSummary(): Observable<DepartmentSummary[]> {
    return this.http.get<DepartmentSummary[]>(`${environment.apiUrl}/hr-dashboard/department-summary`);
  }

  getLeaveSummary(): Observable<LeaveSummary> {
    return this.http.get<LeaveSummary>(`${environment.apiUrl}/hr-dashboard/leave-summary`);
  }
}
