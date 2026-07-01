import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { ResignationService } from '../../../core/services/resignation.service';
import { EmployeeResignation, ResignationFilter } from '../../../core/models/resignation.model';
import { ToastrService } from 'ngx-toastr';
import { ActivatedRoute, Router } from '@angular/router';
import { FilterBarComponent } from '../../../shared/components/filter-bar/filter-bar';
import { FilterField, SortOption } from '../../../shared/components/filter-bar/filter-bar.model';

import {
  LucideDownload,
  LucideX,
  LucideLoader
} from '@lucide/angular';

@Component({
  selector: 'app-resignations',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    LucideDownload,
    LucideX,
    LucideLoader,
    FilterBarComponent
  ],
  templateUrl: './resignations.html',
  styleUrl: './resignations.css',
})
export class Resignations implements OnInit {
  readonly resignations = signal<EmployeeResignation[]>([]);
  readonly loading = signal(false);
  readonly submitting = signal(false);

  // Reusable Filter Config & State
  filterFields: FilterField[] = [
    { key: 'searchTerm', label: 'Search', type: 'text', placeholder: 'Search employee name...' },
    { key: 'employeeId', label: 'Employee ID', type: 'text', placeholder: 'Employee GUID...' },
    { key: 'status', label: 'Status', type: 'select', options: [
      { value: 'Pending', label: 'Pending' },
      { value: 'Approved', label: 'Approved' },
      { value: 'Rejected', label: 'Rejected' },
      { value: 'Withdrawn', label: 'Withdrawn' }
    ]},
    { key: 'finalSettlementStatus', label: 'Settlement Status', type: 'select', options: [
      { value: 'Pending', label: 'Pending' },
      { value: 'Processing', label: 'Processing' },
      { value: 'Completed', label: 'Completed' }
    ]},
    { key: 'fromResignationDate', label: 'From Date', type: 'date' },
    { key: 'toResignationDate', label: 'To Date', type: 'date' }
  ];

  sortOptions: SortOption[] = [
    { value: 'ResignationDate', label: 'Resignation Date' },
    { value: 'LastWorkingDay', label: 'Last Working Day' }
  ];

  filters: { [key: string]: any } = {};
  sortBy = 'ResignationDate';
  descending = true;

  // Reject Modal
  readonly showRejectModal = signal(false);
  rejectingId = '';
  rejectForm;

  // Settlement Modal
  readonly showSettlementModal = signal(false);
  settlingId = '';
  settlementForm;
  
