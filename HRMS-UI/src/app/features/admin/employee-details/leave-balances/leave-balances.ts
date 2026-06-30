import { Component, OnInit, signal, effect, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { LeaveBalanceService } from '../../../../core/services/leave-balance.service';
import { LeaveTypeService } from '../../../../core/services/leave-type.service';
import { EmployeeDetailsStore } from '../../../../stores/employee/employee-details.store';
import { LeaveBalanceResponse } from '../../../../core/models/leave-balance.model';
import { LeaveType } from '../../../../core/models/leave-type.model';
import { ToastrService } from 'ngx-toastr';

import {
  LucideAlertTriangle,
  LucidePlus,
  LucideX,
  LucideLoader
} from '@lucide/angular';

@Component({
  selector: 'app-employee-leave-balances',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    LucideAlertTriangle,
    LucidePlus,
    LucideX,
    LucideLoader
  ],
  templateUrl: './leave-balances.html',
  styleUrl: './leave-balances.css'
})
export class LeaveBalances implements OnInit {
  readonly employeeId = signal('');
  readonly balances = signal<LeaveBalanceResponse[]>([]);
  readonly leaveTypes = signal<LeaveType[]>([]);
  readonly loading = signal(false);
  readonly submitting = signal(false);

  readonly showAllocateModal = signal(false);
  allocateForm;
  
  private toastr = inject(ToastrService);

  constructor(
    public employeeStore: EmployeeDetailsStore,
    private leaveBalanceService: LeaveBalanceService,
    private leaveTypeService: LeaveTypeService,
    private fb: FormBuilder
  ) {
    // React to selected employee changes
    effect(() => {
      const id = this.employeeStore.selectedEmployeeId();
      if (id) {
        this.employeeId.set(id);
        this.loadBalances();
      } else {
        this.employeeId.set('');
        this.balances.set([]);
      }
    });

    this.allocateForm = this.fb.group({
      leaveTypeId: ['', [Validators.required]],
      allocatedDays: [1, [Validators.required, Validators.min(1), Validators.max(365)]]
    });
  }

  ngOnInit(): void {
    this.loadLeaveTypes();
  }

  loadLeaveTypes(): void {
    this.leaveTypeService.getLeaveTypes().subscribe({
      next: (res) => {
        this.leaveTypes.set(res);
      },
      error: (err) => console.error('Failed to load leave types', err)
    });
  }

  loadBalances(): void {
    if (!this.employeeId()) return;
    this.loading.set(true);

    this.leaveBalanceService.getEmployeeBalances(this.employeeId()).subscribe({
      next: (res) => {
        this.balances.set(res);
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Failed to load employee leave balances', err);
        this.balances.set([]);
        this.loading.set(false);
      }
    });
  }

  openModal(): void {
    this.allocateForm.reset({
      leaveTypeId: '',
      allocatedDays: 1
    });
    this.showAllocateModal.set(true);
  }

  closeModal(): void {
    this.showAllocateModal.set(false);
    this.allocateForm.reset();
  }

  submitAllocation(): void {
    if (this.allocateForm.invalid) {
      this.allocateForm.markAllAsTouched();
      return;
    }

    const val = this.allocateForm.value;
    const payload = {
      employeeId: this.employeeId(),
      leaveTypeId: val.leaveTypeId || '',
      allocatedDays: Number(val.allocatedDays || 0)
    };

    this.submitting.set(true);
    this.leaveBalanceService.allocate(payload).subscribe({
      next: () => {
        this.toastr.success('Leave balance allocated successfully.');
        this.closeModal();
        this.loadBalances();
        this.submitting.set(false);
      },
      error: (err) => {
        console.error('Failed to allocate leave balance', err);
        this.submitting.set(false);
      }
    });
  }
}
