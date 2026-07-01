import { Component, OnInit, signal, computed, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { EmployeeSelfService, LeaveBalanceItem } from '../../../core/services/employee-self.service';
import { LeaveResponse } from '../../../core/models/leave.model';
import { LeaveTypeService } from '../../../core/services/leave-type.service';
import { LeaveType } from '../../../core/models/leave-type.model';
import { ActivatedRoute, Router } from '@angular/router';
import { FilterBarComponent } from '../../../shared/components/filter-bar/filter-bar';
import { FilterField, SortOption } from '../../../shared/components/filter-bar/filter-bar.model';

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
    LucideCalendar,
    FilterBarComponent
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

  private router = inject(Router);
  private route = inject(ActivatedRoute);

  // Reusable Filter Config
  filterFields: FilterField[] = [];
  sortOptions: SortOption[] = [
    { value: 'StartDate', label: 'Start Date' },
    { value: 'EndDate', label: 'End Date' },
    { value: 'CreatedAt', label: 'Requested Date' }
  ];
  filters: { [key: string]: any } = {};
  sortBy = 'CreatedAt';
  descending = true;

  readonly totalPages = computed(() => {
    return Math.ceil(this.totalLeaves() / this.leavesPageSize());
  });

  constructor(
    private empService: EmployeeSelfService,
    private leaveTypeService: LeaveTypeService
  ) {}

  ngOnInit(): void {
    this.loadBalances();
    this.loadLeaveTypes();
    this.setupFilterFields();

    this.route.queryParams.subscribe(params => {
      const newFilters: any = {};
      if (params['leaveTypeId']) newFilters['leaveTypeId'] = params['leaveTypeId'];
      if (params['status']) newFilters['status'] = params['status'];
      if (params['fromDate']) newFilters['fromDate'] = params['fromDate'];
      if (params['toDate']) newFilters['toDate'] = params['toDate'];

      this.filters = newFilters;
      this.sortBy = params['sortBy'] || 'CreatedAt';
      this.descending = params['descending'] === 'true' || params['descending'] === undefined;
      this.leavesPage.set(params['pageNumber'] ? parseInt(params['pageNumber'], 10) : 1);

      this.loadLeaves();
    });
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
      leaveTypeId: this.filters['leaveTypeId'] || undefined,
      status: this.filters['status'] || undefined,
      fromDate: this.filters['fromDate'] || undefined,
      toDate: this.filters['toDate'] || undefined,
      pageNumber: this.leavesPage(),
      pageSize: this.leavesPageSize(),
      sortBy: this.sortBy,
      descending: this.descending
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
      next: (types) => {
        this.leaveTypes.set(types);
        this.setupFilterFields();
      },
      error: () => {}
    });
  }

  setupFilterFields(): void {
    const leaveTypeOpts = this.leaveTypes().map(t => ({ value: t.id, label: t.name }));
    this.filterFields = [
      { key: 'leaveTypeId', label: 'Leave Type', type: 'select', options: leaveTypeOpts },
      { key: 'status', label: 'Status', type: 'select', options: [
        { value: 'Pending', label: 'Pending' },
        { value: 'Approved', label: 'Approved' },
        { value: 'Rejected', label: 'Rejected' },
        { value: 'Cancelled', label: 'Cancelled' }
      ]},
      { key: 'fromDate', label: 'From Date', type: 'date' },
      { key: 'toDate', label: 'To Date', type: 'date' }
    ];
  }

  onFiltersChanged(updatedFilters: any): void {
    const queryParams: any = { ...updatedFilters, pageNumber: 1 };
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
        descending: event.descending.toString(),
        pageNumber: 1
      },
      queryParamsHandling: 'merge'
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

  prevPage(): void {
    if (this.leavesPage() <= 1) return;
    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: { pageNumber: this.leavesPage() - 1 },
      queryParamsHandling: 'merge'
    });
  }

  nextPage(): void {
    if (this.leavesPage() >= this.totalPages()) return;
    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: { pageNumber: this.leavesPage() + 1 },
      queryParamsHandling: 'merge'
    });
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
