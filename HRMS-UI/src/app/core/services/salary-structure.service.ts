import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { SalaryStructureResponse, CreateSalaryStructure, UpdateSalaryStructure } from '../models/salary-structure.model';

@Injectable({
  providedIn: 'root'
})
export class SalaryStructureService {
  private apiUrl = 'http://localhost:5237/api/salary-structures';

  constructor(private http: HttpClient) {}

  getAll(): Observable<SalaryStructureResponse[]> {
    return this.http.get<SalaryStructureResponse[]>(this.apiUrl);
  }

  getById(id: string): Observable<SalaryStructureResponse> {
    return this.http.get<SalaryStructureResponse>(`${this.apiUrl}/${id}`);
  }

  create(dto: CreateSalaryStructure): Observable<string> {
    return this.http.post(this.apiUrl, dto, { responseType: 'text' });
  }

  update(id: string, dto: UpdateSalaryStructure): Observable<string> {
    return this.http.put(`${this.apiUrl}/${id}`, dto, { responseType: 'text' });
  }

  delete(id: string): Observable<string> {
    return this.http.delete(`${this.apiUrl}/${id}`, { responseType: 'text' });
  }
}
