import { Component } from '@angular/core';

import { CommonModule } from '@angular/common';

import { EmployeeDetailsStore } from '../../../../stores/employee/employee-details.store';

@Component({
  selector: 'app-overview',
  standalone: true,
  imports: [
    CommonModule
  ],
  templateUrl: './overview.html',
  styleUrl: './overview.css'
})
export class Overview {

  constructor(
    public employeeStore: EmployeeDetailsStore
  ) {}

}