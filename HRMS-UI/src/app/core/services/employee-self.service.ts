import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AttendanceResponse, AttendanceFilter, PagedAttendanceResult, AttendanceRegularizationResponse, AttendanceRegularizationFilter, PagedRegularizationResult } from '../models/attendance.model';
import { LeaveResponse, LeaveFilter, PagedLeaveResult } from '../models/leave.model';
import { PayrollResponse, PayrollFilter } from '../models/payroll.model';
import { BonusResponse, BonusFilter, PagedBonusResult, DeductionResponse, DeductionFilter, PagedDeductionResult } from '../models/bonus-deduction.model';
import { ManagerGoal, UpdateGoalStatus } from '../models/manager.model';
import { Review, ReviewFilter } from '../models/review.model';
import { EmployeeResignation } from '../models/resignation.model';
import { EmployeeProfile } from '../models/employee-profile.model';

export interface CreateRegularizationDto {
  attendanceDate: string;
  requestedCheckIn?: string;
  requestedCheckOut?: string;
  reason: string;
}

export interface ApplyLeaveDto {
  leaveTypeId: string;
  fromDate: string;
  toDate: string;
  reason?: string;
}

export interface CreateResignationDto {
  resignationDate: string;
  lastWorkingDate?: string;
  reason?: string;
}

export interface LeaveBalanceItem {
  id: string;
  leaveType: string;
  allocatedDays: number;
  usedDays: number;
  remainingDays: number;
}

@Injectable({
  providedIn: 'root'
})
export class EmployeeSelfService {
  private api = environment.apiUrl;

  constructor(private http: HttpClient) {}

  // ─── ATTENDANCE ─────────────────────────────────────────────────────────────

  checkIn(): Observable<string> {
    return this.http.post(`${this.api}/attendance/check-in`, {}, { responseType: 'text' });
  }

  checkOut(): Observable<string> {
    return this.http.post(`${this.api}/attendance/check-out`, {}, { responseType: 'text' });
  }

  getMyAttendance(filter?: AttendanceFilter): Observable<PagedAttendanceResult> {
    let params = new HttpParams();
    if (filter) {
      if (filter.status) params = params.set('status', filter.status);
      if (filter.fromDate) params = params.set('fromDate', filter.fromDate);
      if (filter.toDate) params = params.set('toDate', filter.toDate);
      if (filter.pageNumber) params = params.set('pageNumber', filter.pageNumber.toString());
      if (filter.pageSize) params = params.set('pageSize', filter.pageSize.toString());
      if (filter.sortBy) params = params.set('sortBy', filter.sortBy);
      if (filter.descending !== undefined) params = params.set('descending', filter.descending.toString());
    }
    return this.http.get<PagedAttendanceResult>(`${this.api}/attendance/employee`, { params });
  }

  // ─── REGULARIZATIONS ────────────────────────────────────────────────────────

  createRegularization(dto: CreateRegularizationDto): Observable<AttendanceRegularizationResponse> {
    return this.http.post<AttendanceRegularizationResponse>(`${this.api}/attendance-regularizations`, dto);
  }

  getMyRegularizations(filter?: AttendanceRegularizationFilter): Observable<PagedRegularizationResult> {
    let params = new HttpParams();
    if (filter) {
      if (filter.status) params = params.set('status', filter.status);
      if (filter.fromDate) params = params.set('fromDate', filter.fromDate);
      if (filter.toDate) params = params.set('toDate', filter.toDate);
      if (filter.pageNumber) params = params.set('pageNumber', filter.pageNumber.toString());
      if (filter.pageSize) params = params.set('pageSize', filter.pageSize.toString());
    }
    return this.http.get<PagedRegularizationResult>(`${this.api}/attendance-regularizations`, { params });
  }

  // ─── LEAVE ──────────────────────────────────────────────────────────────────

