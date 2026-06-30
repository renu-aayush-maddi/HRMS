import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { PerformanceCycleService } from '../../../core/services/performance-cycle.service';
import { PerformanceCycle, AddPerformanceCycle, UpdatePerformanceCycle } from '../../../core/models/performance-cycle.model';
import { ToastrService } from 'ngx-toastr';

import {
  LucideEdit,
  LucideTrash,
  LucideX,
  LucideLoader
} from '@lucide/angular';

@Component({
  selector: 'app-goals',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    LucideEdit,
    LucideTrash,
    LucideX,
    LucideLoader
  ],
  templateUrl: './goals.html',
  styleUrl: './goals.css',
})
export class Goals implements OnInit {
  readonly cycles = signal<PerformanceCycle[]>([]);
  readonly loading = signal(false);
  readonly submitting = signal(false);
  readonly showModal = signal(false);

  editingId: string | null = null;
  cycleForm;

  statuses = ['Active', 'Closed', 'Draft'];
  private toastr = inject(ToastrService);

  constructor(
    private cycleService: PerformanceCycleService,
    private fb: FormBuilder
  ) {
    this.cycleForm = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(100)]],
      startDate: ['', [Validators.required]],
      endDate: ['', [Validators.required]],
      status: ['Draft']
    });
  }

  ngOnInit(): void {
    this.loadCycles();
  }

  loadCycles(): void {
    this.loading.set(true);
    this.cycleService.getCycles().subscribe({
      next: (data) => {
        // Sort descending by startDate
        this.cycles.set(data.sort((a, b) => b.startDate.localeCompare(a.startDate)));
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
    this.cycleForm.reset({
      name: '',
      startDate: '',
      endDate: '',
      status: 'Draft'
    });
    // Remove status validator if adding
    this.cycleForm.get('status')?.clearValidators();
    this.showModal.set(true);
  }

  openEditModal(cycle: PerformanceCycle): void {
    this.editingId = cycle.id;
    this.cycleForm.patchValue({
      name: cycle.name,
      startDate: cycle.startDate,
      endDate: cycle.endDate,
      status: cycle.status
    });
    this.showModal.set(true);
  }

  closeModal(): void {
    this.showModal.set(false);
    this.editingId = null;
    this.cycleForm.reset();
  }

  saveCycle(): void {
    if (this.cycleForm.invalid) {
      this.cycleForm.markAllAsTouched();
      return;
    }

    const formVal = this.cycleForm.value;
    const addDto: AddPerformanceCycle = {
      name: formVal.name!,
      startDate: formVal.startDate!,
      endDate: formVal.endDate!
    };

    this.submitting.set(true);
    if (this.editingId) {
      const updateDto: UpdatePerformanceCycle = {
        name: formVal.name!,
        startDate: formVal.startDate!,
        endDate: formVal.endDate!,
        status: formVal.status!
      };
      this.cycleService.updateCycle(this.editingId, updateDto).subscribe({
        next: (msg) => {
          this.toastr.success(msg || 'Performance cycle updated successfully.');
          this.closeModal();
          this.loadCycles();
          this.submitting.set(false);
        },
        error: (error) => {
          console.error(error);
          this.submitting.set(false);
        }
      });
    } else {
      this.cycleService.createCycle(addDto).subscribe({
        next: (msg) => {
          this.toastr.success(msg || 'Performance cycle initiated successfully.');
          this.closeModal();
          this.loadCycles();
          this.submitting.set(false);
        },
        error: (error) => {
          console.error(error);
          this.submitting.set(false);
        }
      });
    }
  }

  deleteCycle(id: string): void {
    if (confirm('Are you sure you want to delete this performance cycle?')) {
      this.cycleService.deleteCycle(id).subscribe({
        next: (msg) => {
          this.toastr.success(msg || 'Performance cycle deleted successfully.');
          this.loadCycles();
        },
        error: (error) => {
          console.error(error);
        }
      });
    }
  }
}
