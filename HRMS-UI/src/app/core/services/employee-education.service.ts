import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { EmployeeEducation, AddEmployeeEducation, UpdateEmployeeEducation } from '../models/employee-subprofile.model';
import { PagedResponse } from '../models/paged-response.model';

@Injectable({
  providedIn: 'root'
})
export class EmployeeEducationService {

  constructor(private http: HttpClient) {}

  getEducations(
    employeeId: string,
    pageNumber: number,
    pageSize: number
  ): Observable<PagedResponse<EmployeeEducation>> {
    return this.http.get<PagedResponse<EmployeeEducation>>(
      `${environment.apiUrl}/employee-educations`,
      {
        params: {
          employeeId,
          pageNumber,
          pageSize
        }
      }
    );
  }

  createEducation(request: AddEmployeeEducation): Observable<EmployeeEducation> {
    return this.http.post<EmployeeEducation>(
      `${environment.apiUrl}/employee-educations`,
      request
    );
  }

  updateEducation(id: string, request: UpdateEmployeeEducation): Observable<EmployeeEducation> {
    return this.http.put<EmployeeEducation>(
      `${environment.apiUrl}/employee-educations/${id}`,
      request
    );
  }

  deleteEducation(id: string): Observable<any> {
    return this.http.delete(`${environment.apiUrl}/employee-educations/${id}`);
  }

  exportEducations(employeeId: string): Observable<Blob> {
    return this.http.get(`${environment.apiUrl}/employee-educations/export`, {
      params: { employeeId },
      responseType: 'blob'
    });
  }

  importEducations(file: File): Observable<any> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post(`${environment.apiUrl}/employee-educations/import`, formData);
  }
}
