import { Component, OnInit } from '@angular/core';

import { RouterOutlet } from '@angular/router';

import { FormsModule } from '@angular/forms';

import { EmployeeService } from '../../../core/services/employee.service';

import { EmployeeLookup } from '../../../core/models/employee-lookup.model';

import { EmployeeDetailsStore } from '../../../stores/employee/employee-details.store';

@Component({
  selector: 'app-employee-details',
  standalone: true,
  imports: [
    RouterOutlet,
    FormsModule
  ],
  templateUrl: './employee-details.html',
  styleUrl: './employee-details.css'
})
export class EmployeeDetails implements OnInit {

  employees: EmployeeLookup[] = [];

  selectedEmployeeId = '';

  constructor(
    private employeeService: EmployeeService,
    public employeeStore: EmployeeDetailsStore
  ) {}

  ngOnInit(): void {

    this.loadEmployees();

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