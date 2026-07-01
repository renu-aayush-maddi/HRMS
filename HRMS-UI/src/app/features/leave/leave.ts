import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { LeaveService } from '../../core/services/leave.service';
import { LeaveTypeService } from '../../core/services/leave-type.service';
import { LeaveResponse, LeaveFilter, PagedLeaveResult } from '../../core/models/leave.model';
import { LeaveType } from '../../core/models/leave-type.model';
import { ToastrService } from 'ngx-toastr';
import { ActivatedRoute, Router } from '@angular/router';
import { FilterBarComponent } from '../../shared/components/filter-bar/filter-bar';
import { FilterField, SortOption } from '../../shared/components/filter-bar/filter-bar.model';

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
    LucideLoader,
    FilterBarComponent
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

  // Reusable Filter State and Configuration
  filterFields: FilterField[] = [];
  sortOptions: SortOption[] = [
    { value: 'StartDate', label: 'Start Date' },
    { value: 'EndDate', label: 'End Date' },
    { value: 'CreatedAt', label: 'Requested Date' }
  ];
  filters: { [key: string]: any } = {};
  sortBy = 'CreatedAt';
  descending = true;

  // Action Modal (Approve/Reject)
  readonly showActionModal = signal(false);
  readonly isApproval = signal(false);
  selectedLeaveId = '';
  actionForm;

  private toastr = inject(ToastrService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

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
    this.setupFilterFields();

    this.route.queryParams.subscribe(params => {
      const newFilters: any = {};
      if (params['employeeId']) newFilters['employeeId'] = params['employeeId'];
      if (params['leaveTypeId']) newFilters['leaveTypeId'] = params['leaveTypeId'];
      if (params['status']) newFilters['status'] = params['status'];
      if (params['fromDate']) newFilters['fromDate'] = params['fromDate'];
      if (params['toDate']) newFilters['toDate'] = params['toDate'];

      this.filters = newFilters;
      this.sortBy = params['sortBy'] || 'CreatedAt';
      this.descending = params['descending'] === 'true' || params['descending'] === undefined;
      this.pageNumber.set(params['pageNumber'] ? parseInt(params['pageNumber'], 10) : 1);

      this.loadLeaves();
    });
  }

  loadLeaveTypes(): void {
    this.leaveTypeService.getLeaveTypes().subscribe({
      next: (data: LeaveType[]) => {
        this.leaveTypes.set(data);
        this.setupFilterFields();
      },
      error: (err: any) => console.error('Error loading leave types', err)
    });
  }

  setupFilterFields(): void {
    const leaveTypeOpts = this.leaveTypes().map(t => ({ value: t.id, label: t.name }));
    this.filterFields = [
      { key: 'employeeId', label: 'Employee ID', type: 'text', placeholder: 'Search Employee ID...' },
      { key: 'leaveTypeId', label: 'Leave Type', type: 'select', options: leaveTypeOpts },
      { key: 'status', label: 'Status', type: 'select', options: [
        { value: 'Pending', label: 'Pending' },
        { value: 'Approved', label: 'Approved' },
        { value: 'Rejected', label: 'Rejected' },
        { value: 'Cancelled', label: 'Cancelled' }
      ]},
      { key: 'fromDate', label: 'From Date', type: 'date' },
      { key: 'toDate', label: 'To Date', type: 'date' }
    ];
  }

  loadLeaves(): void {
    this.loading.set(true);
    const filter: LeaveFilter = {
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize(),
      sortBy: this.sortBy,
      descending: this.descending
    };

    if (this.filters['employeeId']) filter.employeeId = this.filters['employeeId'];
    if (this.filters['leaveTypeId']) filter.leaveTypeId = this.filters['leaveTypeId'];
    if (this.filters['status']) filter.status = this.filters['status'];
    if (this.filters['fromDate']) filter.fromDate = this.filters['fromDate'];
    if (this.filters['toDate']) filter.toDate = this.filters['toDate'];

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

  onFiltersChanged(updatedFilters: any): void {
    const queryParams: any = { ...updatedFilters, pageNumber: 1 };
    Object.keys(queryParams).forEach(k => {
      if (queryParams[k] === null || queryParams[k] === undefined || queryParams[k] === '') {
        delete queryParams[k];
      }
    });

    this.router.navigate([], {
      relativeTo: this.route,
      queryParams,
      queryParamsHandling: 'merge'
    });
  }

  onSortChanged(event: { sortBy: string; descending: boolean }): void {
    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: {
        sortBy: event.sortBy,
        descending: event.descending.toString(),
        pageNumber: 1
      },
      queryParamsHandling: 'merge'
    });
  }

  previousPage(): void {
    if (this.pageNumber() <= 1) return;
    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: { pageNumber: this.pageNumber() - 1 },
      queryParamsHandling: 'merge'
    });
  }

  nextPage(): void {
    if (this.pageNumber() >= this.totalPages()) return;
    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: { pageNumber: this.pageNumber() + 1 },
      queryParamsHandling: 'merge'
    });
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
