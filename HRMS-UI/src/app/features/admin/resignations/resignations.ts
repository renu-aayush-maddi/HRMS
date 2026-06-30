import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { ResignationService } from '../../../core/services/resignation.service';
import { EmployeeResignation, ResignationFilter } from '../../../core/models/resignation.model';
import { ToastrService } from 'ngx-toastr';

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
    LucideLoader
  ],
  templateUrl: './resignations.html',
  styleUrl: './resignations.css',
})
export class Resignations implements OnInit {
  readonly resignations = signal<EmployeeResignation[]>([]);
  readonly loading = signal(false);
  readonly submitting = signal(false);

  // Filters
  readonly searchQuery = signal('');
  readonly selectedStatus = signal('');
  readonly selectedSettlementStatus = signal('');

  // Reject Modal
  readonly showRejectModal = signal(false);
  rejectingId = '';
  rejectForm;

  // Settlement Modal
  readonly showSettlementModal = signal(false);
  settlingId = '';
  settlementForm;
  
  private toastr = inject(ToastrService);

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
    this.loadResignations();
  }

  loadResignations(): void {
    this.loading.set(true);
    const filter: ResignationFilter = {};
    if (this.searchQuery()) filter.search = this.searchQuery();
    if (this.selectedStatus()) filter.status = this.selectedStatus();
    if (this.selectedSettlementStatus()) filter.finalSettlementStatus = this.selectedSettlementStatus();

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

  applyFilters(): void {
    this.loadResignations();
  }

  resetFilters(): void {
    this.searchQuery.set('');
    this.selectedStatus.set('');
    this.selectedSettlementStatus.set('');
    this.loadResignations();
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
    if (this.searchQuery()) filter.search = this.searchQuery();
    if (this.selectedStatus()) filter.status = this.selectedStatus();
    if (this.selectedSettlementStatus()) filter.finalSettlementStatus = this.selectedSettlementStatus();

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
