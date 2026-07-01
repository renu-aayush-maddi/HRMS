import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ReviewService } from '../../../core/services/review.service';
import { PerformanceCycleService } from '../../../core/services/performance-cycle.service';
import { Review, ReviewFilter } from '../../../core/models/review.model';
import { PerformanceCycle } from '../../../core/models/performance-cycle.model';
import { ToastrService } from 'ngx-toastr';
import { ActivatedRoute, Router } from '@angular/router';
import { FilterBarComponent } from '../../../shared/components/filter-bar/filter-bar';
import { FilterField, SortOption } from '../../../shared/components/filter-bar/filter-bar.model';

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
    LucideTrash,
    FilterBarComponent
  ],
  templateUrl: './reviews.html',
  styleUrl: './reviews.css',
})
export class Reviews implements OnInit {
  readonly reviews = signal<Review[]>([]);
  readonly cycles = signal<PerformanceCycle[]>([]);
  readonly loading = signal(false);

  // Reusable Filter Config & State
  filterFields: FilterField[] = [];
  sortOptions: SortOption[] = [
    { value: 'ReviewDate', label: 'Review Date' },
    { value: 'Rating', label: 'Rating' }
  ];
  filters: { [key: string]: any } = {};
  sortBy = 'ReviewDate';
  descending = true;

  private toastr = inject(ToastrService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  constructor(
    private reviewService: ReviewService,
    private cycleService: PerformanceCycleService
  ) {}

  ngOnInit(): void {
    this.loadCycles();
    this.setupFilterFields();

    this.route.queryParams.subscribe(params => {
      const newFilters: any = {};
      if (params['searchTerm']) newFilters['searchTerm'] = params['searchTerm'];
      if (params['employeeId']) newFilters['employeeId'] = params['employeeId'];
      if (params['reviewerId']) newFilters['reviewerId'] = params['reviewerId'];
      if (params['performanceCycleId']) newFilters['performanceCycleId'] = params['performanceCycleId'];
      if (params['minRating']) newFilters['minRating'] = parseInt(params['minRating'], 10);
      if (params['maxRating']) newFilters['maxRating'] = parseInt(params['maxRating'], 10);
      if (params['fromReviewDate']) newFilters['fromReviewDate'] = params['fromReviewDate'];
      if (params['toReviewDate']) newFilters['toReviewDate'] = params['toReviewDate'];

      this.filters = newFilters;
      this.sortBy = params['sortBy'] || 'ReviewDate';
      this.descending = params['descending'] === 'true' || params['descending'] === undefined;

      this.loadReviews();
    });
  }

  loadCycles(): void {
    this.cycleService.getCycles().subscribe({
      next: (data) => {
        this.cycles.set(data);
        this.setupFilterFields();
      },
      error: (error) => {
        console.warn('Performance cycles could not be loaded (likely due to role permissions).', error);
        this.cycles.set([]);
      }
    });
  }

  setupFilterFields(): void {
    const cycleOpts = this.cycles().map(c => ({ value: c.id, label: c.name }));
    this.filterFields = [
      { key: 'searchTerm', label: 'Search', type: 'text', placeholder: 'Search employee name...' },
      { key: 'employeeId', label: 'Employee ID', type: 'text', placeholder: 'Employee GUID...' },
      { key: 'reviewerId', label: 'Reviewer ID', type: 'text', placeholder: 'Reviewer GUID...' },
      { key: 'performanceCycleId', label: 'Performance Cycle', type: 'select', options: cycleOpts },
      { key: 'minRating', label: 'Min Rating', type: 'number', placeholder: 'Min rating (1-5)...' },
      { key: 'maxRating', label: 'Max Rating', type: 'number', placeholder: 'Max rating (1-5)...' },
      { key: 'fromReviewDate', label: 'From Date', type: 'date' },
      { key: 'toReviewDate', label: 'To Date', type: 'date' }
    ];
  }

  loadReviews(): void {
    this.loading.set(true);
    const filter: ReviewFilter = {
      sortBy: this.sortBy,
      descending: this.descending
    };
    if (this.filters['searchTerm']) filter.search = this.filters['searchTerm'];
    if (this.filters['employeeId']) filter.employeeId = this.filters['employeeId'];
    if (this.filters['reviewerId']) filter.reviewerId = this.filters['reviewerId'];
    if (this.filters['performanceCycleId']) filter.performanceCycleId = this.filters['performanceCycleId'];
    if (this.filters['minRating']) filter.minRating = this.filters['minRating'];
    if (this.filters['maxRating']) filter.maxRating = this.filters['maxRating'];
    if (this.filters['fromReviewDate']) filter.fromReviewDate = this.filters['fromReviewDate'];
    if (this.filters['toReviewDate']) filter.toReviewDate = this.filters['toReviewDate'];

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

  onFiltersChanged(updatedFilters: any): void {
    const queryParams: any = { ...updatedFilters };
    Object.keys(queryParams).forEach(k => {
      if (queryParams[k] === null || queryParams[k] === undefined || queryParams[k] === '') {
        delete queryParams[k];
      }
    });

    this.router.navigate([], {
      relativeTo: this.route,
      queryParams,
      queryParamsHandling: 'merge'
    });
  }

  onSortChanged(event: { sortBy: string; descending: boolean }): void {
    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: {
        sortBy: event.sortBy,
        descending: event.descending.toString()
      },
      queryParamsHandling: 'merge'
    });
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
    if (this.filters['searchTerm']) filter.search = this.filters['searchTerm'];
    if (this.filters['employeeId']) filter.employeeId = this.filters['employeeId'];
    if (this.filters['reviewerId']) filter.reviewerId = this.filters['reviewerId'];
    if (this.filters['performanceCycleId']) filter.performanceCycleId = this.filters['performanceCycleId'];
    if (this.filters['minRating']) filter.minRating = this.filters['minRating'];
    if (this.filters['maxRating']) filter.maxRating = this.filters['maxRating'];
    if (this.filters['fromReviewDate']) filter.fromReviewDate = this.filters['fromReviewDate'];
    if (this.filters['toReviewDate']) filter.toReviewDate = this.filters['toReviewDate'];

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
