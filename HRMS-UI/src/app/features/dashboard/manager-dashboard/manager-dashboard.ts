import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ManagerService } from '../../../core/services/manager.service';
import { ManagerDashboardStats, LateEmployee } from '../../../core/models/manager.model';

import {
  LucideAlertTriangle,
  LucideUsers,
  LucideCheckCircle,
  LucidePalmtree,
  LucideClock,
  LucideAlarmClock,
  LucideSun,
  LucideFolder,
  LucideCalendar,
  LucideTarget,
  LucideFileText
} from '@lucide/angular';

@Component({
  selector: 'app-manager-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    LucideAlertTriangle,
    LucideUsers,
    LucideCheckCircle,
    LucidePalmtree,
    LucideClock,
    LucideAlarmClock,
    LucideSun,
    LucideFolder,
    LucideCalendar,
    LucideTarget,
    LucideFileText
  ],
  templateUrl: './manager-dashboard.html',
  styleUrl: './manager-dashboard.css',
})
export class ManagerDashboard implements OnInit {
  readonly stats = signal<ManagerDashboardStats | null>(null);
  readonly lateEmployees = signal<LateEmployee[]>([]);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);

  constructor(private managerService: ManagerService) {}

  ngOnInit(): void {
    this.loadDashboardData();
  }

  loadDashboardData(): void {
    this.loading.set(true);
    this.error.set(null);
    
    this.managerService.getDashboard().subscribe({
      next: (statsData) => {
        this.stats.set(statsData);
        this.loadLateEmployees();
      },
      error: (err) => {
        console.error('Error loading manager dashboard stats', err);
        this.error.set('Failed to load dashboard metrics. Please try again later.');
        this.loading.set(false);
      }
    });
  }

  loadLateEmployees(): void {
    this.managerService.getLateEmployees().subscribe({
      next: (lateData) => {
        this.lateEmployees.set(lateData);
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Error loading late employees', err);
        // We do not fail the whole page if just late employees fails
        this.loading.set(false);
      }
    });
  }
}
