import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { ManagerService } from '../../../core/services/manager.service';
import { ManagerGoal, TeamMember, AddGoal, UpdateGoalStatus } from '../../../core/models/manager.model';
import { ToastrService } from 'ngx-toastr';

import {
  LucideTarget,
  LucideCheckCircle,
  LucideAlertTriangle,
  LucideEdit,
  LucideX,
  LucideLoader
} from '@lucide/angular';

@Component({
  selector: 'app-manager-goals',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    LucideTarget,
    LucideCheckCircle,
    LucideAlertTriangle,
    LucideEdit,
    LucideX,
    LucideLoader
  ],
  templateUrl: './manager-goals.html',
  styleUrl: './manager-goals.css'
})
export class ManagerGoals implements OnInit {
  readonly goals = signal<ManagerGoal[]>([]);
  readonly filteredGoals = signal<ManagerGoal[]>([]);
  readonly team = signal<TeamMember[]>([]);
  
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);
  readonly successMessage = signal<string | null>(null);

  // Filter state
  readonly selectedEmployeeFilter = signal('');
  readonly selectedStatusFilter = signal('');
  
  private toastr = inject(ToastrService);

  // Modals state
  readonly showAssignModal = signal(false);
  readonly showStatusModal = signal(false);
  readonly selectedGoal = signal<ManagerGoal | null>(null);
  readonly submitting = signal(false);

  // Forms
  assignForm;
  statusForm;

  constructor(
    private managerService: ManagerService,
    private fb: FormBuilder,
    private route: ActivatedRoute
  ) {
    this.assignForm = this.fb.group({
      employeeId: ['', [Validators.required]],
      title: ['', [Validators.required, Validators.maxLength(150)]],
      description: ['', [Validators.required, Validators.maxLength(500)]],
      targetDate: ['', [Validators.required]]
    });

    this.statusForm = this.fb.group({
      status: ['Pending', [Validators.required]]
    });
  }

  ngOnInit(): void {
    this.loadTeamMembers();
  }

  loadTeamMembers(): void {
    this.managerService.getTeamMembers().subscribe({
      next: (members) => {
        this.team.set(members);
        this.loadGoals();
      },
      error: (err) => {
        console.error('Error loading team lookup', err);
        this.error.set('Failed to load team list.');
        this.loading.set(false);
      }
    });
  }

  loadGoals(): void {
    this.loading.set(true);
    this.error.set(null);
    this.managerService.getGoals().subscribe({
      next: (data) => {
        this.goals.set(data);
        
        // Listen to query parameters for auto-filtering
        this.route.queryParams.subscribe(params => {
          if (params['employeeId']) {
            this.selectedEmployeeFilter.set(params['employeeId']);
          }
          this.applyFilters();
        });
        
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Error loading manager goals', err);
        this.error.set('Failed to load team goals. Please try again.');
        this.loading.set(false);
      }
    });
  }

  applyFilters(): void {
    let result = [...this.goals()];

    if (this.selectedEmployeeFilter()) {
      // Find employee's full name to filter by
      const member = this.team().find(m => m.id === this.selectedEmployeeFilter());
      if (member) {
        result = result.filter(g => g.employeeName === member.fullName);
      }
    }

    if (this.selectedStatusFilter()) {
      result = result.filter(g => g.status === this.selectedStatusFilter());
    }

    this.filteredGoals.set(result);
  }

  openAssignModal(): void {
    // If an employee filter is active, pre-select that employee in the form
    if (this.selectedEmployeeFilter()) {
      this.assignForm.patchValue({
        employeeId: this.selectedEmployeeFilter(),
        title: '',
        description: '',
        targetDate: ''
      });
    } else {
      this.assignForm.reset({
        employeeId: '',
        title: '',
        description: '',
        targetDate: ''
      });
    }
    this.showAssignModal.set(true);
  }

  closeAssignModal(): void {
    this.showAssignModal.set(false);
    this.assignForm.reset();
  }

  submitAssignGoal(): void {
    if (this.assignForm.invalid) return;

    this.submitting.set(true);
    const dto: AddGoal = this.assignForm.value as AddGoal;

    this.managerService.addGoal(dto).subscribe({
      next: () => {
        this.toastr.success('Performance goal successfully assigned to employee.');
        this.successMessage.set('Performance goal successfully assigned to employee.');
        this.loadGoals();
        this.closeAssignModal();
        this.submitting.set(false);
        setTimeout(() => this.successMessage.set(null), 3000);
      },
      error: (err) => {
        console.error('Error assigning goal', err);
        this.submitting.set(false);
      }
    });
  }

  openStatusModal(goal: ManagerGoal): void {
    this.selectedGoal.set(goal);
    this.statusForm.patchValue({ status: goal.status });
    this.showStatusModal.set(true);
  }

  closeStatusModal(): void {
    this.showStatusModal.set(false);
    this.selectedGoal.set(null);
  }

  submitStatusUpdate(): void {
    const selected = this.selectedGoal();
    if (!selected || this.statusForm.invalid) return;

    this.submitting.set(true);
    const dto: UpdateGoalStatus = this.statusForm.value as UpdateGoalStatus;

    this.managerService.updateGoalStatus(selected.id, dto).subscribe({
      next: () => {
        this.toastr.success('Goal status updated successfully.');
        this.successMessage.set('Goal status updated successfully.');
        this.loadGoals();
        this.closeStatusModal();
        this.submitting.set(false);
        setTimeout(() => this.successMessage.set(null), 3000);
      },
      error: (err) => {
        console.error('Error updating goal status', err);
        this.submitting.set(false);
      }
    });
  }
}
