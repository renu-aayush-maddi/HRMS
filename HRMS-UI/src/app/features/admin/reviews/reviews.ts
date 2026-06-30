import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ReviewService } from '../../../core/services/review.service';
import { PerformanceCycleService } from '../../../core/services/performance-cycle.service';
import { Review, ReviewFilter } from '../../../core/models/review.model';
import { PerformanceCycle } from '../../../core/models/performance-cycle.model';
import { ToastrService } from 'ngx-toastr';

import {
  LucideDownload,
  LucideStar,
  LucideTrash
} from '@lucide/angular';

@Component({
  selector: 'app-reviews',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    LucideDownload,
    LucideStar,
    LucideTrash
  ],
  templateUrl: './reviews.html',
  styleUrl: './reviews.css',
})
export class Reviews implements OnInit {
  readonly reviews = signal<Review[]>([]);
  readonly cycles = signal<PerformanceCycle[]>([]);
  readonly loading = signal(false);

  // Filter bindings
  readonly selectedCycleId = signal('');
  readonly searchQuery = signal('');
  readonly selectedRating = signal<number | null>(null);

  private toastr = inject(ToastrService);

  constructor(
    private reviewService: ReviewService,
    private cycleService: PerformanceCycleService
  ) {}

  ngOnInit(): void {
    this.loadCycles();
    this.loadReviews();
  }

  loadCycles(): void {
    this.cycleService.getCycles().subscribe({
      next: (data) => {
        this.cycles.set(data);
      },
      error: (error) => {
        console.warn('Performance cycles could not be loaded (likely due to role permissions).', error);
        this.cycles.set([]);
      }
    });
  }

  loadReviews(): void {
    this.loading.set(true);
    const filter: ReviewFilter = {};
    if (this.selectedCycleId()) filter.performanceCycleId = this.selectedCycleId();
    if (this.searchQuery()) filter.search = this.searchQuery();
    if (this.selectedRating() !== null) filter.rating = this.selectedRating()!;

    this.reviewService.getReviews(filter).subscribe({
      next: (data: any) => {
        const items = data?.data || (Array.isArray(data) ? data : []);
        this.reviews.set(items);
        this.loading.set(false);
      },
      error: (error) => {
        console.error(error);
        this.loading.set(false);
      }
    });
  }

  applyFilters(): void {
    this.loadReviews();
  }

  resetFilters(): void {
    this.selectedCycleId.set('');
    this.searchQuery.set('');
    this.selectedRating.set(null);
    this.loadReviews();
  }

  deleteReview(id: string): void {
    if (confirm('Are you sure you want to delete this performance review?')) {
      this.reviewService.deleteReview(id).subscribe({
        next: () => {
          this.toastr.success('Review deleted successfully.');
          this.loadReviews();
        },
        error: (error) => {
          console.error(error);
        }
      });
    }
  }

  exportReviews(): void {
    const filter: ReviewFilter = {};
    if (this.selectedCycleId()) filter.performanceCycleId = this.selectedCycleId();
    if (this.searchQuery()) filter.search = this.searchQuery();
    if (this.selectedRating() !== null) filter.rating = this.selectedRating()!;

    this.reviewService.exportReviews(filter).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `performance_reviews_${new Date().toISOString().slice(0, 10)}.xlsx`;
        a.click();
        window.URL.revokeObjectURL(url);
        this.toastr.success('Reviews exported successfully.');
      },
      error: (error) => {
        console.error(error);
      }
    });
  }
}
