import { Component, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { EmployeeSelfService } from '../../../core/services/employee-self.service';
import { ManagerGoal } from '../../../core/models/manager.model';

import {
  LucideCheckCircle,
  LucideAlertTriangle,
  LucideTarget,
  LucideCalendar
} from '@lucide/angular';

@Component({
  selector: 'app-emp-goals',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    LucideCheckCircle,
    LucideAlertTriangle,
    LucideTarget,
    LucideCalendar
  ],
  templateUrl: './emp-goals.html',
  styleUrl: './emp-goals.css',
})
export class EmpGoals implements OnInit {
  readonly goals = signal<ManagerGoal[]>([]);
  readonly filteredGoals = signal<ManagerGoal[]>([]);
  readonly loading = signal(false);
  readonly statusFilter = signal('');
  readonly updatingId = signal<string | null>(null);
  readonly successMessage = signal<string | null>(null);
  readonly errorMessage = signal<string | null>(null);

  readonly statusOptions = ['Pending', 'In Progress', 'Completed'];

  readonly stats = computed(() => {
    const list = this.goals();
    return {
      pending: list.filter(g => g.status === 'Pending' || g.status === 'Pending').length,
      inProgress: list.filter(g => g.status === 'In Progress' || g.status === 'InProgress').length,
      completed: list.filter(g => g.status === 'Completed').length,
    };
  });

  constructor(private empService: EmployeeSelfService) {}

  ngOnInit(): void {
    this.loadGoals();
  }

  loadGoals(): void {
    this.loading.set(true);
    this.empService.getMyGoals().subscribe({
      next: (data) => {
        this.goals.set(data);
        this.applyFilter();
        this.loading.set(false);
      },
      error: () => { this.loading.set(false); }
    });
  }

  applyFilter(): void {
    this.filteredGoals.set(this.statusFilter()
      ? this.goals().filter(g => g.status === this.statusFilter())
      : [...this.goals()]);
  }

  updateStatus(goalId: string, newStatus: string): void {
    this.updatingId.set(goalId);
    this.empService.updateMyGoalStatus(goalId, { status: newStatus }).subscribe({
      next: () => {
        this.goals.update(list => list.map(g => g.id === goalId ? { ...g, status: newStatus } : g));
        this.applyFilter();
        this.notify('success', `Goal status updated to "${newStatus}".`);
        this.updatingId.set(null);
      },
      error: () => {
        this.notify('error', 'Failed to update goal status. Please try again.');
        this.updatingId.set(null);
      }
    });
  }

  getStatusClass(status: string): string {
    const map: Record<string, string> = {
      'Pending': 'badge-warning',
      'InProgress': 'badge-info',
      'In Progress': 'badge-info',
      'Completed': 'badge-success',
    };
    return map[status] || 'badge-secondary';
  }

  private notify(type: 'success' | 'error', msg: string): void {
    if (type === 'success') { this.successMessage.set(msg); setTimeout(() => this.successMessage.set(null), 3000); }
    else { this.errorMessage.set(msg); setTimeout(() => this.errorMessage.set(null), 4000); }
  }
}
