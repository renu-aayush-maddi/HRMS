import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Holiday, AddHoliday, UpdateHoliday } from '../models/holiday.model';

@Injectable({
  providedIn: 'root'
})
export class HolidayService {

  constructor(private http: HttpClient) {}

  getHolidays(): Observable<Holiday[]> {
    return this.http.get<Holiday[]>(`${environment.apiUrl}/holidays`);
  }

  getUpcomingHolidays(): Observable<Holiday[]> {
    return this.http.get<Holiday[]>(`${environment.apiUrl}/holidays/upcoming`);
  }

  createHoliday(dto: AddHoliday): Observable<string> {
    return this.http.post(`${environment.apiUrl}/holidays`, dto, { responseType: 'text' });
  }

  updateHoliday(id: string, dto: UpdateHoliday): Observable<string> {
    return this.http.put(`${environment.apiUrl}/holidays/${id}`, dto, { responseType: 'text' });
  }

  deleteHoliday(id: string): Observable<string> {
    return this.http.delete(`${environment.apiUrl}/holidays/${id}`, { responseType: 'text' });
  }
}
