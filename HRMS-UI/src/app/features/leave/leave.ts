import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { LeaveService } from '../../core/services/leave.service';
import { LeaveTypeService } from '../../core/services/leave-type.service';
import { LeaveResponse, LeaveFilter, PagedLeaveResult } from '../../core/models/leave.model';
import { LeaveType } from '../../core/models/leave-type.model';
import { ToastrService } from 'ngx-toastr';

import {
  LucideChevronLeft,
  LucideChevronRight,
  LucideX,
  LucideLoader
} from '@lucide/angular';

@Component({
  selector: 'app-leave',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    LucideChevronLeft,
    LucideChevronRight,
    LucideX,
    LucideLoader
  ],
  templateUrl: './leave.html',
  styleUrl: './leave.css'
})
export class Leave implements OnInit {
  readonly leaves = signal<LeaveResponse[]>([]);
  readonly leaveTypes = signal<LeaveType[]>([]);
  readonly loading = signal(false);
  readonly submitting = signal(false);

  // Pagination
  readonly pageNumber = signal(1);
  readonly pageSize = signal(10);
  readonly totalPages = signal(0);
  readonly totalRecords = signal(0);

  // Filters
  readonly searchQuery = signal('');
  readonly selectedLeaveTypeId = signal('');
  readonly selectedStatus = signal('');
  readonly filterFromDate = signal('');
  readonly filterToDate = signal('');

  // Action Modal (Approve/Reject)
  readonly showActionModal = signal(false);
  readonly isApproval = signal(false);
  selectedLeaveId = '';
  actionForm;

  private toastr = inject(ToastrService);

  constructor(
    private leaveService: LeaveService,
    private leaveTypeService: LeaveTypeService,
    private fb: FormBuilder
  ) {
    this.actionForm = this.fb.group({
      comments: ['', [Validators.maxLength(500)]]
    });
  }

  ngOnInit(): void {
    this.loadLeaveTypes();
    this.loadLeaves();
  }

  loadLeaveTypes(): void {
    this.leaveTypeService.getLeaveTypes().subscribe({
      next: (data: LeaveType[]) => {
        this.leaveTypes.set(data);
      },
      error: (err: any) => console.error('Error loading leave types', err)
    });
  }

  loadLeaves(): void {
    this.loading.set(true);
    const filter: LeaveFilter = {
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize()
    };

    if (this.selectedLeaveTypeId()) filter.leaveTypeId = this.selectedLeaveTypeId();
    if (this.selectedStatus()) filter.status = this.selectedStatus();
    if (this.filterFromDate()) filter.fromDate = this.filterFromDate();
    if (this.filterToDate()) filter.toDate = this.filterToDate();

    this.leaveService.getLeaves(filter).subscribe({
      next: (res: PagedLeaveResult) => {
        this.leaves.set(res.data);
        this.totalPages.set(res.totalPages);
        this.totalRecords.set(res.totalRecords);
        this.loading.set(false);
      },
      error: (err: any) => {
        console.error(err);
        this.loading.set(false);
      }
    });
  }

  applyFilters(): void {
    this.pageNumber.set(1);
    this.loadLeaves();
  }

  resetFilters(): void {
    this.selectedLeaveTypeId.set('');
    this.selectedStatus.set('');
    this.filterFromDate.set('');
    this.filterToDate.set('');
    this.pageNumber.set(1);
    this.loadLeaves();
  }

  previousPage(): void {
    if (this.pageNumber() <= 1) return;
    this.pageNumber.update(p => p - 1);
    this.loadLeaves();
  }

  nextPage(): void {
    if (this.pageNumber() >= this.totalPages()) return;
    this.pageNumber.update(p => p + 1);
    this.loadLeaves();
  }

  openActionModal(leaveId: string, approve: boolean): void {
    this.selectedLeaveId = leaveId;
    this.isApproval.set(approve);
    this.actionForm.reset({ comments: '' });
    this.showActionModal.set(true);
  }

  closeActionModal(): void {
    this.showActionModal.set(false);
    this.selectedLeaveId = '';
    this.actionForm.reset();
  }

  submitAction(): void {
    if (this.actionForm.invalid) return;

    const managerComments = this.actionForm.value.comments || '';
    const request = { managerComments };

    this.submitting.set(true);
    if (this.isApproval()) {
      this.leaveService.approveLeave(this.selectedLeaveId, request).subscribe({
        next: () => {
          this.toastr.success('Leave request approved.');
          this.closeActionModal();
          this.loadLeaves();
          this.submitting.set(false);
        },
        error: (err: any) => {
          console.error(err);
          this.submitting.set(false);
        }
      });
    } else {
      this.leaveService.rejectLeave(this.selectedLeaveId, request).subscribe({
        next: () => {
          this.toastr.success('Leave request rejected.');
          this.closeActionModal();
          this.loadLeaves();
          this.submitting.set(false);
        },
        error: (err: any) => {
          console.error(err);
          this.submitting.set(false);
        }
      });
    }
  }

  cancelLeave(leaveId: string): void {
    if (confirm('Are you sure you want to cancel this leave request? This cannot be undone.')) {
      this.submitting.set(true);
      this.leaveService.cancelLeave(leaveId).subscribe({
        next: () => {
          this.toastr.success('Leave request cancelled.');
          this.loadLeaves();
          this.submitting.set(false);
        },
        error: (err: any) => {
          console.error(err);
          this.submitting.set(false);
        }
      });
    }
  }
}
