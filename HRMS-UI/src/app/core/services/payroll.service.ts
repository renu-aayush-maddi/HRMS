import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { PayrollResponse, PayrollFilter, GeneratePayroll, GenerateMonthlyPayroll } from '../models/payroll.model';

@Injectable({
  providedIn: 'root'
})
export class PayrollService {

  constructor(private http: HttpClient) {}

  getPayrolls(filter?: PayrollFilter): Observable<PayrollResponse[]> {
    let params = new HttpParams();
    if (filter) {
      Object.entries(filter).forEach(([key, val]) => {
        if (val !== undefined && val !== null) {
          params = params.set(key, val.toString());
        }
      });
    }
    return this.http.get<PayrollResponse[]>(`${environment.apiUrl}/payroll`, { params });
  }

  getPayroll(id: string): Observable<PayrollResponse> {
    return this.http.get<PayrollResponse>(`${environment.apiUrl}/payroll/${id}`);
  }

  generatePayroll(dto: GeneratePayroll): Observable<any> {
    return this.http.post(`${environment.apiUrl}/payroll/generate`, dto);
  }

  generateMonthlyPayroll(dto: GenerateMonthlyPayroll): Observable<any> {
    return this.http.post(`${environment.apiUrl}/payroll/generate-monthly`, dto);
  }

  approvePayroll(id: string): Observable<any> {
    return this.http.put(`${environment.apiUrl}/payroll/${id}/approve`, {});
  }

  markPayrollPaid(id: string): Observable<any> {
    return this.http.put(`${environment.apiUrl}/payroll/${id}/mark-paid`, {});
  }

  exportPayrolls(filter?: PayrollFilter): Observable<Blob> {
    let params = new HttpParams();
    if (filter) {
      Object.entries(filter).forEach(([key, val]) => {
        if (val !== undefined && val !== null) {
          params = params.set(key, val.toString());
        }
      });
    }
    return this.http.get(`${environment.apiUrl}/payroll/export`, {
      params,
      responseType: 'blob'
    });
  }
}
