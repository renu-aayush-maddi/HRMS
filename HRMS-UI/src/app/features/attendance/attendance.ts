import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { AttendanceService } from '../../core/services/attendance.service';
import { ToastrService } from 'ngx-toastr';
import { ActivatedRoute, Router } from '@angular/router';
import { FilterBarComponent } from '../../shared/components/filter-bar/filter-bar';
import { FilterField, SortOption } from '../../shared/components/filter-bar/filter-bar.model';
import {
  LucideDownload,
  LucideChevronLeft,
  LucideChevronRight,
  LucideX,
  LucideLoader
} from '@lucide/angular';
import { 
  AttendanceResponse, 
  AttendanceFilter, 
  AttendanceRegularizationResponse, 
  AttendanceRegularizationFilter,
  PagedAttendanceResult,
  PagedRegularizationResult
} from '../../core/models/attendance.model';

@Component({
  selector: 'app-attendance',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    LucideDownload,
    LucideChevronLeft,
    LucideChevronRight,
    LucideX,
    LucideLoader,
    FilterBarComponent
  ],
  templateUrl: './attendance.html',
  styleUrl: './attendance.css'
})
export class Attendance implements OnInit {
  readonly activeTab = signal<'logs' | 'regularizations'>('logs');
  readonly loading = signal(false);
  readonly submitting = signal(false);

  // Logs state
  readonly logs = signal<AttendanceResponse[]>([]);
  readonly logsPage = signal(1);
  readonly logsPageSize = signal(10);
  readonly logsTotalPages = signal(0);
  readonly logsTotalRecords = signal(0);
  
