import { Component, OnInit, signal, computed, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { EmployeeSelfService } from '../../../core/services/employee-self.service';
import { AttendanceResponse, AttendanceRegularizationResponse } from '../../../core/models/attendance.model';
import { ActivatedRoute, Router } from '@angular/router';
import { FilterBarComponent } from '../../../shared/components/filter-bar/filter-bar';
import { FilterField, SortOption } from '../../../shared/components/filter-bar/filter-bar.model';

import {
  LucideCheckCircle,
  LucideAlertTriangle,
  LucideLogIn,
  LucideLogOut,
  LucideClipboardList,
  LucideChevronLeft,
  LucideChevronRight,
  LucideFileText,
  LucideX
} from '@lucide/angular';

@Component({
  selector: 'app-emp-attendance',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    LucideCheckCircle,
    LucideAlertTriangle,
    LucideLogIn,
    LucideLogOut,
    LucideClipboardList,
    LucideChevronLeft,
    LucideChevronRight,
    LucideFileText,
    LucideX,
    FilterBarComponent
  ],
  templateUrl: './emp-attendance.html',
  styleUrl: './emp-attendance.css',
})
export class EmpAttendance implements OnInit {
  // Attendance log state
  readonly logs = signal<AttendanceResponse[]>([]);
  readonly totalLogs = signal(0);
  readonly logsPage = signal(1);
  readonly logsPageSize = signal(10);
  readonly logsLoading = signal(false);

  // Check-in/out state
  readonly checkingIn = signal(false);
  readonly checkingOut = signal(false);
  readonly todayLog = signal<AttendanceResponse | null>(null);

  // Regularization state
  readonly regularizations = signal<AttendanceRegularizationResponse[]>([]);
  readonly regularizationsLoading = signal(false);
  readonly showRegModal = signal(false);
  readonly submittingReg = signal(false);

  // Regularization Form Fields
  readonly regDate = signal('');
  readonly regCheckIn = signal('');
  readonly regCheckOut = signal('');
  readonly regReason = signal('');

  // Notifications
  readonly successMessage = signal<string | null>(null);
  readonly errorMessage = signal<string | null>(null);

  private router = inject(Router);
  private route = inject(ActivatedRoute);

  // Filters Configuration
  filterFields: FilterField[] = [
    { key: 'status', label: 'Status', type: 'select', options: [
      { value: 'Present', label: 'Present' },
      { value: 'Absent', label: 'Absent' },
      { value: 'Late', label: 'Late' },
      { value: 'On Leave', label: 'On Leave' },
      { value: 'Half Day', label: 'Half Day' }
    ]},
    { key: 'fromDate', label: 'From Date', type: 'date' },
    { key: 'toDate', label: 'To Date', type: 'date' }
  ];

  sortOptions: SortOption[] = [
    { value: 'AttendanceDate', label: 'Attendance Date' },
    { value: 'CheckInTime', label: 'Check In Time' },
    { value: 'CheckOutTime', label: 'Check Out Time' }
  ];

  filters: { [key: string]: any } = {};
  sortBy = 'AttendanceDate';
  descending = true;

  readonly totalPages = computed(() => {
    return Math.ceil(this.totalLogs() / this.logsPageSize());
  });

  constructor(private empService: EmployeeSelfService) { }

  ngOnInit(): void {
    this.loadTodayAttendance();
    this.loadRegularizations();

    this.route.queryParams.subscribe(params => {
      const newFilters: any = {};
      if (params['status']) newFilters['status'] = params['status'];
      if (params['fromDate']) newFilters['fromDate'] = params['fromDate'];
      if (params['toDate']) newFilters['toDate'] = params['toDate'];

      this.filters = newFilters;
      this.sortBy = params['sortBy'] || 'AttendanceDate';
      this.descending = params['descending'] === 'true' || params['descending'] === undefined;
      this.logsPage.set(params['pageNumber'] ? parseInt(params['pageNumber'], 10) : 1);

      this.loadLogs();
    });
  }

  loadTodayAttendance(): void {
    const d = new Date();
    const today = `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`;
    this.empService.getMyAttendance({ fromDate: today, toDate: today, pageSize: 1 }).subscribe({
      next: (result) => {
        this.todayLog.set(result.data?.[0] ?? null);
      },
      error: () => { }
    });
  }

