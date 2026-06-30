import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { EmployeeEmergencyContact, AddEmployeeEmergencyContact, UpdateEmployeeEmergencyContact } from '../models/employee-subprofile.model';
import { PagedResponse } from '../models/paged-response.model';

@Injectable({
  providedIn: 'root'
})
export class EmployeeEmergencyContactService {

  constructor(private http: HttpClient) {}

  getEmergencyContacts(
    employeeId: string,
    pageNumber: number,
    pageSize: number
  ): Observable<PagedResponse<EmployeeEmergencyContact>> {
    return this.http.get<PagedResponse<EmployeeEmergencyContact>>(
      `${environment.apiUrl}/employee-emergency-contacts`,
      {
        params: {
          employeeId,
          pageNumber,
          pageSize
        }
      }
    );
  }

  createEmergencyContact(request: AddEmployeeEmergencyContact): Observable<EmployeeEmergencyContact> {
    return this.http.post<EmployeeEmergencyContact>(
      `${environment.apiUrl}/employee-emergency-contacts`,
      request
    );
  }

  updateEmergencyContact(id: string, request: UpdateEmployeeEmergencyContact): Observable<EmployeeEmergencyContact> {
    return this.http.put<EmployeeEmergencyContact>(
      `${environment.apiUrl}/employee-emergency-contacts/${id}`,
      request
    );
  }

  deleteEmergencyContact(id: string): Observable<any> {
    return this.http.delete(`${environment.apiUrl}/employee-emergency-contacts/${id}`);
  }

  exportEmergencyContacts(employeeId: string): Observable<Blob> {
    return this.http.get(`${environment.apiUrl}/employee-emergency-contacts/export`, {
      params: { employeeId },
      responseType: 'blob'
    });
  }

  importEmergencyContacts(file: File): Observable<any> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post(`${environment.apiUrl}/employee-emergency-contacts/import`, formData);
  }
}
