import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Review, ReviewFilter } from '../models/review.model';

@Injectable({
  providedIn: 'root'
})
export class ReviewService {

  constructor(private http: HttpClient) {}

  getReviews(filter?: ReviewFilter): Observable<Review[]> {
    let params = new HttpParams();
    if (filter) {
      Object.entries(filter).forEach(([key, val]) => {
        if (val !== undefined && val !== null) {
          params = params.set(key, val.toString());
        }
      });
    }
    return this.http.get<Review[]>(`${environment.apiUrl}/reviews`, { params });
  }

  deleteReview(id: string): Observable<any> {
    return this.http.delete(`${environment.apiUrl}/reviews/${id}`);
  }

  exportReviews(filter?: ReviewFilter): Observable<Blob> {
    let params = new HttpParams();
    if (filter) {
      Object.entries(filter).forEach(([key, val]) => {
        if (val !== undefined && val !== null) {
          params = params.set(key, val.toString());
        }
      });
    }
    return this.http.get(`${environment.apiUrl}/reviews/export`, {
      params,
      responseType: 'blob'
    });
  }
}
