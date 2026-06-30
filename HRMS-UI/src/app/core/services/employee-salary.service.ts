import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { EmployeeSalaryResponse, AssignEmployeeSalary, SalaryHistoryResponse } from '../models/employee-salary.model';

@Injectable({
  providedIn: 'root'
})
export class EmployeeSalaryService {
  private apiUrl = 'http://localhost:5237/api/employee-salaries';

  constructor(private http: HttpClient) {}

  getAll(): Observable<EmployeeSalaryResponse[]> {
    return this.http.get<EmployeeSalaryResponse[]>(this.apiUrl);
  }

  assignSalary(dto: AssignEmployeeSalary): Observable<string> {
    return this.http.post(this.apiUrl, dto, { responseType: 'text' });
  }

  getActiveSalary(employeeId: string): Observable<EmployeeSalaryResponse> {
    return this.http.get<EmployeeSalaryResponse>(`${this.apiUrl}/${employeeId}`);
  }

  getSalaryHistory(employeeId: string): Observable<SalaryHistoryResponse[]> {
    return this.http.get<SalaryHistoryResponse[]>(`${this.apiUrl}/history/${employeeId}`);
  }
}
