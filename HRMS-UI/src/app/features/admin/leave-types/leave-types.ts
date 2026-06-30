import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { LeaveTypeService } from '../../../core/services/leave-type.service';
import { LeaveType, AddLeaveType, UpdateLeaveType } from '../../../core/models/leave-type.model';
import { AuthStore } from '../../../stores/auth/auth.store';
import { ToastrService } from 'ngx-toastr';

import {
  LucidePlus,
  LucideEdit,
  LucideTrash,
  LucideX,
  LucideLoader
} from '@lucide/angular';

@Component({
  selector: 'app-leave-types',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    LucidePlus,
    LucideEdit,
    LucideTrash,
    LucideX,
    LucideLoader
  ],
  templateUrl: './leave-types.html',
  styleUrl: './leave-types.css',
})
export class LeaveTypes implements OnInit {
  readonly leaveTypes = signal<LeaveType[]>([]);
  readonly loading = signal(false);
  readonly submitting = signal(false);
  readonly isAdmin = signal(false);

  readonly showModal = signal(false);
  editingId: string | null = null;
  leaveTypeForm;

  private toastr = inject(ToastrService);

  constructor(
    private leaveTypeService: LeaveTypeService,
    private fb: FormBuilder,
    private authStore: AuthStore
  ) {
    this.leaveTypeForm = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(100)]],
      annualAllocation: [0, [Validators.required, Validators.min(0)]],
      carryForwardAllowed: [false],
      maxCarryForward: [0, [Validators.required, Validators.min(0)]],
      negativeBalanceAllowed: [false],
      isActive: [true]
    });
  }

  ngOnInit(): void {
    this.isAdmin.set(this.authStore.currentUser()?.role === 'Admin');
    this.loadLeaveTypes();
  }

  loadLeaveTypes(): void {
    this.loading.set(true);
    this.leaveTypeService.getLeaveTypes().subscribe({
      next: (data) => {
        this.leaveTypes.set(data);
        this.loading.set(false);
      },
      error: (error) => {
        console.error(error);
        this.loading.set(false);
      }
    });
  }

  openAddModal(): void {
    this.editingId = null;
    this.leaveTypeForm.reset({
      annualAllocation: 0,
      carryForwardAllowed: false,
      maxCarryForward: 0,
      negativeBalanceAllowed: false,
      isActive: true
    });
    this.showModal.set(true);
  }

  openEditModal(type: LeaveType): void {
    this.editingId = type.id;
    this.leaveTypeForm.patchValue({
      name: type.name,
      annualAllocation: type.annualAllocation,
      carryForwardAllowed: type.carryForwardAllowed,
      maxCarryForward: type.maxCarryForward,
      negativeBalanceAllowed: type.negativeBalanceAllowed,
      isActive: type.isActive
    });
    this.showModal.set(true);
  }

  closeModal(): void {
    this.showModal.set(false);
    this.editingId = null;
    this.leaveTypeForm.reset();
  }

  saveLeaveType(): void {
    if (this.leaveTypeForm.invalid) {
      this.leaveTypeForm.markAllAsTouched();
      return;
    }

    const formVal = this.leaveTypeForm.value;
    const dto: any = {
      name: formVal.name!,
      annualAllocation: Number(formVal.annualAllocation!),
      carryForwardAllowed: !!formVal.carryForwardAllowed,
      maxCarryForward: Number(formVal.maxCarryForward!),
      negativeBalanceAllowed: !!formVal.negativeBalanceAllowed
    };

    this.submitting.set(true);
    if (this.editingId) {
      dto.isActive = !!formVal.isActive;
      this.leaveTypeService.updateLeaveType(this.editingId, dto as UpdateLeaveType).subscribe({
        next: (msg) => {
          this.toastr.success(msg || 'Leave type updated successfully.');
          this.closeModal();
          this.loadLeaveTypes();
          this.submitting.set(false);
        },
        error: (error) => {
          console.error(error);
          this.submitting.set(false);
        }
      });
    } else {
      this.leaveTypeService.createLeaveType(dto as AddLeaveType).subscribe({
        next: (msg) => {
          this.toastr.success(msg || 'Leave type created successfully.');
          this.closeModal();
          this.loadLeaveTypes();
          this.submitting.set(false);
        },
        error: (error) => {
          console.error(error);
          this.submitting.set(false);
        }
      });
    }
  }

  deleteLeaveType(id: string): void {
    if (confirm('Are you sure you want to delete this leave type?')) {
      this.leaveTypeService.deleteLeaveType(id).subscribe({
        next: (msg) => {
          this.toastr.success(msg || 'Leave type deleted successfully.');
          this.loadLeaveTypes();
        },
        error: (error) => {
          console.error(error);
        }
      });
    }
  }
}