  applyLeave(dto: ApplyLeaveDto): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.api}/leave/apply`, dto);
  }

  getMyLeaves(filter?: LeaveFilter): Observable<PagedLeaveResult> {
    let params = new HttpParams();
    if (filter) {
      if (filter.status) params = params.set('status', filter.status);
      if (filter.fromDate) params = params.set('fromDate', filter.fromDate);
      if (filter.toDate) params = params.set('toDate', filter.toDate);
      if (filter.pageNumber) params = params.set('pageNumber', filter.pageNumber.toString());
      if (filter.pageSize) params = params.set('pageSize', filter.pageSize.toString());
    }
    return this.http.get<PagedLeaveResult>(`${this.api}/leave/my-leaves`, { params });
  }

  withdrawLeave(leaveId: string): Observable<{ message: string }> {
    return this.http.put<{ message: string }>(`${this.api}/leave/${leaveId}/withdraw`, {});
  }

  getLeaveBalances(): Observable<LeaveBalanceItem[]> {
    return this.http.get<LeaveBalanceItem[]>(`${this.api}/leave/balance`);
  }

  // ─── PAYROLL & PAYSLIP ──────────────────────────────────────────────────────

  getMyPayrolls(filter?: PayrollFilter): Observable<PayrollResponse[]> {
    let params = new HttpParams();
    if (filter) {
      if (filter.payMonth) params = params.set('payMonth', filter.payMonth.toString());
      if (filter.payYear) params = params.set('payYear', filter.payYear.toString());
      if (filter.status) params = params.set('status', filter.status);
    }
    return this.http.get<PayrollResponse[]>(`${this.api}/payroll/my`, { params });
  }

  downloadMyPayslip(payrollId: string): Observable<Blob> {
    return this.http.get(`${this.api}/payslips/my/${payrollId}`, { responseType: 'blob' });
  }

  // ─── BONUSES & DEDUCTIONS ───────────────────────────────────────────────────

  getMyBonuses(filter?: BonusFilter): Observable<PagedBonusResult> {
    let params = new HttpParams();
    if (filter) {
      if (filter.bonusMonth) params = params.set('bonusMonth', filter.bonusMonth.toString());
      if (filter.bonusYear) params = params.set('bonusYear', filter.bonusYear.toString());
      if (filter.pageNumber) params = params.set('pageNumber', filter.pageNumber.toString());
      if (filter.pageSize) params = params.set('pageSize', filter.pageSize.toString());
    }
    return this.http.get<PagedBonusResult>(`${this.api}/bonuses/my`, { params });
  }

  getMyDeductions(filter?: DeductionFilter): Observable<PagedDeductionResult> {
    let params = new HttpParams();
    if (filter) {
      if (filter.deductionMonth) params = params.set('deductionMonth', filter.deductionMonth.toString());
      if (filter.deductionYear) params = params.set('deductionYear', filter.deductionYear.toString());
      if (filter.pageNumber) params = params.set('pageNumber', filter.pageNumber.toString());
      if (filter.pageSize) params = params.set('pageSize', filter.pageSize.toString());
    }
    return this.http.get<PagedDeductionResult>(`${this.api}/deductions/my`, { params });
  }

  // ─── GOALS ──────────────────────────────────────────────────────────────────

  getMyGoals(): Observable<ManagerGoal[]> {
    return this.http.get<ManagerGoal[]>(`${this.api}/employee/goals`);
  }

  updateMyGoalStatus(goalId: string, dto: UpdateGoalStatus): Observable<string> {
    return this.http.put(`${this.api}/employee/goals/${goalId}`, dto, { responseType: 'text' });
  }

  // ─── REVIEWS ────────────────────────────────────────────────────────────────

  getMyReviews(filter?: ReviewFilter): Observable<Review[]> {
    let params = new HttpParams();
    if (filter) {
      if (filter.performanceCycleId) params = params.set('performanceCycleId', filter.performanceCycleId);
      if (filter.search) params = params.set('search', filter.search);
    }
    return this.http.get<Review[]>(`${this.api}/reviews/my`, { params });
  }

  // ─── RESIGNATION ────────────────────────────────────────────────────────────

  submitResignation(dto: CreateResignationDto): Observable<EmployeeResignation> {
    return this.http.post<EmployeeResignation>(`${this.api}/employee-resignations`, dto);
  }

  getMyResignations(): Observable<EmployeeResignation[]> {
    return this.http.get<EmployeeResignation[]>(`${this.api}/employee-resignations`);
  }

  withdrawResignation(id: string): Observable<void> {
    return this.http.put<void>(`${this.api}/employee-resignations/${id}/withdraw`, {});
  }

  // ─── PROFILE ────────────────────────────────────────────────────────────────

  getMyProfile(): Observable<EmployeeProfile> {
    return this.http.get<EmployeeProfile>(`${this.api}/Employee/my-profile`);
  }

  updateMyProfile(dto: { phone?: string }): Observable<any> {
    return this.http.put(`${this.api}/Employee/my-profile`, dto);
  }

  uploadProfilePhoto(file: File): Observable<{ profilePhotoUrl: string, message?: string }> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<{ profilePhotoUrl: string, message?: string }>(`${this.api}/Employee/my-profile/photo`, formData);
  }

  deleteProfilePhoto(): Observable<any> {
    return this.http.delete(`${this.api}/Employee/my-profile/photo`);
  }
}
