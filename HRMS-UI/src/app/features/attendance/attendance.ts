import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { AttendanceService } from '../../core/services/attendance.service';
import { ToastrService } from 'ngx-toastr';
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
    LucideLoader
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

  readonly logsSearch = signal('');
  readonly logsStatus = signal('');
  readonly logsFromDate = signal('');
  readonly logsToDate = signal('');

  // Regularizations state
  readonly requests = signal<AttendanceRegularizationResponse[]>([]);
  readonly reqPage = signal(1);
  readonly reqPageSize = signal(10);
  readonly reqTotalPages = signal(0);
  readonly reqTotalRecords = signal(0);

  readonly reqStatus = signal('Pending');
  readonly reqFromDate = signal('');
  readonly reqToDate = signal('');

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
    this.loadData();
  }

  setTab(tab: 'logs' | 'regularizations'): void {
    this.activeTab.set(tab);
    this.loadData();
  }

  loadData(): void {
    if (this.activeTab() === 'logs') {
      this.loadLogs();
    } else {
      this.loadRequests();
    }
  }

  loadLogs(): void {
    this.loading.set(true);
    const filter: AttendanceFilter = {
      pageNumber: this.logsPage(),
      pageSize: this.logsPageSize()
    };

    if (this.logsSearch()) filter.employeeId = this.logsSearch();
    if (this.logsStatus()) filter.status = this.logsStatus();
    if (this.logsFromDate()) filter.fromDate = this.logsFromDate();
    if (this.logsToDate()) filter.toDate = this.logsToDate();

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
      pageSize: this.reqPageSize()
    };

    if (this.reqStatus()) filter.status = this.reqStatus();
    if (this.reqFromDate()) filter.fromDate = this.reqFromDate();
    if (this.reqToDate()) filter.toDate = this.reqToDate();

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

  applyLogsFilters(): void {
    this.logsPage.set(1);
    this.loadLogs();
  }

  resetLogsFilters(): void {
    this.logsSearch.set('');
    this.logsStatus.set('');
    this.logsFromDate.set('');
    this.logsToDate.set('');
    this.logsPage.set(1);
    this.loadLogs();
  }

  applyReqFilters(): void {
    this.reqPage.set(1);
    this.loadRequests();
  }

  resetReqFilters(): void {
    this.reqStatus.set('Pending');
    this.reqFromDate.set('');
    this.reqToDate.set('');
    this.reqPage.set(1);
    this.loadRequests();
  }

  previousLogsPage(): void {
    if (this.logsPage() <= 1) return;
    this.logsPage.update(p => p - 1);
    this.loadLogs();
  }

  nextLogsPage(): void {
    if (this.logsPage() >= this.logsTotalPages()) return;
    this.logsPage.update(p => p + 1);
    this.loadLogs();
  }

  previousReqPage(): void {
    if (this.reqPage() <= 1) return;
    this.reqPage.update(p => p - 1);
    this.loadRequests();
  }

  nextReqPage(): void {
    if (this.reqPage() >= this.reqTotalPages()) return;
    this.reqPage.update(p => p + 1);
    this.loadRequests();
  }

  exportLogs(): void {
    const filter: AttendanceFilter = {};
    if (this.logsSearch()) filter.employeeId = this.logsSearch();
    if (this.logsStatus()) filter.status = this.logsStatus();
    if (this.logsFromDate()) filter.fromDate = this.logsFromDate();
    if (this.logsToDate()) filter.toDate = this.logsToDate();

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
