import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { ManagerService } from '../../../core/services/manager.service';
import { PerformanceCycleService } from '../../../core/services/performance-cycle.service';
import { ManagerPerformanceReview, TeamMember, AddPerformanceReview } from '../../../core/models/manager.model';
import { PerformanceCycle } from '../../../core/models/performance-cycle.model';
import { ToastrService } from 'ngx-toastr';

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
    LucideLoader
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

  // Filter
  readonly selectedEmployeeFilter = signal('');

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
        this.loadReviews();
      },
      error: (err) => {
        console.error('Error loading team lookup', err);
        this.error.set('Failed to load team list.');
        this.loading.set(false);
      }
    });
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
            this.selectedEmployeeFilter.set(params['employeeId']);
            // Also, open evaluation modal directly with pre-selected employee!
            this.openAddModalWithEmployee(params['employeeId']);
          } else {
            this.applyFilters();
          }
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

  applyFilters(): void {
    let result = [...this.reviews()];

    if (this.selectedEmployeeFilter()) {
      const member = this.team().find(m => m.id === this.selectedEmployeeFilter());
      if (member) {
        result = result.filter(r => r.employeeName === member.fullName);
      }
    }

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