  private toastr = inject(ToastrService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  settlementStatuses = ['Pending', 'Processing', 'Completed'];

  constructor(
    private resignationService: ResignationService,
    private fb: FormBuilder
  ) {
    this.rejectForm = this.fb.group({
      reason: ['', [Validators.required, Validators.maxLength(500)]]
    });

    this.settlementForm = this.fb.group({
      status: ['Pending', Validators.required]
    });
  }

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      const newFilters: any = {};
      if (params['searchTerm']) newFilters['searchTerm'] = params['searchTerm'];
      if (params['employeeId']) newFilters['employeeId'] = params['employeeId'];
      if (params['status']) newFilters['status'] = params['status'];
      if (params['finalSettlementStatus']) newFilters['finalSettlementStatus'] = params['finalSettlementStatus'];
      if (params['fromResignationDate']) newFilters['fromResignationDate'] = params['fromResignationDate'];
      if (params['toResignationDate']) newFilters['toResignationDate'] = params['toResignationDate'];

      this.filters = newFilters;
      this.sortBy = params['sortBy'] || 'ResignationDate';
      this.descending = params['descending'] === 'true' || params['descending'] === undefined;

      this.loadResignations();
    });
  }

  loadResignations(): void {
    this.loading.set(true);
    const filter: ResignationFilter = {
      sortBy: this.sortBy,
      descending: this.descending
    };
    if (this.filters['searchTerm']) filter.search = this.filters['searchTerm'];
    if (this.filters['employeeId']) filter.employeeId = this.filters['employeeId'];
    if (this.filters['status']) filter.status = this.filters['status'];
    if (this.filters['finalSettlementStatus']) filter.finalSettlementStatus = this.filters['finalSettlementStatus'];
    if (this.filters['fromResignationDate']) filter.fromResignationDate = this.filters['fromResignationDate'];
    if (this.filters['toResignationDate']) filter.toResignationDate = this.filters['toResignationDate'];

    this.resignationService.getResignations(filter).subscribe({
      next: (data: any) => {
        const items = data?.data || (Array.isArray(data) ? data : []);
        this.resignations.set(items);
        this.loading.set(false);
      },
      error: (error) => {
        console.error(error);
        this.loading.set(false);
      }
    });
  }

  onFiltersChanged(updatedFilters: any): void {
    const queryParams: any = { ...updatedFilters };
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
        descending: event.descending.toString()
      },
      queryParamsHandling: 'merge'
    });
  }

  approveResignation(id: string): void {
    if (confirm('Are you sure you want to approve this resignation request?')) {
      this.submitting.set(true);
      this.resignationService.approveResignation(id).subscribe({
        next: () => {
          this.toastr.success('Resignation request approved successfully.');
          this.loadResignations();
          this.submitting.set(false);
        },
        error: (error) => {
          console.error(error);
          this.submitting.set(false);
        }
      });
    }
  }

  openRejectModal(id: string): void {
    this.rejectingId = id;
    this.rejectForm.reset();
    this.showRejectModal.set(true);
  }

  closeRejectModal(): void {
    this.showRejectModal.set(false);
    this.rejectingId = '';
    this.rejectForm.reset();
  }

  submitRejection(): void {
    if (this.rejectForm.invalid) {
      this.rejectForm.markAllAsTouched();
      return;
    }

    this.submitting.set(true);
    const reason = this.rejectForm.value.reason!;
    this.resignationService.rejectResignation(this.rejectingId, { reason }).subscribe({
      next: () => {
        this.toastr.success('Resignation request rejected.');
        this.closeRejectModal();
        this.loadResignations();
        this.submitting.set(false);
      },
      error: (error) => {
        console.error(error);
        this.submitting.set(false);
      }
    });
  }

  openSettlementModal(res: EmployeeResignation): void {
    this.settlingId = res.id;
    this.settlementForm.reset({
      status: res.finalSettlementStatus || 'Pending'
    });
    this.showSettlementModal.set(true);
  }

  closeSettlementModal(): void {
    this.showSettlementModal.set(false);
    this.settlingId = '';
    this.settlementForm.reset();
  }

  submitSettlement(): void {
    if (this.settlementForm.invalid) return;

    this.submitting.set(true);
    const status = this.settlementForm.value.status!;
    this.resignationService.updateSettlementStatus(this.settlingId, { status }).subscribe({
      next: () => {
        this.toastr.success('Settlement status updated successfully.');
        this.closeSettlementModal();
        this.loadResignations();
        this.submitting.set(false);
      },
      error: (error) => {
        console.error(error);
        this.submitting.set(false);
      }
    });
  }

  exportResignations(): void {
    const filter: ResignationFilter = {};
    if (this.filters['searchTerm']) filter.search = this.filters['searchTerm'];
    if (this.filters['employeeId']) filter.employeeId = this.filters['employeeId'];
    if (this.filters['status']) filter.status = this.filters['status'];
    if (this.filters['finalSettlementStatus']) filter.finalSettlementStatus = this.filters['finalSettlementStatus'];
    if (this.filters['fromResignationDate']) filter.fromResignationDate = this.filters['fromResignationDate'];
    if (this.filters['toResignationDate']) filter.toResignationDate = this.filters['toResignationDate'];

    this.resignationService.exportResignations(filter).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `resignations_export_${new Date().toISOString().slice(0, 10)}.xlsx`;
        a.click();
        window.URL.revokeObjectURL(url);
        this.toastr.success('Resignations list exported successfully.');
      },
      error: (error) => {
        console.error(error);
      }
    });
  }
}
