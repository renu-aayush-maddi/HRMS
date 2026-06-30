import { Component, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { EmployeeSelfService } from '../../../core/services/employee-self.service';
import { AttendanceResponse, AttendanceRegularizationResponse } from '../../../core/models/attendance.model';

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
    LucideX
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
  readonly fromDateFilter = signal('');
  readonly toDateFilter = signal('');
  readonly statusFilter = signal('');
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

  readonly totalPages = computed(() => {
    return Math.ceil(this.totalLogs() / this.logsPageSize());
  });

  constructor(private empService: EmployeeSelfService) { }

  ngOnInit(): void {
    this.loadTodayAttendance();
    this.loadLogs();
    this.loadRegularizations();
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
      fromDate: this.fromDateFilter() || undefined,
      toDate: this.toDateFilter() || undefined,
      status: this.statusFilter() || undefined,
      pageNumber: this.logsPage(),
      pageSize: this.logsPageSize(),
      sortBy: 'attendanceDate',
      descending: true
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

  applyFilters(): void {
    this.logsPage.set(1);
    this.loadLogs();
  }

  resetFilters(): void {
    this.fromDateFilter.set('');
    this.toDateFilter.set('');
    this.statusFilter.set('');
    this.logsPage.set(1);
    this.loadLogs();
  }

  prevPage(): void {
    if (this.logsPage() > 1) {
      this.logsPage.update(p => p - 1);
      this.loadLogs();
    }
  }

  nextPage(): void {
    if (this.logsPage() < this.totalPages()) {
      this.logsPage.update(p => p + 1);
      this.loadLogs();
    }
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
    this.submittingReg.set(true);
    this.empService.createRegularization({
      attendanceDate: this.regDate(),
      requestedCheckIn: this.regCheckIn() || undefined,
      requestedCheckOut: this.regCheckOut() || undefined,
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
