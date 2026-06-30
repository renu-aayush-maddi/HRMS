import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface EmployeeNode {
  id: string;
  managerId: string | null;
  employeeCode: string;
  firstName: string;
  lastName: string;
  name: string;
  email: string;
  designation: string | null;
  departmentId: string | null;
  departmentName: string | null;
  employmentStatus: string | null;
  profilePhotoUrl: string | null;
  directReportsCount: number;
  warning: string | null;
  children: EmployeeNode[];
}

export interface ManagerInfo {
  id: string;
  employeeCode: string;
  name: string;
  designation: string | null;
  department: string | null;
  profilePhotoUrl: string | null;
  employmentStatus: string | null;
}

@Injectable({
  providedIn: 'root'
})
export class HierarchyService {
  private api = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getTree(rootId?: string, scope?: string): Observable<EmployeeNode[]> {
    let params = new HttpParams();
    if (rootId) {
      params = params.set('rootId', rootId);
    }
    if (scope) {
      params = params.set('scope', scope);
    }
    return this.http.get<EmployeeNode[]>(`${this.api}/hierarchy/tree`, { params });
  }

  getReportingChain(employeeId: string): Observable<EmployeeNode[]> {
    return this.http.get<EmployeeNode[]>(`${this.api}/hierarchy/reporting-chain/${employeeId}`);
  }

  getDirectReports(employeeId: string): Observable<EmployeeNode[]> {
    return this.http.get<EmployeeNode[]>(`${this.api}/hierarchy/direct-reports/${employeeId}`);
  }

  getManagerInfo(employeeId: string): Observable<ManagerInfo> {
    return this.http.get<ManagerInfo>(`${this.api}/hierarchy/manager/${employeeId}`);
  }

  searchEmployees(query: string): Observable<EmployeeNode[]> {
    const params = new HttpParams().set('query', query);
    return this.http.get<EmployeeNode[]>(`${this.api}/hierarchy/search`, { params });
  }
}
