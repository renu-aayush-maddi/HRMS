import { Injectable } from '@angular/core';

import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';

import { EmployeeAddress } from '../models/employee-address.model';

import { AddEmployeeAddress } from '../models/add-employee-address.model';

import { UpdateEmployeeAddress } from '../models/update-employee-address.model';

import { PagedResponse } from '../models/paged-response.model';

@Injectable({
  providedIn: 'root'
})
export class EmployeeAddressService {

  constructor(
    private http: HttpClient
  ) {}

  getAddresses(
    employeeId: string,
    pageNumber: number,
    pageSize: number
  ): Observable<PagedResponse<EmployeeAddress>> {

    return this.http.get<PagedResponse<EmployeeAddress>>(
      `${environment.apiUrl}/employee-addresses`,
      {
        params: {
          employeeId,
          pageNumber,
          pageSize
        }
      }
    );
  }

  createAddress(
    request: AddEmployeeAddress
  ): Observable<EmployeeAddress> {

    return this.http.post<EmployeeAddress>(
      `${environment.apiUrl}/employee-addresses`,
      request
    );
  }

  updateAddress(
    id: string,
    request: UpdateEmployeeAddress
  ): Observable<EmployeeAddress> {

    return this.http.put<EmployeeAddress>(
      `${environment.apiUrl}/employee-addresses/${id}`,
      request
    );
  }

  deleteAddress(
    id: string
  ): Observable<any> {

    return this.http.delete(
      `${environment.apiUrl}/employee-addresses/${id}`
    );
  }

  exportAddresses(
    employeeId: string
  ): Observable<Blob> {

    return this.http.get(
      `${environment.apiUrl}/employee-addresses/export`,
      {
        params: {
          employeeId
        },
        responseType: 'blob'
      }
    );
  }

  importAddresses(
    file: File
  ): Observable<any> {

    const formData = new FormData();

    formData.append(
      'file',
      file
    );

    return this.http.post(
      `${environment.apiUrl}/employee-addresses/import`,
      formData
    );
  }

}