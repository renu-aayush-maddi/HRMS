import { Component, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { EmployeeSelfService } from '../../../core/services/employee-self.service';
import { EmployeeResignation } from '../../../core/models/resignation.model';

import {
  LucideCheckCircle,
  LucideAlertTriangle,
  LucideInfo,
  LucideFileText,
  LucideX
} from '@lucide/angular';

@Component({
  selector: 'app-emp-resignation',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    LucideCheckCircle,
    LucideAlertTriangle,
    LucideInfo,
    LucideFileText,
    LucideX
  ],
  templateUrl: './emp-resignation.html',
  styleUrl: './emp-resignation.css',
})
export class EmpResignation implements OnInit {
  readonly resignations = signal<EmployeeResignation[]>([]);
  readonly loading = signal(false);
  readonly showModal = signal(false);
  readonly submitting = signal(false);
  readonly withdrawingId = signal<string | null>(null);

  // Form Fields
  readonly resignationDate = signal('');
  readonly lastWorkingDate = signal('');
  readonly reason = signal('');

  readonly successMessage = signal<string | null>(null);
  readonly errorMessage = signal<string | null>(null);

  readonly hasPendingResignation = computed(() => {
    return this.resignations().some(r => r.status === 'Pending' || r.status === 'Approved');
  });

  constructor(private empService: EmployeeSelfService) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.empService.getMyResignations().subscribe({
      next: (data: any) => {
        const items = data?.data || (Array.isArray(data) ? data : []);
        this.resignations.set(items);
        this.loading.set(false);
      },
      error: () => { this.loading.set(false); }
    });
  }

  openModal(): void {
    this.resignationDate.set('');
    this.lastWorkingDate.set('');
    this.reason.set('');
    this.showModal.set(true);
  }

  closeModal(): void {
    this.showModal.set(false);
  }

  submit(): void {
    if (!this.resignationDate()) {
      this.notify('error', 'Resignation date is required.');
      return;
    }
    this.submitting.set(true);
    this.empService.submitResignation({
      resignationDate: this.resignationDate(),
      lastWorkingDate: this.lastWorkingDate() || undefined,
      reason: this.reason()
    }).subscribe({
      next: () => {
        this.notify('success', 'Resignation submitted successfully.');
        this.submitting.set(false);
        this.closeModal();
        this.load();
      },
      error: (err) => {
        this.notify('error', err?.error?.message || 'Failed to submit resignation.');
        this.submitting.set(false);
      }
    });
  }

  withdraw(id: string): void {
    if (!confirm('Are you sure you want to withdraw this resignation?')) return;
    this.withdrawingId.set(id);
    this.empService.withdrawResignation(id).subscribe({
      next: () => {
        this.notify('success', 'Resignation withdrawn successfully.');
        this.withdrawingId.set(null);
        this.load();
      },
      error: (err) => {
        this.notify('error', err?.error?.message || 'Failed to withdraw resignation.');
        this.withdrawingId.set(null);
      }
    });
  }

  canWithdraw(r: EmployeeResignation): boolean {
    return r.status === 'Pending' || r.status === 'Approved';
  }

  getStatusClass(status: string): string {
    const map: Record<string, string> = {
      'Pending': 'badge-warning',
      'Approved': 'badge-success',
      'Rejected': 'badge-danger',
      'Withdrawn': 'badge-secondary',
    };
    return map[status] || 'badge-secondary';
  }

  getSettlementClass(status: string): string {
    return status === 'Completed' ? 'badge-success' : status === 'In Progress' ? 'badge-info' : 'badge-secondary';
  }

  private notify(type: 'success' | 'error', msg: string): void {
    if (type === 'success') { this.successMessage.set(msg); setTimeout(() => this.successMessage.set(null), 4000); }
    else { this.errorMessage.set(msg); setTimeout(() => this.errorMessage.set(null), 4000); }
  }
}
