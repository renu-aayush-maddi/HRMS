import { Component, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { EmployeeSelfService } from '../../../core/services/employee-self.service';
import { Review } from '../../../core/models/review.model';

import {
  LucideAlertTriangle,
  LucideStar
} from '@lucide/angular';

@Component({
  selector: 'app-emp-reviews',
  standalone: true,
  imports: [
    CommonModule,
    LucideAlertTriangle,
    LucideStar
  ],
  templateUrl: './emp-reviews.html',
  styleUrl: './emp-reviews.css',
})
export class EmpReviews implements OnInit {
  readonly reviews = signal<Review[]>([]);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  readonly stars = [1, 2, 3, 4, 5];

  readonly avgRating = computed(() => {
    const list = this.reviews();
    if (!list.length) return 0;
    const rated = list.filter(r => r.rating);
    if (!rated.length) return 0;
    return Math.round((rated.reduce((s, r) => s + (r.rating ?? 0), 0) / rated.length) * 10) / 10;
  });

  constructor(private empService: EmployeeSelfService) {}

  ngOnInit(): void {
    this.loadReviews();
  }

  loadReviews(): void {
    this.loading.set(true);
    this.empService.getMyReviews().subscribe({
      next: (data: any) => {
        const items = data?.data || (Array.isArray(data) ? data : []);
        this.reviews.set(items);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Failed to load performance reviews.');
        this.loading.set(false);
      }
    });
  }

  getStarClass(star: number, rating?: number): string {
    return star <= (rating ?? 0) ? 'star-filled' : 'star-empty';
  }

  getRatingLabel(rating?: number): string {
    const labels = ['', 'Poor', 'Below Average', 'Average', 'Good', 'Excellent'];
    return labels[rating ?? 0] || '';
  }
}