  loadLogs(): void {
    this.logsLoading.set(true);
    this.empService.getMyAttendance({
      fromDate: this.filters['fromDate'] || undefined,
      toDate: this.filters['toDate'] || undefined,
      status: this.filters['status'] || undefined,
      pageNumber: this.logsPage(),
      pageSize: this.logsPageSize(),
      sortBy: this.sortBy,
      descending: this.descending
    }).subscribe({
      next: (result) => {
        this.logs.set(result.data ?? []);
        this.totalLogs.set(result.totalRecords ?? 0);
        this.logsLoading.set(false);
      },
      error: () => {
        this.errorMessage.set('Failed to load attendance logs.');
        this.logsLoading.set(false);
      }
    });
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

  loadRegularizations(): void {
    this.regularizationsLoading.set(true);
    this.empService.getMyRegularizations({ pageSize: 20 }).subscribe({
      next: (result) => {
        this.regularizations.set(result.data ?? []);
        this.regularizationsLoading.set(false);
      },
      error: () => {
        this.regularizationsLoading.set(false);
      }
    });
  }

  checkIn(): void {
    this.checkingIn.set(true);
    this.empService.checkIn().subscribe({
      next: () => {
        this.notify('success', 'Successfully checked in! Have a great day.');
        this.checkingIn.set(false);
        this.loadTodayAttendance();
        this.loadLogs();
      },
      error: (err) => {
        this.notify('error', err?.error?.message || 'Check-in failed. You may have already checked in today.');
        this.checkingIn.set(false);
      }
    });
  }

  checkOut(): void {
    this.checkingOut.set(true);
    this.empService.checkOut().subscribe({
      next: () => {
        this.notify('success', 'Successfully checked out!');
        this.checkingOut.set(false);
        this.loadTodayAttendance();
        this.loadLogs();
      },
      error: (err) => {
        this.notify('error', err?.error?.message || 'Check-out failed. Please try again.');
        this.checkingOut.set(false);
      }
    });
  }

  prevPage(): void {
    if (this.logsPage() <= 1) return;
    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: { pageNumber: this.logsPage() - 1 },
      queryParamsHandling: 'merge'
    });
  }

  nextPage(): void {
    if (this.logsPage() >= this.totalPages()) return;
    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: { pageNumber: this.logsPage() + 1 },
      queryParamsHandling: 'merge'
    });
  }

  openRegModal(): void {
    this.regDate.set('');
    this.regCheckIn.set('');
    this.regCheckOut.set('');
    this.regReason.set('');
    this.showRegModal.set(true);
  }

  closeRegModal(): void {
    this.showRegModal.set(false);
  }

  submitRegularization(): void {
    if (!this.regDate() || !this.regReason().trim()) {
      this.notify('error', 'Attendance date and reason are required.');
      return;
    }

    const dateStr = this.regDate();
    const checkInTime = this.regCheckIn()?.trim();
    const checkOutTime = this.regCheckOut()?.trim();

    const requestedCheckIn = checkInTime ? `${dateStr}T${checkInTime}:00` : undefined;
    const requestedCheckOut = checkOutTime ? `${dateStr}T${checkOutTime}:00` : undefined;

    this.submittingReg.set(true);
    this.empService.createRegularization({
      attendanceDate: dateStr,
      requestedCheckIn: requestedCheckIn,
      requestedCheckOut: requestedCheckOut,
      reason: this.regReason()
    }).subscribe({
      next: () => {
        this.notify('success', 'Regularization request submitted successfully.');
        this.submittingReg.set(false);
        this.closeRegModal();
        this.loadRegularizations();
      },
      error: (err) => {
        this.notify('error', err?.error?.message || 'Failed to submit request. Please try again.');
        this.submittingReg.set(false);
      }
    });
  }

  getWorkHours(checkIn?: string, checkOut?: string): string {
    if (!checkIn || !checkOut) return '-';
    const diff = new Date(checkOut).getTime() - new Date(checkIn).getTime();
    const hours = Math.floor(diff / 3600000);
    const minutes = Math.floor((diff % 3600000) / 60000);
    return `${hours}h ${minutes}m`;
  }

  getStatusClass(status: string): string {
    const map: Record<string, string> = {
      'Present': 'badge-success',
      'Late': 'badge-warning',
      'Absent': 'badge-danger',
      'Half Day': 'badge-info',
    };
    return map[status] || 'badge-secondary';
  }

  getRegStatusClass(status: string): string {
    const map: Record<string, string> = {
      'Approved': 'badge-success',
      'Pending': 'badge-warning',
      'Rejected': 'badge-danger',
    };
    return map[status] || 'badge-secondary';
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
