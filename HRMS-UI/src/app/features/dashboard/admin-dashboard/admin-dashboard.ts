import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HrDashboardService } from '../../../core/services/hr-dashboard.service';
import { HolidayService } from '../../../core/services/holiday.service';
import { HrDashboardStats, DepartmentSummary, LeaveSummary } from '../../../core/models/hr-dashboard.model';
import { Holiday } from '../../../core/models/holiday.model';

import {
  LucideRefreshCw,
  LucideLoader,
  LucideUsers,
  LucideCalendar,
  LucideMail,
  LucideCoins,
  LucideUser,
  LucideCreditCard,
  LucideFileText,
  LucidePalmtree
} from '@lucide/angular';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    LucideRefreshCw,
    LucideLoader,
    LucideUsers,
    LucideCalendar,
    LucideMail,
    LucideCoins,
    LucideUser,
    LucideCreditCard,
    LucideFileText,
    LucidePalmtree
  ],
  templateUrl: './admin-dashboard.html',
  styleUrl: './admin-dashboard.css',
})
export class AdminDashboard implements OnInit {
  readonly stats = signal<HrDashboardStats | null>(null);
  readonly departments = signal<DepartmentSummary[]>([]);
  readonly leaveSummary = signal<LeaveSummary | null>(null);
  readonly upcomingHolidays = signal<Holiday[]>([]);
  readonly loading = signal(false);

  constructor(
    private dashboardService: HrDashboardService,
    private holidayService: HolidayService
  ) {}

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.loading.set(true);
    
    // Load Stats
    this.dashboardService.getStats().subscribe({
      next: data => {
        this.stats.set(data);
      },
      error: err => console.error('Error loading stats', err)
    });

    // Load Department Breakdown
    this.dashboardService.getDepartmentSummary().subscribe({
      next: data => {
        this.departments.set(data);
      },
      error: err => console.error('Error loading departments summary', err)
    });

    // Load Leave Summary
    this.dashboardService.getLeaveSummary().subscribe({
      next: data => {
        this.leaveSummary.set(data);
      },
      error: err => console.error('Error loading leave summary', err)
    });

    // Load Upcoming Holidays
    this.holidayService.getUpcomingHolidays().subscribe({
      next: data => {
        this.upcomingHolidays.set(data);
        this.loading.set(false);
      },
      error: err => {
        console.error('Error loading holidays', err);
        this.loading.set(false);
      }
    });
  }
}
