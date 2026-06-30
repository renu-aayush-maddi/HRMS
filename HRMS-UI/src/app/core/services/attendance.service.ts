import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { 
  PagedAttendanceResult, 
  AttendanceFilter, 
  PagedRegularizationResult, 
  AttendanceRegularizationFilter,
  ApproveAttendanceRegularization,
  RejectAttendanceRegularization
} from '../models/attendance.model';

@Injectable({
  providedIn: 'root'
})
export class AttendanceService {
  private baseAttendanceUrl = 'http://localhost:5237/api/attendance';
  private baseRegularizationUrl = 'http://localhost:5237/api/attendance-regularizations';

  constructor(private http: HttpClient) {}

  getAttendance(filter: AttendanceFilter): Observable<PagedAttendanceResult> {
    let params = new HttpParams();

    if (filter.employeeId) params = params.set('employeeId', filter.employeeId);
    if (filter.status) params = params.set('status', filter.status);
    if (filter.fromDate) params = params.set('fromDate', filter.fromDate);
    if (filter.toDate) params = params.set('toDate', filter.toDate);
    if (filter.pageNumber) params = params.set('pageNumber', filter.pageNumber.toString());
    if (filter.pageSize) params = params.set('pageSize', filter.pageSize.toString());
    if (filter.sortBy) params = params.set('sortBy', filter.sortBy);
    if (filter.descending !== undefined) params = params.set('descending', filter.descending.toString());

    return this.http.get<PagedAttendanceResult>(this.baseAttendanceUrl, { params });
  }

  exportAttendance(filter: AttendanceFilter): Observable<Blob> {
    let params = new HttpParams();

    if (filter.employeeId) params = params.set('employeeId', filter.employeeId);
    if (filter.status) params = params.set('status', filter.status);
    if (filter.fromDate) params = params.set('fromDate', filter.fromDate);
    if (filter.toDate) params = params.set('toDate', filter.toDate);
    if (filter.pageNumber) params = params.set('pageNumber', filter.pageNumber.toString());
    if (filter.pageSize) params = params.set('pageSize', filter.pageSize.toString());
    if (filter.sortBy) params = params.set('sortBy', filter.sortBy);
    if (filter.descending !== undefined) params = params.set('descending', filter.descending.toString());

    return this.http.get(`${this.baseAttendanceUrl}/export`, {
      params,
      responseType: 'blob'
    });
  }

  getRegularizationRequests(filter: AttendanceRegularizationFilter): Observable<PagedRegularizationResult> {
    let params = new HttpParams();

    if (filter.employeeId) params = params.set('employeeId', filter.employeeId);
    if (filter.status) params = params.set('status', filter.status);
    if (filter.fromDate) params = params.set('fromDate', filter.fromDate);
    if (filter.toDate) params = params.set('toDate', filter.toDate);
    if (filter.pageNumber) params = params.set('pageNumber', filter.pageNumber.toString());
    if (filter.pageSize) params = params.set('pageSize', filter.pageSize.toString());
    if (filter.sortBy) params = params.set('sortBy', filter.sortBy);
    if (filter.descending !== undefined) params = params.set('descending', filter.descending.toString());

    return this.http.get<PagedRegularizationResult>(this.baseRegularizationUrl, { params });
  }

  approveRegularization(id: string, request: ApproveAttendanceRegularization): Observable<any> {
    return this.http.put<any>(`${this.baseRegularizationUrl}/${id}/approve`, request);
  }

  rejectRegularization(id: string, request: RejectAttendanceRegularization): Observable<any> {
    return this.http.put<any>(`${this.baseRegularizationUrl}/${id}/reject`, request);
  }
}
