import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { 
  PerformanceBonusRuleResponse, AddPerformanceBonusRule, UpdatePerformanceBonusRule,
  PerformanceBonusRecommendationResponse, UpdatePerformanceBonusRecommendation, HrDashboardResponse
} from '../models/performance-bonus.model';

@Injectable({
  providedIn: 'root'
})
export class PerformanceBonusService {
  private rulesUrl = 'http://localhost:5237/api/performance-bonus-rules';
  private recsUrl = 'http://localhost:5237/api/performance-bonus-recommendations';
  private dashUrl = 'http://localhost:5237/api/performance/dashboard';

  constructor(private http: HttpClient) {}

  // Rules
  getRules(): Observable<PerformanceBonusRuleResponse[]> {
    return this.http.get<PerformanceBonusRuleResponse[]>(this.rulesUrl);
  }

  getRule(id: string): Observable<PerformanceBonusRuleResponse> {
    return this.http.get<PerformanceBonusRuleResponse>(`${this.rulesUrl}/${id}`);
  }

  addRule(dto: AddPerformanceBonusRule): Observable<string> {
    return this.http.post(this.rulesUrl, dto, { responseType: 'text' });
  }

  updateRule(id: string, dto: UpdatePerformanceBonusRule): Observable<string> {
    return this.http.put(`${this.rulesUrl}/${id}`, dto, { responseType: 'text' });
  }

  deleteRule(id: string): Observable<string> {
    return this.http.delete(`${this.rulesUrl}/${id}`, { responseType: 'text' });
  }

  // Recommendations
  generateRecommendations(cycleId: string): Observable<string> {
    return this.http.post(`${this.recsUrl}/generate/${cycleId}`, {}, { responseType: 'text' });
  }

  getRecommendations(): Observable<PerformanceBonusRecommendationResponse[]> {
    return this.http.get<PerformanceBonusRecommendationResponse[]>(this.recsUrl);
  }

  getRecommendation(id: string): Observable<PerformanceBonusRecommendationResponse> {
    return this.http.get<PerformanceBonusRecommendationResponse>(`${this.recsUrl}/${id}`);
  }

  updateRecommendation(id: string, dto: UpdatePerformanceBonusRecommendation): Observable<string> {
    return this.http.put(`${this.recsUrl}/${id}`, dto, { responseType: 'text' });
  }

  approveRecommendation(id: string): Observable<string> {
    return this.http.post(`${this.recsUrl}/${id}/approve`, {}, { responseType: 'text' });
  }

  rejectRecommendation(id: string): Observable<string> {
    return this.http.post(`${this.recsUrl}/${id}/reject`, {}, { responseType: 'text' });
  }

  // Dashboard
  getHrDashboard(cycleId: string): Observable<HrDashboardResponse> {
    const params = new HttpParams().set('cycleId', cycleId);
    return this.http.get<HrDashboardResponse>(`${this.dashUrl}/hr`, { params });
  }
}
