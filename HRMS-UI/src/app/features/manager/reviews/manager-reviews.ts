import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { ManagerService } from '../../../core/services/manager.service';
import { PerformanceCycleService } from '../../../core/services/performance-cycle.service';
import { ManagerPerformanceReview, TeamMember, AddPerformanceReview } from '../../../core/models/manager.model';
import { PerformanceCycle } from '../../../core/models/performance-cycle.model';
import { ToastrService } from 'ngx-toastr';
import { FilterBarComponent } from '../../../shared/components/filter-bar/filter-bar';
import { FilterField, SortOption } from '../../../shared/components/filter-bar/filter-bar.model';

import {
  LucideFileText,
  LucideCheckCircle,
  LucideAlertTriangle,
  LucideStar,
  LucideX,
  LucideLoader
} from '@lucide/angular';

@Component({
  selector: 'app-manager-reviews',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    LucideFileText,
    LucideCheckCircle,
    LucideAlertTriangle,
    LucideStar,
    LucideX,
    LucideLoader,
    FilterBarComponent
  ],
  templateUrl: './manager-reviews.html',
  styleUrl: './manager-reviews.css'
})
export class ManagerReviews implements OnInit {
  readonly reviews = signal<ManagerPerformanceReview[]>([]);
  readonly filteredReviews = signal<ManagerPerformanceReview[]>([]);
  readonly team = signal<TeamMember[]>([]);
  readonly cycles = signal<PerformanceCycle[]>([]);

  readonly loading = signal(true);
  readonly error = signal<string | null>(null);
  readonly successMessage = signal<string | null>(null);

  // Reusable Filter Config & State
  filterFields: FilterField[] = [];
  sortOptions: SortOption[] = [
    { value: 'reviewDate', label: 'Review Date' },
    { value: 'rating', label: 'Rating' }
  ];
  filters: { [key: string]: any } = {};
  sortBy = 'reviewDate';
  descending = true;

  // Modals state
  readonly showAddModal = signal(false);
  readonly submitting = signal(false);
  
  // Form
  reviewForm;

  private toastr = inject(ToastrService);

  // Rating helper
  stars = [1, 2, 3, 4, 5];

  constructor(
    private managerService: ManagerService,
    private cycleService: PerformanceCycleService,
    private fb: FormBuilder,
    private route: ActivatedRoute
  ) {
    this.reviewForm = this.fb.group({
      employeeId: ['', [Validators.required]],
      performanceCycleId: ['', [Validators.required]],
      rating: [5, [Validators.required, Validators.min(1), Validators.max(5)]],
      comments: ['', [Validators.required, Validators.maxLength(1000)]]
    });
  }

  ngOnInit(): void {
    this.loadTeamMembers();
    this.loadActiveCycles();
    this.setupFilterFields();
  }

  loadActiveCycles(): void {
    this.cycleService.getCycles().subscribe({
      next: (data) => {
        // Only load Active cycles
        this.cycles.set(data.filter(c => c.status === 'Active'));
      },
      error: (err) => {
        console.error('Error loading cycles lookup', err);
      }
    });
  }

  loadTeamMembers(): void {
    this.managerService.getTeamMembers().subscribe({
      next: (members) => {
        this.team.set(members);
        this.setupFilterFields();
        this.loadReviews();
      },
      error: (err) => {
        console.error('Error loading team lookup', err);
        this.error.set('Failed to load team list.');
        this.loading.set(false);
      }
    });
  }

  setupFilterFields(): void {
    const memberOpts = this.team().map(t => ({ value: t.id, label: t.fullName }));
    this.filterFields = [
      { key: 'employeeId', label: 'Employee', type: 'select', options: memberOpts },
      { key: 'rating', label: 'Rating', type: 'select', options: [
        { value: 1, label: '1 Star' },
        { value: 2, label: '2 Stars' },
        { value: 3, label: '3 Stars' },
        { value: 4, label: '4 Stars' },
        { value: 5, label: '5 Stars' }
      ]}
    ];
  }

  loadReviews(): void {
    this.loading.set(true);
    this.error.set(null);
    this.managerService.getPerformanceReviews().subscribe({
      next: (data) => {
        this.reviews.set(data);
        
        // Listen to query parameters for auto-filtering or auto-evaluation
        this.route.queryParams.subscribe(params => {
          if (params['employeeId']) {
            this.filters['employeeId'] = params['employeeId'];
            // Also, open evaluation modal directly with pre-selected employee!
            this.openAddModalWithEmployee(params['employeeId']);
          }
          this.applyFilters();
        });
        
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Error loading manager reviews', err);
        this.error.set('Failed to load performance reviews. Please try again.');
        this.loading.set(false);
      }
    });
  }

  onFiltersChanged(updatedFilters: any): void {
    this.filters = updatedFilters;
    this.applyFilters();
  }

  onSortChanged(event: { sortBy: string; descending: boolean }): void {
    this.sortBy = event.sortBy;
    this.descending = event.descending;
    this.applyFilters();
  }

  applyFilters(): void {
    let result = [...this.reviews()];

    if (this.filters['employeeId']) {
      const member = this.team().find(m => m.id === this.filters['employeeId']);
      if (member) {
        result = result.filter(r => r.employeeName === member.fullName);
      }
    }

    if (this.filters['rating']) {
      result = result.filter(r => r.rating === parseInt(this.filters['rating'], 10));
    }

    // Client-side sort
    result.sort((a: any, b: any) => {
      let valA = a[this.sortBy];
      let valB = b[this.sortBy];
      if (valA === undefined || valA === null) return 1;
      if (valB === undefined || valB === null) return -1;
      if (typeof valA === 'string') {
        return this.descending ? valB.localeCompare(valA) : valA.localeCompare(valB);
      }
      return this.descending ? valB - valA : valA - valB;
    });

    this.filteredReviews.set(result);
  }

  openAddModalWithEmployee(employeeId: string): void {
    this.reviewForm.reset({
      employeeId: employeeId,
      performanceCycleId: '',
      rating: 5,
      comments: ''
    });
    this.showAddModal.set(true);
  }

  openAddModal(): void {
    this.reviewForm.reset({
      employeeId: '',
      performanceCycleId: '',
      rating: 5,
      comments: ''
    });
    this.showAddModal.set(true);
  }

  closeAddModal(): void {
    this.showAddModal.set(false);
    this.reviewForm.reset();
  }

  setRating(rating: number): void {
    this.reviewForm.patchValue({ rating });
  }

  submitReview(): void {
    if (this.reviewForm.invalid) {
      this.reviewForm.markAllAsTouched();
      return;
    }

    this.submitting.set(true);
    // getRawValue gets everything, even disabled elements
    const val = this.reviewForm.getRawValue();
    const dto: AddPerformanceReview = {
      employeeId: val.employeeId || '',
      performanceCycleId: val.performanceCycleId || '',
      rating: val.rating || 5,
      comments: val.comments || ''
    };

    this.managerService.addPerformanceReview(dto).subscribe({
      next: () => {
        this.toastr.success('Performance evaluation submitted successfully.');
        this.successMessage.set('Performance review evaluation submitted successfully.');
        this.loadReviews();
        this.closeAddModal();
        this.submitting.set(false);
        setTimeout(() => this.successMessage.set(null), 3000);
      },
      error: (err) => {
        console.error('Error submitting performance review', err);
        this.submitting.set(false);
      }
    });
  }

  getSelectedRating(): number {
    return this.reviewForm.get('rating')?.value ?? 0;
  }
}
