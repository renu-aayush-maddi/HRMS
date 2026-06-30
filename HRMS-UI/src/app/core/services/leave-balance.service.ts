import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { LeaveBalanceResponse, AllocateLeaveBalance } from '../models/leave-balance.model';

@Injectable({
  providedIn: 'root'
})
export class LeaveBalanceService {
  private apiUrl = 'http://localhost:5237/api/leave-balances';

  constructor(private http: HttpClient) {}

  getAllBalances(): Observable<LeaveBalanceResponse[]> {
    return this.http.get<LeaveBalanceResponse[]>(this.apiUrl);
  }

  getEmployeeBalances(employeeId: string): Observable<LeaveBalanceResponse[]> {
    return this.http.get<LeaveBalanceResponse[]>(`${this.apiUrl}/${employeeId}`);
  }

  allocate(dto: AllocateLeaveBalance): Observable<string> {
    return this.http.post(`${this.apiUrl}/allocate`, dto, { responseType: 'text' });
  }
}
