import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { 
  ManagerDashboardStats, 
  TeamMember, 
  LateEmployee, 
  PendingLeave, 
  TeamAttendance, 
  ManagerPerformanceReview, 
  AddPerformanceReview, 
  ManagerGoal, 
  AddGoal, 
  UpdateGoalStatus,
  GoalQuery
} from '../models/manager.model';

@Injectable({
  providedIn: 'root'
})
export class ManagerService {
  private apiUrl = `${environment.apiUrl}/manager`;

  constructor(private http: HttpClient) {}

  getDashboard(): Observable<ManagerDashboardStats> {
    return this.http.get<ManagerDashboardStats>(`${this.apiUrl}/dashboard`);
  }

  getTeamMembers(): Observable<TeamMember[]> {
    return this.http.get<TeamMember[]>(`${this.apiUrl}/team`);
  }

  getTeamMember(employeeId: string): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/team/${employeeId}`);
  }

  getTeamAttendance(): Observable<TeamAttendance[]> {
    return this.http.get<TeamAttendance[]>(`${this.apiUrl}/team-attendance`);
  }

  getLateEmployees(): Observable<LateEmployee[]> {
    return this.http.get<LateEmployee[]>(`${this.apiUrl}/late-employees`);
  }

  getPendingLeaves(): Observable<PendingLeave[]> {
    return this.http.get<PendingLeave[]>(`${this.apiUrl}/pending-leaves`);
  }

  addPerformanceReview(dto: AddPerformanceReview): Observable<string> {
    return this.http.post(`${this.apiUrl}/performance-reviews`, dto, { responseType: 'text' });
  }

  getPerformanceReviews(): Observable<ManagerPerformanceReview[]> {
    return this.http.get<ManagerPerformanceReview[]>(`${this.apiUrl}/performance-reviews`);
  }

  getEmployeePerformanceReviews(employeeId: string): Observable<ManagerPerformanceReview[]> {
    return this.http.get<ManagerPerformanceReview[]>(`${this.apiUrl}/performance-reviews/${employeeId}`);
  }

  addGoal(dto: AddGoal): Observable<string> {
    return this.http.post(`${this.apiUrl}/goals`, dto, { responseType: 'text' });
  }

  getGoals(query?: GoalQuery): Observable<ManagerGoal[]> {
    let params = new HttpParams();
    if (query?.employeeId) {
      params = params.set('employeeId', query.employeeId);
    }
    if (query?.status) {
      params = params.set('status', query.status);
    }
    return this.http.get<ManagerGoal[]>(`${this.apiUrl}/goals`, { params });
  }

  getEmployeeGoals(employeeId: string): Observable<ManagerGoal[]> {
    return this.http.get<ManagerGoal[]>(`${this.apiUrl}/goals/${employeeId}`);
  }

  updateGoalStatus(goalId: string, dto: UpdateGoalStatus): Observable<string> {
    return this.http.put(`${this.apiUrl}/goals/${goalId}`, dto, { responseType: 'text' });
  }

  getEligibleEmployees(search?: string, page: number = 1, pageSize: number = 10): Observable<{ employees: any[], totalCount: number }> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());
    if (search) {
      params = params.set('search', search);
    }
    return this.http.get<{ employees: any[], totalCount: number }>(`${this.apiUrl}/eligible-employees`, { params });
  }

  addTeamMember(employeeId: string): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/team`, { employeeId });
  }

  removeTeamMember(employeeId: string): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/team/${employeeId}`);
  }
}
