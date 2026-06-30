import { Component, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { EmployeeSelfService, LeaveBalanceItem } from '../../../core/services/employee-self.service';
import { LeaveResponse } from '../../../core/models/leave.model';
import { LeaveTypeService } from '../../../core/services/leave-type.service';
import { LeaveType } from '../../../core/models/leave-type.model';

import {
  LucideCheckCircle,
  LucideAlertTriangle,
  LucidePalmtree,
  LucideChevronLeft,
  LucideChevronRight,
  LucideX,
  LucideCalendar
} from '@lucide/angular';

@Component({
  selector: 'app-emp-leave',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    LucideCheckCircle,
    LucideAlertTriangle,
    LucidePalmtree,
    LucideChevronLeft,
    LucideChevronRight,
    LucideX,
    LucideCalendar
  ],
  templateUrl: './emp-leave.html',
  styleUrl: './emp-leave.css',
})
export class EmpLeave implements OnInit {
  // Leave balances
  readonly balances = signal<LeaveBalanceItem[]>([]);
  readonly balancesLoading = signal(false);

  // Leave history
  readonly leaves = signal<LeaveResponse[]>([]);
  readonly totalLeaves = signal(0);
  readonly leavesPage = signal(1);
  readonly leavesPageSize = signal(10);
  readonly statusFilter = signal('');
  readonly leavesLoading = signal(false);

  // Leave types
  readonly leaveTypes = signal<LeaveType[]>([]);

  // Apply leave form fields
  readonly showApplyModal = signal(false);
  readonly submittingLeave = signal(false);
  readonly applyLeaveTypeId = signal('');
  readonly applyFromDate = signal('');
  readonly applyToDate = signal('');
  readonly applyReason = signal('');

  // Withdraw
  readonly withdrawingId = signal<string | null>(null);

  readonly successMessage = signal<string | null>(null);
  readonly errorMessage = signal<string | null>(null);

  readonly totalPages = computed(() => {
    return Math.ceil(this.totalLeaves() / this.leavesPageSize());
  });

  constructor(
    private empService: EmployeeSelfService,
    private leaveTypeService: LeaveTypeService
  ) {}

  ngOnInit(): void {
    this.loadBalances();
    this.loadLeaves();
    this.loadLeaveTypes();
  }

  loadBalances(): void {
    this.balancesLoading.set(true);
    this.empService.getLeaveBalances().subscribe({
      next: (data) => {
        this.balances.set(data);
        this.balancesLoading.set(false);
      },
      error: () => { this.balancesLoading.set(false); }
    });
  }

  loadLeaves(): void {
    this.leavesLoading.set(true);
    this.empService.getMyLeaves({
      status: this.statusFilter() || undefined,
      pageNumber: this.leavesPage(),
      pageSize: this.leavesPageSize()
    }).subscribe({
      next: (result) => {
        this.leaves.set(result.data ?? []);
        this.totalLeaves.set(result.totalRecords ?? 0);
        this.leavesLoading.set(false);
      },
      error: () => { this.leavesLoading.set(false); }
    });
  }

  loadLeaveTypes(): void {
    this.leaveTypeService.getLeaveTypes().subscribe({
      next: (types) => { this.leaveTypes.set(types); },
      error: () => {}
    });
  }

  openApplyModal(): void {
    this.applyLeaveTypeId.set('');
    this.applyFromDate.set('');
    this.applyToDate.set('');
    this.applyReason.set('');
    this.showApplyModal.set(true);
  }

  closeApplyModal(): void {
    this.showApplyModal.set(false);
  }

  submitLeave(): void {
    if (!this.applyLeaveTypeId() || !this.applyFromDate() || !this.applyToDate()) {
      this.notify('error', 'Leave type, from date, and to date are required.');
      return;
    }
    if (new Date(this.applyToDate()) < new Date(this.applyFromDate())) {
      this.notify('error', 'To date must be on or after from date.');
      return;
    }
    this.submittingLeave.set(true);
    this.empService.applyLeave({
      leaveTypeId: this.applyLeaveTypeId(),
      fromDate: this.applyFromDate(),
      toDate: this.applyToDate(),
      reason: this.applyReason()
    }).subscribe({
      next: () => {
        this.notify('success', 'Leave request submitted successfully!');
        this.submittingLeave.set(false);
        this.closeApplyModal();
        this.loadLeaves();
        this.loadBalances();
      },
      error: (err) => {
        this.notify('error', err?.error?.message || 'Failed to apply leave. Please try again.');
        this.submittingLeave.set(false);
      }
    });
  }

  withdrawLeave(leaveId: string): void {
    if (!confirm('Are you sure you want to withdraw this leave request?')) return;
    this.withdrawingId.set(leaveId);
    this.empService.withdrawLeave(leaveId).subscribe({
      next: () => {
        this.notify('success', 'Leave request withdrawn successfully.');
        this.withdrawingId.set(null);
        this.loadLeaves();
        this.loadBalances();
      },
      error: (err) => {
        this.notify('error', err?.error?.message || 'Failed to withdraw leave. Please try again.');
        this.withdrawingId.set(null);
      }
    });
  }

  applyStatusFilter(): void {
    this.leavesPage.set(1);
    this.loadLeaves();
  }

  prevPage(): void {
    if (this.leavesPage() > 1) {
      this.leavesPage.update(p => p - 1);
      this.loadLeaves();
    }
  }

  nextPage(): void {
    if (this.leavesPage() < this.totalPages()) {
      this.leavesPage.update(p => p + 1);
      this.loadLeaves();
    }
  }

  getLeaveDuration(fromDate: string, toDate: string): number {
    return Math.round((new Date(toDate).getTime() - new Date(fromDate).getTime()) / 86400000) + 1;
  }

  getStatusClass(status?: string): string {
    const map: Record<string, string> = {
      'Pending': 'badge-warning',
      'Approved': 'badge-success',
      'Rejected': 'badge-danger',
      'Withdrawn': 'badge-secondary',
      'Cancelled': 'badge-secondary',
    };
    return map[status || ''] || 'badge-secondary';
  }

  canWithdraw(status?: string): boolean {
    return status === 'Pending' || status === 'Approved';
  }

  getBalancePercent(balance: LeaveBalanceItem): number {
    if (!balance.allocatedDays) return 0;
    return Math.min(100, (balance.usedDays / balance.allocatedDays) * 100);
  }

  private notify(type: 'success' | 'error', msg: string): void {
    if (type === 'success') {
      this.successMessage.set(msg);
      this.errorMessage.set(null);
      setTimeout(() => this.successMessage.set(null), 4000);
    } else {
      this.errorMessage.set(msg);
      this.successMessage.set(null);
      setTimeout(() => this.errorMessage.set(null), 4000);
    }
  }
}
