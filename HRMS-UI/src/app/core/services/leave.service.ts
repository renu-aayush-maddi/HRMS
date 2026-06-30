import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PagedLeaveResult, LeaveFilter, LeaveAction } from '../models/leave.model';

@Injectable({
  providedIn: 'root'
})
export class LeaveService {
  private apiUrl = 'http://localhost:5237/api/leave';

  constructor(private http: HttpClient) {}

  getLeaves(filter: LeaveFilter): Observable<PagedLeaveResult> {
    let params = new HttpParams();

    if (filter.employeeId) params = params.set('employeeId', filter.employeeId);
    if (filter.leaveTypeId) params = params.set('leaveTypeId', filter.leaveTypeId);
    if (filter.status) params = params.set('status', filter.status);
    if (filter.fromDate) params = params.set('fromDate', filter.fromDate);
    if (filter.toDate) params = params.set('toDate', filter.toDate);
    if (filter.pageNumber) params = params.set('pageNumber', filter.pageNumber.toString());
    if (filter.pageSize) params = params.set('pageSize', filter.pageSize.toString());
    if (filter.sortBy) params = params.set('sortBy', filter.sortBy);
    if (filter.descending !== undefined) params = params.set('descending', filter.descending.toString());

    return this.http.get<PagedLeaveResult>(this.apiUrl, { params });
  }

  approveLeave(leaveId: string, action: LeaveAction): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/${leaveId}/approve`, action);
  }

  rejectLeave(leaveId: string, action: LeaveAction): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/${leaveId}/reject`, action);
  }

  cancelLeave(leaveId: string): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/${leaveId}/cancel`, {});
  }
}