  private toastr = inject(ToastrService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  // Logs configs
  logsFilterFields: FilterField[] = [
    { key: 'employeeId', label: 'Employee ID', type: 'text', placeholder: 'Search Employee ID...' },
    { key: 'status', label: 'Status', type: 'select', options: [
      { value: 'Present', label: 'Present' },
      { value: 'Absent', label: 'Absent' },
      { value: 'Late', label: 'Late' },
      { value: 'On Leave', label: 'On Leave' },
      { value: 'Half Day', label: 'Half Day' }
    ]},
    { key: 'fromDate', label: 'From Date', type: 'date' },
    { key: 'toDate', label: 'To Date', type: 'date' }
  ];
  logsSortOptions: SortOption[] = [
    { value: 'AttendanceDate', label: 'Attendance Date' },
    { value: 'CheckInTime', label: 'Check In Time' },
    { value: 'CheckOutTime', label: 'Check Out Time' }
  ];
  logsFilters: { [key: string]: any } = {};
  logsSortBy = 'AttendanceDate';
  logsDescending = true;

  // Regularizations state
  readonly requests = signal<AttendanceRegularizationResponse[]>([]);
  readonly reqPage = signal(1);
  readonly reqPageSize = signal(10);
  readonly reqTotalPages = signal(0);
  readonly reqTotalRecords = signal(0);

  // Regularizations configs
  reqFilterFields: FilterField[] = [
    { key: 'status', label: 'Status', type: 'select', options: [
      { value: 'Pending', label: 'Pending' },
      { value: 'Approved', label: 'Approved' },
      { value: 'Rejected', label: 'Rejected' }
    ]},
    { key: 'fromDate', label: 'From Date', type: 'date' },
    { key: 'toDate', label: 'To Date', type: 'date' }
  ];
  reqSortOptions: SortOption[] = [
    { value: 'RequestedDate', label: 'Requested Date' }
  ];
  reqFilters: { [key: string]: any } = {};
  reqSortBy = 'RequestedDate';
  reqDescending = true;

  // Approve/Reject Modals
  readonly showActionModal = signal(false);
  readonly isApproval = signal(true);
  selectedRequestId = '';
  actionForm;

  constructor(
    private attendanceService: AttendanceService,
    private fb: FormBuilder
  ) {
    this.actionForm = this.fb.group({
      comments: ['', [Validators.maxLength(500)]]
    });
  }

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      const activeTab = params['tab'] || 'logs';
      this.activeTab.set(activeTab as 'logs' | 'regularizations');

      if (activeTab === 'logs') {
        const newFilters: any = {};
        if (params['employeeId']) newFilters['employeeId'] = params['employeeId'];
        if (params['status']) newFilters['status'] = params['status'];
        if (params['fromDate']) newFilters['fromDate'] = params['fromDate'];
        if (params['toDate']) newFilters['toDate'] = params['toDate'];

        this.logsFilters = newFilters;
        this.logsSortBy = params['sortBy'] || 'AttendanceDate';
        this.logsDescending = params['descending'] === 'true' || params['descending'] === undefined;
        this.logsPage.set(params['pageNumber'] ? parseInt(params['pageNumber'], 10) : 1);
        
        this.loadLogs();
      } else {
        const newFilters: any = {};
        if (params['status']) newFilters['status'] = params['status'];
        if (params['fromDate']) newFilters['fromDate'] = params['fromDate'];
        if (params['toDate']) newFilters['toDate'] = params['toDate'];

        this.reqFilters = newFilters;
        this.reqSortBy = params['sortBy'] || 'RequestedDate';
        this.reqDescending = params['descending'] === 'true' || params['descending'] === undefined;
        this.reqPage.set(params['pageNumber'] ? parseInt(params['pageNumber'], 10) : 1);

        this.loadRequests();
      }
    });
  }

  setTab(tab: 'logs' | 'regularizations'): void {
    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: { tab, pageNumber: 1 },
      queryParamsHandling: 'merge'
    });
  }

  loadLogs(): void {
    this.loading.set(true);
    const filter: AttendanceFilter = {
      pageNumber: this.logsPage(),
      pageSize: this.logsPageSize(),
      sortBy: this.logsSortBy,
      descending: this.logsDescending
    };

    if (this.logsFilters['employeeId']) filter.employeeId = this.logsFilters['employeeId'];
    if (this.logsFilters['status']) filter.status = this.logsFilters['status'];
    if (this.logsFilters['fromDate']) filter.fromDate = this.logsFilters['fromDate'];
    if (this.logsFilters['toDate']) filter.toDate = this.logsFilters['toDate'];

    this.attendanceService.getAttendance(filter).subscribe({
      next: (res: PagedAttendanceResult) => {
        this.logs.set(res.data);
        this.logsTotalPages.set(res.totalPages);
        this.logsTotalRecords.set(res.totalRecords);
        this.loading.set(false);
      },
      error: (err: any) => {
        console.error(err);
        this.loading.set(false);
      }
    });
  }

  loadRequests(): void {
    this.loading.set(true);
    const filter: AttendanceRegularizationFilter = {
      pageNumber: this.reqPage(),
      pageSize: this.reqPageSize(),
      sortBy: this.reqSortBy,
      descending: this.reqDescending
    };

    if (this.reqFilters['status']) filter.status = this.reqFilters['status'];
    if (this.reqFilters['fromDate']) filter.fromDate = this.reqFilters['fromDate'];
    if (this.reqFilters['toDate']) filter.toDate = this.reqFilters['toDate'];

    this.attendanceService.getRegularizationRequests(filter).subscribe({
      next: (res: PagedRegularizationResult) => {
        this.requests.set(res.data);
        this.reqTotalPages.set(res.totalPages);
        this.reqTotalRecords.set(res.totalRecords);
        this.loading.set(false);
      },
      error: (err: any) => {
        console.error(err);
        this.loading.set(false);
      }
    });
  }

  onLogsFilterChanged(updatedFilters: any): void {
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

  onLogsSortChanged(event: { sortBy: string; descending: boolean }): void {
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

  onReqFilterChanged(updatedFilters: any): void {
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

  onReqSortChanged(event: { sortBy: string; descending: boolean }): void {
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

  previousLogsPage(): void {
    if (this.logsPage() <= 1) return;
    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: { pageNumber: this.logsPage() - 1 },
      queryParamsHandling: 'merge'
    });
  }

  nextLogsPage(): void {
    if (this.logsPage() >= this.logsTotalPages()) return;
    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: { pageNumber: this.logsPage() + 1 },
      queryParamsHandling: 'merge'
    });
  }

  previousReqPage(): void {
    if (this.reqPage() <= 1) return;
    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: { pageNumber: this.reqPage() - 1 },
      queryParamsHandling: 'merge'
    });
  }

  nextReqPage(): void {
    if (this.reqPage() >= this.reqTotalPages()) return;
    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: { pageNumber: this.reqPage() + 1 },
      queryParamsHandling: 'merge'
    });
  }

  exportLogs(): void {
    const filter: AttendanceFilter = {};
    if (this.logsFilters['employeeId']) filter.employeeId = this.logsFilters['employeeId'];
    if (this.logsFilters['status']) filter.status = this.logsFilters['status'];
    if (this.logsFilters['fromDate']) filter.fromDate = this.logsFilters['fromDate'];
    if (this.logsFilters['toDate']) filter.toDate = this.logsFilters['toDate'];

    this.submitting.set(true);
    this.attendanceService.exportAttendance(filter).subscribe({
      next: (blob: Blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `attendance_logs_${new Date().toISOString().slice(0,10)}.xlsx`;
        a.click();
        window.URL.revokeObjectURL(url);
        this.toastr.success('Attendance logs exported successfully.');
        this.submitting.set(false);
      },
      error: (err: any) => {
        console.error(err);
        this.submitting.set(false);
      }
    });
  }

  openActionModal(id: string, approve: boolean): void {
    this.selectedRequestId = id;
    this.isApproval.set(approve);
    this.actionForm.reset({ comments: '' });
    
    if (!approve) {
      this.actionForm.get('comments')?.setValidators([Validators.required, Validators.maxLength(500)]);
    } else {
      this.actionForm.get('comments')?.setValidators([Validators.maxLength(500)]);
    }
    this.actionForm.get('comments')?.updateValueAndValidity();

    this.showActionModal.set(true);
  }

  closeActionModal(): void {
    this.showActionModal.set(false);
    this.selectedRequestId = '';
    this.actionForm.reset();
  }

  submitAction(): void {
    if (this.actionForm.invalid) {
      this.actionForm.markAllAsTouched();
      return;
    }

    const hrComments = this.actionForm.value.comments || '';

    this.submitting.set(true);
    if (this.isApproval()) {
      this.attendanceService.approveRegularization(this.selectedRequestId, { hrComments }).subscribe({
        next: () => {
          this.toastr.success('Regularization request approved successfully.');
          this.closeActionModal();
          this.loadRequests();
          this.submitting.set(false);
        },
        error: (err: any) => {
          console.error(err);
          this.submitting.set(false);
        }
      });
    } else {
      this.attendanceService.rejectRegularization(this.selectedRequestId, { hrComments }).subscribe({
        next: () => {
          this.toastr.success('Regularization request rejected.');
          this.closeActionModal();
          this.loadRequests();
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
