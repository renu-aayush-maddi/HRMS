import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { EmployeeExperience, AddEmployeeExperience, UpdateEmployeeExperience } from '../models/employee-subprofile.model';
import { PagedResponse } from '../models/paged-response.model';

@Injectable({
  providedIn: 'root'
})
export class EmployeeExperienceService {

  constructor(private http: HttpClient) {}

  getExperiences(
    employeeId: string,
    pageNumber: number,
    pageSize: number
  ): Observable<PagedResponse<EmployeeExperience>> {
    return this.http.get<PagedResponse<EmployeeExperience>>(
      `${environment.apiUrl}/employee-experiences`,
      {
        params: {
          employeeId,
          pageNumber,
          pageSize
        }
      }
    );
  }

  createExperience(request: AddEmployeeExperience): Observable<EmployeeExperience> {
    return this.http.post<EmployeeExperience>(
      `${environment.apiUrl}/employee-experiences`,
      request
    );
  }

  updateExperience(id: string, request: UpdateEmployeeExperience): Observable<EmployeeExperience> {
    return this.http.put<EmployeeExperience>(
      `${environment.apiUrl}/employee-experiences/${id}`,
      request
    );
  }

  deleteExperience(id: string): Observable<any> {
    return this.http.delete(`${environment.apiUrl}/employee-experiences/${id}`);
  }

  exportExperiences(employeeId: string): Observable<Blob> {
    return this.http.get(`${environment.apiUrl}/employee-experiences/export`, {
      params: { employeeId },
      responseType: 'blob'
    });
  }

  importExperiences(file: File): Observable<any> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post(`${environment.apiUrl}/employee-experiences/import`, formData);
  }
}
