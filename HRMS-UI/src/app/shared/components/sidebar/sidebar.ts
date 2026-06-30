import { Component, signal, computed, input, output } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { CommonModule } from '@angular/common';

import { AuthStore } from '../../../stores/auth/auth.store';
import { MenuItem } from '../../../core/models/menu-item.model';
import {
  LucideLayoutDashboard,
  LucideUsers,
  LucideUser,
  LucideBuilding,
  LucideClock,
  LucideCalendarClock,
  LucidePalmtree,
  LucideTarget,
  LucideClipboardCheck,
  LucideUserMinus,
  LucideCoins,
  LucideReceipt,
  LucideCalculator,
  LucideSparkles,
  LucideChevronDown,
  LucideChevronRight,
  LucideChevronLeft,
  LucideX,
  LucideWallet,
  LucideNetwork
} from '@lucide/angular';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    RouterLinkActive,
    LucideLayoutDashboard,
    LucideUsers,
    LucideUser,
    LucideBuilding,
    LucideClock,
    LucideCalendarClock,
    LucidePalmtree,
    LucideTarget,
    LucideClipboardCheck,
    LucideUserMinus,
    LucideCoins,
    LucideReceipt,
    LucideCalculator,
    LucideSparkles,
    LucideChevronDown,
    LucideChevronRight,
    LucideChevronLeft,
    LucideX,
    LucideWallet,
    LucideNetwork
  ],
  templateUrl: './sidebar.html',
  styleUrl: './sidebar.css'
})
export class Sidebar {
  readonly collapsed = input(false);
  readonly mobileOpen = input(false);
  readonly toggleCollapse = output<void>();
  readonly closeMobile = output<void>();

  readonly employeesExpanded = signal(false);
  readonly employeeDetailsExpanded = signal(false);

  readonly menuItems = computed<MenuItem[]>(() => {
    const role = this.authStore.currentUser()?.role;
    switch (role) {
      case 'Admin':
        return [
          { label: 'Dashboard', route: '/admin/dashboard' },
          { label: 'Employees', route: '/admin/employees' },
          { label: 'Org Chart', route: '/org-chart' },
          { label: 'Departments', route: '/admin/departments' },
          { label: 'Leave Types', route: '/admin/leave-types' },
          { label: 'Holidays', route: '/admin/holidays' },
          { label: 'Goals', route: '/admin/goals' },
          { label: 'Reviews', route: '/admin/reviews' },
          { label: 'Resignations', route: '/admin/resignations' },
          { label: 'Payroll', route: '/admin/payroll' },
          { label: 'Salary Structures', route: '/admin/salary-structures' },
          { label: 'Additions & Deductions', route: '/admin/additions-deductions' },
          { label: 'Performance Bonus', route: '/admin/performance-bonus' }
        ];

      case 'HR':
        return [
          { label: 'Dashboard', route: '/hr/dashboard' },
          { label: 'Employees', route: '/hr/employees' },
          { label: 'Org Chart', route: '/org-chart' },
          { label: 'Attendance', route: '/hr/attendance' },
          { label: 'Leave', route: '/hr/leave' },
          { label: 'Payroll', route: '/hr/payroll' },
          { label: 'Salary Structures', route: '/hr/salary-structures' },
          { label: 'Additions & Deductions', route: '/hr/additions-deductions' },
          { label: 'Performance Bonus', route: '/hr/performance-bonus' }
        ];

      case 'Manager':
        return [
          { label: 'Dashboard', route: '/manager/dashboard' },
          { label: 'Team', route: '/team' },
          { label: 'Org Chart', route: '/org-chart' },
          { label: 'Goals', route: '/goals' },
          { label: 'Reviews', route: '/reviews' },
          { label: 'Leave Approvals', route: '/leave-approvals' }
        ];

      case 'Employee':
        return [
          { label: 'Dashboard', route: '/employee/dashboard' },
          { label: 'Profile', route: '/profile' },
          { label: 'Org Chart', route: '/org-chart' },
          { label: 'Attendance', route: '/attendance' },
          { label: 'Leave', route: '/leave' },
          { label: 'Goals', route: '/goals' },
          { label: 'Resignation', route: '/resignation' }
        ];

      default:
        return [];
    }
  });

  constructor(
    public authStore: AuthStore
  ) {}

  toggleEmployees(): void {
    if (this.collapsed()) {
      this.toggleCollapse.emit();
    }
    this.employeesExpanded.update(val => !val);
    if (!this.employeesExpanded()) {
      this.employeeDetailsExpanded.set(false);
    }
  }

  toggleEmployeeDetails(): void {
    if (this.collapsed()) {
      this.toggleCollapse.emit();
    }
    this.employeeDetailsExpanded.update(val => !val);
  }
}