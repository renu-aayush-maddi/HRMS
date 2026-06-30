import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';

import { Department } from '../models/department.model';

@Injectable({
  providedIn: 'root'
})
export class DepartmentService {

  constructor(
    private http: HttpClient
  ) {}

  getDepartments(): Observable<Department[]> {

    return this.http.get<Department[]>(
      `${environment.apiUrl}/Departments`
    );
  }

  createDepartment(name: string): Observable<string> {
    return this.http.post(
      `${environment.apiUrl}/Departments`,
      { name },
      { responseType: 'text' }
    );
  }

  updateDepartment(id: string, name: string): Observable<string> {
    return this.http.put(
      `${environment.apiUrl}/Departments/${id}`,
      { name },
      { responseType: 'text' }
    );
  }

  deleteDepartment(id: string): Observable<string> {
    return this.http.delete(
      `${environment.apiUrl}/Departments/${id}`,
      { responseType: 'text' }
    );
  }
}