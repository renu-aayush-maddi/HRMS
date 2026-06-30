import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { PerformanceCycle, AddPerformanceCycle, UpdatePerformanceCycle } from '../models/performance-cycle.model';

@Injectable({
  providedIn: 'root'
})
export class PerformanceCycleService {

  constructor(private http: HttpClient) {}

  getCycles(): Observable<PerformanceCycle[]> {
    return this.http.get<PerformanceCycle[]>(`${environment.apiUrl}/performance-cycles`);
  }

  getCycle(id: string): Observable<PerformanceCycle> {
    return this.http.get<PerformanceCycle>(`${environment.apiUrl}/performance-cycles/${id}`);
  }

  createCycle(dto: AddPerformanceCycle): Observable<string> {
    return this.http.post(`${environment.apiUrl}/performance-cycles`, dto, { responseType: 'text' });
  }

  updateCycle(id: string, dto: UpdatePerformanceCycle): Observable<string> {
    return this.http.put(`${environment.apiUrl}/performance-cycles/${id}`, dto, { responseType: 'text' });
  }

  deleteCycle(id: string): Observable<string> {
    return this.http.delete(`${environment.apiUrl}/performance-cycles/${id}`, { responseType: 'text' });
  }
}
