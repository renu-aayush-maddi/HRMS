import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { EmployeeDocument } from '../models/employee-subprofile.model';
import { PagedResponse } from '../models/paged-response.model';

@Injectable({
  providedIn: 'root'
})
export class EmployeeDocumentService {

  constructor(private http: HttpClient) {}

  getDocuments(
    employeeId: string,
    pageNumber: number,
    pageSize: number
  ): Observable<PagedResponse<EmployeeDocument>> {
    return this.http.get<PagedResponse<EmployeeDocument>>(
      `${environment.apiUrl}/employee-documents`,
      {
        params: {
          employeeId,
          pageNumber,
          pageSize
        }
      }
    );
  }

  uploadDocument(employeeId: string, documentType: string, file: File): Observable<EmployeeDocument> {
    const formData = new FormData();
    formData.append('employeeId', employeeId);
    formData.append('documentType', documentType);
    formData.append('file', file);
    return this.http.post<EmployeeDocument>(`${environment.apiUrl}/employee-documents`, formData);
  }

  verifyDocument(documentId: string): Observable<any> {
    return this.http.put(`${environment.apiUrl}/employee-documents/${documentId}/verify`, {});
  }

  downloadDocument(documentId: string): Observable<Blob> {
    return this.http.get(`${environment.apiUrl}/employee-documents/${documentId}/download`, {
      responseType: 'blob'
    });
  }

  deleteDocument(documentId: string): Observable<any> {
    return this.http.delete(`${environment.apiUrl}/employee-documents/${documentId}`);
  }

  exportDocuments(employeeId: string): Observable<Blob> {
    return this.http.get(`${environment.apiUrl}/employee-documents/export`, {
      params: { employeeId },
      responseType: 'blob'
    });
  }
}
