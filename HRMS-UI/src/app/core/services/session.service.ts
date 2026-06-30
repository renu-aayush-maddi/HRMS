import { Injectable } from '@angular/core';

import { AuthStore } from '../../stores/auth/auth.store';
import { JwtService } from './jwt.service';

@Injectable({
  providedIn: 'root'
})
export class SessionService {

  constructor(
    private authStore: AuthStore,
    private jwtService: JwtService
  ) {}

  restoreSession(): void {

    const token = localStorage.getItem('token');

    if (!token) {
      return;
    }

    if (this.jwtService.isExpired(token)) {
      localStorage.removeItem('token');
      return;
    }

    const payload: any = this.jwtService.decode(token);

    this.authStore.setToken(token);

    this.authStore.setCurrentUser({
      userId:
        payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'],

      email:
        payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'],

      role:
        payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'],

      employeeId:
        payload['EmployeeId']
    });
  }
}