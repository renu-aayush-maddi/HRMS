import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ManagerService } from '../../../core/services/manager.service';
import { TeamAttendance } from '../../../core/models/manager.model';

import { LucideAlertTriangle } from '@lucide/angular';

@Component({
  selector: 'app-manager-attendance',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAlertTriangle],
  templateUrl: './manager-attendance.html',
  styleUrl: './manager-attendance.css'
})
export class ManagerAttendance implements OnInit {
  readonly logs = signal<TeamAttendance[]>([]);
  readonly filteredLogs = signal<TeamAttendance[]>([]);
  
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);
  
  // Filters
  readonly employeeQuery = signal('');
  readonly statusFilter = signal('');
  readonly dateFilter = signal('');

  constructor(private managerService: ManagerService) {}

  ngOnInit(): void {
    this.loadAttendance();
  }

  loadAttendance(): void {
    this.loading.set(true);
    this.error.set(null);
    this.managerService.getTeamAttendance().subscribe({
      next: (data) => {
        this.logs.set(data);
        this.applyFilters();
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Error loading team attendance logs', err);
        this.error.set('Failed to load attendance logs. Please try again.');
        this.loading.set(false);
      }
    });
  }

  applyFilters(): void {
    let result = [...this.logs()];

    if (this.employeeQuery().trim()) {
      const q = this.employeeQuery().toLowerCase();
      result = result.filter(log => log.employeeName.toLowerCase().includes(q));
    }

    if (this.statusFilter()) {
      result = result.filter(log => log.status === this.statusFilter());
    }

    if (this.dateFilter()) {
      result = result.filter(log => log.attendanceDate.toString().startsWith(this.dateFilter()));
    }

    this.filteredLogs.set(result);
  }

  resetFilters(): void {
    this.employeeQuery.set('');
    this.statusFilter.set('');
    this.dateFilter.set('');
    this.applyFilters();
  }

  getWorkHours(checkIn?: string, checkOut?: string): number | null {
    if (!checkIn || !checkOut) return null;
    const inTime = new Date(checkIn).getTime();
    const outTime = new Date(checkOut).getTime();
    return Math.round(((outTime - inTime) / 3600000) * 100) / 100;
  }
}
