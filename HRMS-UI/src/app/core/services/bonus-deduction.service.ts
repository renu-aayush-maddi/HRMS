import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { 
  BonusResponse, CreateBonus, BonusFilter, PagedBonusResult,
  DeductionResponse, CreateDeduction, DeductionFilter, PagedDeductionResult
} from '../models/bonus-deduction.model';

@Injectable({
  providedIn: 'root'
})
export class BonusDeductionService {
  private bonusUrl = 'http://localhost:5237/api/bonuses';
  private deductionUrl = 'http://localhost:5237/api/deductions';

  constructor(private http: HttpClient) {}

  // Bonuses
  getBonuses(filter: BonusFilter): Observable<PagedBonusResult> {
    let params = new HttpParams();
    if (filter.employeeId) params = params.set('employeeId', filter.employeeId);
    if (filter.status) params = params.set('status', filter.status);
    if (filter.bonusMonth !== undefined) params = params.set('bonusMonth', filter.bonusMonth.toString());
    if (filter.bonusYear !== undefined) params = params.set('bonusYear', filter.bonusYear.toString());
    if (filter.minAmount !== undefined) params = params.set('minAmount', filter.minAmount.toString());
    if (filter.maxAmount !== undefined) params = params.set('maxAmount', filter.maxAmount.toString());
    if (filter.pageNumber !== undefined) params = params.set('pageNumber', filter.pageNumber.toString());
    if (filter.pageSize !== undefined) params = params.set('pageSize', filter.pageSize.toString());
    if (filter.sortBy) params = params.set('sortBy', filter.sortBy);
    params = params.set('descending', (filter.descending ?? false).toString());

    return this.http.get<PagedBonusResult>(this.bonusUrl, { params });
  }

  getBonus(id: string): Observable<BonusResponse> {
    return this.http.get<BonusResponse>(`${this.bonusUrl}/${id}`);
  }

  createBonus(dto: CreateBonus): Observable<BonusResponse> {
    return this.http.post<BonusResponse>(this.bonusUrl, dto);
  }

  approveBonus(id: string): Observable<{ message: string }> {
    return this.http.put<{ message: string }>(`${this.bonusUrl}/${id}/approve`, {});
  }

  rejectBonus(id: string): Observable<{ message: string }> {
    return this.http.put<{ message: string }>(`${this.bonusUrl}/${id}/reject`, {});
  }

  exportBonuses(filter: BonusFilter): Observable<Blob> {
    let params = new HttpParams();
    if (filter.employeeId) params = params.set('employeeId', filter.employeeId);
    if (filter.status) params = params.set('status', filter.status);
    if (filter.bonusMonth !== undefined) params = params.set('bonusMonth', filter.bonusMonth.toString());
    if (filter.bonusYear !== undefined) params = params.set('bonusYear', filter.bonusYear.toString());

    return this.http.get(`${this.bonusUrl}/export`, { params, responseType: 'blob' });
  }

  // Deductions
  getDeductions(filter: DeductionFilter): Observable<PagedDeductionResult> {
    let params = new HttpParams();
    if (filter.employeeId) params = params.set('employeeId', filter.employeeId);
    if (filter.status) params = params.set('status', filter.status);
    if (filter.deductionMonth !== undefined) params = params.set('deductionMonth', filter.deductionMonth.toString());
    if (filter.deductionYear !== undefined) params = params.set('deductionYear', filter.deductionYear.toString());
    if (filter.minAmount !== undefined) params = params.set('minAmount', filter.minAmount.toString());
    if (filter.maxAmount !== undefined) params = params.set('maxAmount', filter.maxAmount.toString());
    if (filter.pageNumber !== undefined) params = params.set('pageNumber', filter.pageNumber.toString());
    if (filter.pageSize !== undefined) params = params.set('pageSize', filter.pageSize.toString());
    if (filter.sortBy) params = params.set('sortBy', filter.sortBy);
    params = params.set('descending', (filter.descending ?? false).toString());

    return this.http.get<PagedDeductionResult>(this.deductionUrl, { params });
  }

  getDeduction(id: string): Observable<DeductionResponse> {
    return this.http.get<DeductionResponse>(`${this.deductionUrl}/${id}`);
  }

  createDeduction(dto: CreateDeduction): Observable<DeductionResponse> {
    return this.http.post<DeductionResponse>(this.deductionUrl, dto);
  }

  approveDeduction(id: string): Observable<{ message: string }> {
    return this.http.put<{ message: string }>(`${this.deductionUrl}/${id}/approve`, {});
  }

  rejectDeduction(id: string): Observable<{ message: string }> {
    return this.http.put<{ message: string }>(`${this.deductionUrl}/${id}/reject`, {});
  }

  exportDeductions(filter: DeductionFilter): Observable<Blob> {
    let params = new HttpParams();
    if (filter.employeeId) params = params.set('employeeId', filter.employeeId);
    if (filter.status) params = params.set('status', filter.status);
    if (filter.deductionMonth !== undefined) params = params.set('deductionMonth', filter.deductionMonth.toString());
    if (filter.deductionYear !== undefined) params = params.set('deductionYear', filter.deductionYear.toString());

    return this.http.get(`${this.deductionUrl}/export`, { params, responseType: 'blob' });
  }
}
