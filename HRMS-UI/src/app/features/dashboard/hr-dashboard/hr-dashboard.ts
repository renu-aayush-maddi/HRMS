import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HrDashboardService } from '../../../core/services/hr-dashboard.service';
import { HrDashboardStats, DepartmentSummary, LeaveSummary } from '../../../core/models/hr-dashboard.model';

import {
  LucideRefreshCw,
  LucideLoader,
  LucideUsers,
  LucideCalendar,
  LucidePlane,
  LucideCoins
} from '@lucide/angular';

@Component({
  selector: 'app-hr-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    LucideRefreshCw,
    LucideLoader,
    LucideUsers,
    LucideCalendar,
    LucidePlane,
    LucideCoins
  ],
  templateUrl: './hr-dashboard.html',
  styleUrl: './hr-dashboard.css',
})
export class HrDashboard implements OnInit {
  readonly stats = signal<HrDashboardStats | undefined>(undefined);
  readonly departments = signal<DepartmentSummary[]>([]);
  readonly leaves = signal<LeaveSummary | undefined>(undefined);
  readonly loading = signal(false);

  constructor(private hrDashboardService: HrDashboardService) {}

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.loading.set(true);
    
    this.hrDashboardService.getStats().subscribe({
      next: (data) => {
        this.stats.set(data);
      },
      error: (err) => console.error(err)
    });

    this.hrDashboardService.getDepartmentSummary().subscribe({
      next: (data) => {
        this.departments.set(data);
      },
      error: (err) => console.error(err)
    });

    this.hrDashboardService.getLeaveSummary().subscribe({
      next: (data) => {
        this.leaves.set(data);
        this.loading.set(false);
      },
      error: (err) => {
        console.error(err);
        this.loading.set(false);
      }
    });
  }
}
