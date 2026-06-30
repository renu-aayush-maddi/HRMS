import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { EmployeeResignation, ResignationFilter, RejectResignation, UpdateSettlementStatus } from '../models/resignation.model';

@Injectable({
  providedIn: 'root'
})
export class ResignationService {

  constructor(private http: HttpClient) {}

  getResignations(filter?: ResignationFilter): Observable<EmployeeResignation[]> {
    let params = new HttpParams();
    if (filter) {
      Object.entries(filter).forEach(([key, val]) => {
        if (val !== undefined && val !== null) {
          params = params.set(key, val.toString());
        }
      });
    }
    return this.http.get<EmployeeResignation[]>(`${environment.apiUrl}/employee-resignations`, { params });
  }

  getResignation(id: string): Observable<EmployeeResignation> {
    return this.http.get<EmployeeResignation>(`${environment.apiUrl}/employee-resignations/${id}`);
  }

  approveResignation(id: string): Observable<any> {
    return this.http.put(`${environment.apiUrl}/employee-resignations/${id}/approve`, {});
  }

  rejectResignation(id: string, dto: RejectResignation): Observable<any> {
    return this.http.put(`${environment.apiUrl}/employee-resignations/${id}/reject`, dto);
  }

  updateSettlementStatus(id: string, dto: UpdateSettlementStatus): Observable<any> {
    return this.http.put(`${environment.apiUrl}/employee-resignations/${id}/settlement-status`, dto);
  }

  exportResignations(filter?: ResignationFilter): Observable<Blob> {
    let params = new HttpParams();
    if (filter) {
      Object.entries(filter).forEach(([key, val]) => {
        if (val !== undefined && val !== null) {
          params = params.set(key, val.toString());
        }
      });
    }
    return this.http.get(`${environment.apiUrl}/employee-resignations/export`, {
      params,
      responseType: 'blob'
    });
  }
}
