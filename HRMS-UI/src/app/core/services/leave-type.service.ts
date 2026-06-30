import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { LeaveType, AddLeaveType, UpdateLeaveType } from '../models/leave-type.model';

@Injectable({
  providedIn: 'root'
})
export class LeaveTypeService {

  constructor(private http: HttpClient) {}

  getLeaveTypes(): Observable<LeaveType[]> {
    return this.http.get<LeaveType[]>(`${environment.apiUrl}/leave-types`);
  }

  createLeaveType(dto: AddLeaveType): Observable<string> {
    return this.http.post(`${environment.apiUrl}/leave-types`, dto, { responseType: 'text' });
  }

  updateLeaveType(id: string, dto: UpdateLeaveType): Observable<string> {
    return this.http.put(`${environment.apiUrl}/leave-types/${id}`, dto, { responseType: 'text' });
  }

  deleteLeaveType(id: string): Observable<string> {
    return this.http.delete(`${environment.apiUrl}/leave-types/${id}`, { responseType: 'text' });
  }
}
