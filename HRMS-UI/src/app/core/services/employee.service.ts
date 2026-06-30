import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';

import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';

import { Employee } from '../models/employee.model';
import { PagedResponse } from '../models/paged-response.model';

import { AddEmployee } from '../models/add-employee.model';
import { EmployeeCreated } from '../models/employee-created.model';
import { Manager } from '../models/manager.model';
import { EmployeeProfile } from '../models/employee-profile.model';
import { UpdateEmployee } from '../models/update-employee.model';

@Injectable({
  providedIn: 'root'
})
export class EmployeeService {
  

  constructor(
    private http: HttpClient
  ) {}

  getEmployees(
    pageNumber: number,
    pageSize: number,
    search: string = ''
  ): Observable<PagedResponse<Employee>> {

    const params = new HttpParams()
      .set('pageNumber', pageNumber)
      .set('pageSize', pageSize)
      .set('search', search);

    return this.http.get<PagedResponse<Employee>>(
      `${environment.apiUrl}/Employee`,
      { params }
    );
  }


      createEmployee(request: AddEmployee) {

      return this.http.post<EmployeeCreated>(
        `${environment.apiUrl}/Employee`,
        request
      );
    }

      getManagers() {

    return this.http.get<Manager[]>(
      `${environment.apiUrl}/Employee/managers`
    );
  }

    getEmployeeProfile(employeeId: string) {

    return this.http.get<EmployeeProfile>(
      `${environment.apiUrl}/Employee/${employeeId}/profile`
    );
  }


    updateEmployee(employeeId: string,request:UpdateEmployee) {

    return this.http.put(
      `${environment.apiUrl}/Employee/${employeeId}`,
      request
    );
  }

updateEmployeeStatus(
  employeeId: string,
  status: string
) {
  return this.http.put(
    `${environment.apiUrl}/Employee/${employeeId}/status`,
    { status },
    {
      responseType: 'text'
    }
  );
}

exportEmployees() {
  return this.http.get(`${environment.apiUrl}/Employee/export`, {
    responseType: 'blob'
  });
}

importEmployees(file: File) {
  const formData = new FormData();
  formData.append('file', file);

  return this.http.post(`${environment.apiUrl}/Employee/import`, formData);
}

getEmployeeLookup() {

  return this.http.get<any>(
    `${environment.apiUrl}/Employee?pageNumber=1&pageSize=500`
  );

}

}