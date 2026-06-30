import { Injectable } from '@angular/core';

import { jwtDecode } from 'jwt-decode';

@Injectable({
  providedIn: 'root'
})
export class JwtService {

  decode(token: string): any {
    return jwtDecode(token);
  }

  isExpired(token: string): boolean {

    const decoded: any =
      jwtDecode(token);

    const currentTime =
      Date.now() / 1000;

    return decoded.exp < currentTime;
  }
}