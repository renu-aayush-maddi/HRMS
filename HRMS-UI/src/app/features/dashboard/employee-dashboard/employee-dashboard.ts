import { Component, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { EmployeeSelfService } from '../../../core/services/employee-self.service';
import { LeaveBalanceItem } from '../../../core/services/employee-self.service';
import { AttendanceResponse } from '../../../core/models/attendance.model';

import {
  LucideCheckCircle,
  LucideAlertTriangle,
  LucideLogIn,
  LucideLogOut,
  LucidePalmtree,
  LucideClock,
  LucideTarget,
  LucideCoins,
  LucideCalendar,
  LucideStar,
  LucideUser,
  LucideFileText
} from '@lucide/angular';

@Component({
  selector: 'app-employee-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    LucideCheckCircle,
    LucideAlertTriangle,
    LucideLogIn,
    LucideLogOut,
    LucidePalmtree,
    LucideClock,
    LucideTarget,
    LucideCoins,
    LucideCalendar,
    LucideStar,
    LucideUser,
    LucideFileText
  ],
  templateUrl: './employee-dashboard.html',
  styleUrl: './employee-dashboard.css',
})
export class EmployeeDashboard implements OnInit {
  readonly leaveBalances = signal<LeaveBalanceItem[]>([]);
  readonly todayAttendance = signal<AttendanceResponse | null>(null);
  readonly pendingLeaves = signal(0);
  readonly pendingGoals = signal(0);
  readonly loading = signal(true);
  readonly checkingIn = signal(false);
  readonly checkingOut = signal(false);
  readonly error = signal<string | null>(null);
  readonly successMessage = signal<string | null>(null);
  today = new Date();

  readonly totalLeaveAllocated = computed(() => {
    return this.leaveBalances().reduce((s, b) => s + b.allocatedDays, 0);
  });

  readonly totalLeaveUsed = computed(() => {
    return this.leaveBalances().reduce((s, b) => s + b.usedDays, 0);
  });

  constructor(private empService: EmployeeSelfService) { }

  ngOnInit(): void {
    this.loadDashboard();
  }

  loadDashboard(): void {
    this.loading.set(true);
    this.error.set(null);

    // Load leave balances
    this.empService.getLeaveBalances().subscribe({
      next: (balances) => {
        this.leaveBalances.set(balances);
      },
      error: () => { }
    });

    // Load today's attendance
    const d = new Date();
    const today = `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`;
    this.empService.getMyAttendance({ fromDate: today, toDate: today, pageSize: 1 }).subscribe({
      next: (result) => {
        this.todayAttendance.set(result.data?.[0] ?? null);
      },
      error: () => { }
    });

    // Load my leaves to count pending
    this.empService.getMyLeaves({ status: 'Pending', pageSize: 1 }).subscribe({
      next: (result) => {
        this.pendingLeaves.set(result.totalRecords ?? 0);
      },
      error: () => { }
    });

    // Load goals count
    this.empService.getMyGoals().subscribe({
      next: (goals) => {
        this.pendingGoals.set(goals.filter(g => g.status !== 'Completed').length);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
      }
    });
  }

  checkIn(): void {
    this.checkingIn.set(true);
    this.empService.checkIn().subscribe({
      next: () => {
        this.successMessage.set('Successfully checked in! Have a productive day.');
        this.checkingIn.set(false);
        this.loadDashboard();
        setTimeout(() => this.successMessage.set(null), 4000);
      },
      error: (err) => {
        this.error.set(err?.error?.message || 'Failed to check in. You may have already checked in today.');
        this.checkingIn.set(false);
        setTimeout(() => this.error.set(null), 4000);
      }
    });
  }

  checkOut(): void {
    this.checkingOut.set(true);
    this.empService.checkOut().subscribe({
      next: () => {
        this.successMessage.set('Successfully checked out! Great work today.');
        this.checkingOut.set(false);
        this.loadDashboard();
        setTimeout(() => this.successMessage.set(null), 4000);
      },
      error: (err) => {
        this.error.set(err?.error?.message || 'Failed to check out. Please try again.');
        this.checkingOut.set(false);
        setTimeout(() => this.error.set(null), 4000);
      }
    });
  }

  getAttendanceStatusClass(): string {
    const att = this.todayAttendance();
    if (!att) return 'status-absent';
    if (att.status === 'Present') return 'status-present';
    if (att.status === 'Late') return 'status-late';
    return 'status-absent';
  }
}
