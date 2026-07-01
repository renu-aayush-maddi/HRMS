import { Component, OnInit } from '@angular/core';
import { RouterOutlet, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { EmployeeService } from '../../../core/services/employee.service';
import { EmployeeLookup } from '../../../core/models/employee-lookup.model';
import { EmployeeDetailsStore } from '../../../stores/employee/employee-details.store';
import { AuthStore } from '../../../stores/auth/auth.store';

@Component({
  selector: 'app-employee-details',
  standalone: true,
  imports: [
    RouterOutlet,
    FormsModule,
    CommonModule
  ],
  templateUrl: './employee-details.html',
  styleUrl: './employee-details.css'
})
export class EmployeeDetails implements OnInit {

  employees: EmployeeLookup[] = [];
  selectedEmployeeId = '';

  constructor(
    private employeeService: EmployeeService,
    public employeeStore: EmployeeDetailsStore,
    private authStore: AuthStore,
    private router: Router
  ) {}

  ngOnInit(): void {
    const isSelfService = this.router.url.includes('/employee/employee-details');
    if (isSelfService) {
      const user = this.authStore.currentUser();
      const empId = user?.employeeId;
      if (empId) {
        this.selectedEmployeeId = empId;
        this.employeeChanged();
      }
    } else {
      this.loadEmployees();
    }
  }

  showSelector(): boolean {
    return !this.router.url.includes('/employee/employee-details');
  }

  loadEmployees(): void {
    this.employeeService
      .getEmployeeLookup()
      .subscribe({
        next: response => {
          this.employees = response.data;
        }
      });
  }

  employeeChanged(): void {
    if (!this.selectedEmployeeId) {
      this.employeeStore.clear();
      return;
    }

    this.employeeService
      .getEmployeeProfile(this.selectedEmployeeId)
      .subscribe({
        next: response => {
          this.employeeStore.setSelectedEmployee(
            this.selectedEmployeeId
          );
          this.employeeStore.setProfile(response);
        }
      });
  }
}