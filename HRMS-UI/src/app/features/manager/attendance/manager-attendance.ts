import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ManagerService } from '../../../core/services/manager.service';
import { TeamAttendance } from '../../../core/models/manager.model';
import { FilterBarComponent } from '../../../shared/components/filter-bar/filter-bar';
import { FilterField, SortOption } from '../../../shared/components/filter-bar/filter-bar.model';

import { LucideAlertTriangle } from '@lucide/angular';

@Component({
  selector: 'app-manager-attendance',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAlertTriangle, FilterBarComponent],
  templateUrl: './manager-attendance.html',
  styleUrl: './manager-attendance.css'
})
export class ManagerAttendance implements OnInit {
  readonly logs = signal<TeamAttendance[]>([]);
  readonly filteredLogs = signal<TeamAttendance[]>([]);
  
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);
  
  // Reusable Filter Bar Config
  filterFields: FilterField[] = [
    { key: 'employeeName', label: 'Employee Name', type: 'text', placeholder: 'Search by employee...' },
    { key: 'status', label: 'Status', type: 'select', options: [
      { value: 'Present', label: 'Present' },
      { value: 'Absent', label: 'Absent' },
      { value: 'Late', label: 'Late' },
      { value: 'On Leave', label: 'On Leave' },
      { value: 'Half Day', label: 'Half Day' }
    ]},
    { key: 'date', label: 'Date', type: 'date' }
  ];

  sortOptions: SortOption[] = [
    { value: 'employeeName', label: 'Employee Name' },
    { value: 'attendanceDate', label: 'Attendance Date' },
    { value: 'checkInTime', label: 'Check In Time' },
    { value: 'checkOutTime', label: 'Check Out Time' }
  ];

  filters: { [key: string]: any } = {};
  sortBy = 'attendanceDate';
  descending = true;

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

  onFiltersChanged(updatedFilters: any): void {
    this.filters = updatedFilters;
    this.applyFilters();
  }

  onSortChanged(event: { sortBy: string; descending: boolean }): void {
    this.sortBy = event.sortBy;
    this.descending = event.descending;
    this.applyFilters();
  }

  applyFilters(): void {
    let result = [...this.logs()];

    const query = this.filters['employeeName'] || this.filters['search'];
    if (query) {
      const q = query.toLowerCase();
      result = result.filter(log => log.employeeName.toLowerCase().includes(q));
    }

    if (this.filters['status']) {
      result = result.filter(log => log.status === this.filters['status']);
    }

    if (this.filters['date']) {
      result = result.filter(log => log.attendanceDate.toString().startsWith(this.filters['date']));
    }

    // Client-side sort
    result.sort((a: any, b: any) => {
      let valA = a[this.sortBy];
      let valB = b[this.sortBy];
      if (valA === undefined || valA === null) return 1;
      if (valB === undefined || valB === null) return -1;
      if (typeof valA === 'string') {
        return this.descending ? valB.localeCompare(valA) : valA.localeCompare(valB);
      }
      return this.descending ? valB - valA : valA - valB;
    });

    this.filteredLogs.set(result);
  }

  resetFilters(): void {
    this.filters = {};
    this.applyFilters();
  }

  getWorkHours(checkIn?: string, checkOut?: string): number | null {
    if (!checkIn || !checkOut) return null;
    const inTime = new Date(checkIn).getTime();
    const outTime = new Date(checkOut).getTime();
    return Math.round(((outTime - inTime) / 3600000) * 100) / 100;
  }
}
