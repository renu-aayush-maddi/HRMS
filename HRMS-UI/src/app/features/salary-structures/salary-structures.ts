import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { SalaryStructureService } from '../../core/services/salary-structure.service';
import { SalaryStructureResponse } from '../../core/models/salary-structure.model';
import { ToastrService } from 'ngx-toastr';

import {
  LucidePlus,
  LucideX,
  LucideInfo,
  LucideLoader
} from '@lucide/angular';

@Component({
  selector: 'app-salary-structures',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    LucidePlus,
    LucideX,
    LucideInfo,
    LucideLoader
  ],
  templateUrl: './salary-structures.html',
  styleUrl: './salary-structures.css'
})
export class SalaryStructures implements OnInit {
  readonly structures = signal<SalaryStructureResponse[]>([]);
  readonly loading = signal(false);
  readonly submitting = signal(false);

  // Add/Edit Modal
  readonly showModal = signal(false);
  readonly isEdit = signal(false);
  selectedStructureId: string | null = null;
  structureForm;
  
  private toastr = inject(ToastrService);

  constructor(
    private service: SalaryStructureService,
    private fb: FormBuilder
  ) {
    this.structureForm = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(100)]],
      basicPercentage: [0, [Validators.required, Validators.min(0), Validators.max(100)]],
      hraPercentage: [0, [Validators.required, Validators.min(0), Validators.max(100)]],
      specialAllowancePercentage: [0, [Validators.required, Validators.min(0), Validators.max(100)]],
      medicalAllowancePercentage: [0, [Validators.required, Validators.min(0), Validators.max(100)]],
      travelAllowancePercentage: [0, [Validators.required, Validators.min(0), Validators.max(100)]]
    });
  }

  ngOnInit(): void {
    this.loadStructures();
  }

  loadStructures(): void {
    this.loading.set(true);
    this.service.getAll().subscribe({
      next: (res: SalaryStructureResponse[]) => {
        this.structures.set(res);
        this.loading.set(false);
      },
      error: (err: any) => {
        console.error(err);
        this.loading.set(false);
      }
    });
  }

  openAddModal(): void {
    this.isEdit.set(false);
    this.selectedStructureId = null;
    this.structureForm.reset({
      name: '',
      basicPercentage: 40,
      hraPercentage: 20,
      specialAllowancePercentage: 30,
      medicalAllowancePercentage: 5,
      travelAllowancePercentage: 5
    });
    this.showModal.set(true);
  }

  openEditModal(struct: SalaryStructureResponse): void {
    this.isEdit.set(true);
    this.selectedStructureId = struct.id;
    this.structureForm.reset({
      name: struct.name,
      basicPercentage: struct.basicPercentage,
      hraPercentage: struct.hraPercentage,
      specialAllowancePercentage: struct.specialAllowancePercentage,
      medicalAllowancePercentage: struct.medicalAllowancePercentage,
      travelAllowancePercentage: struct.travelAllowancePercentage
    });
    this.showModal.set(true);
  }

  closeModal(): void {
    this.showModal.set(false);
    this.selectedStructureId = null;
    this.structureForm.reset();
  }

  submitForm(): void {
    if (this.structureForm.invalid) {
      this.structureForm.markAllAsTouched();
      return;
    }

    const val = this.structureForm.value;
    const total = (val.basicPercentage || 0) + (val.hraPercentage || 0) + 
                  (val.specialAllowancePercentage || 0) + (val.medicalAllowancePercentage || 0) + 
                  (val.travelAllowancePercentage || 0);

    if (total !== 100) {
      this.toastr.warning(`The sum of all percentages must equal exactly 100%. Current total is ${total}%.`);
      return;
    }

    const payload = {
      name: val.name || '',
      basicPercentage: val.basicPercentage || 0,
      hraPercentage: val.hraPercentage || 0,
      specialAllowancePercentage: val.specialAllowancePercentage || 0,
      medicalAllowancePercentage: val.medicalAllowancePercentage || 0,
      travelAllowancePercentage: val.travelAllowancePercentage || 0
    };

    this.submitting.set(true);
    if (this.isEdit() && this.selectedStructureId) {
      this.service.update(this.selectedStructureId, payload).subscribe({
        next: () => {
          this.toastr.success('Salary structure updated successfully.');
          this.closeModal();
          this.loadStructures();
          this.submitting.set(false);
        },
        error: (err: any) => {
          console.error(err);
          this.submitting.set(false);
        }
      });
    } else {
      this.service.create(payload).subscribe({
        next: () => {
          this.toastr.success('Salary structure created successfully.');
          this.closeModal();
          this.loadStructures();
          this.submitting.set(false);
        },
        error: (err: any) => {
          console.error(err);
          this.submitting.set(false);
        }
      });
    }
  }

  deleteStructure(id: string): void {
    if (confirm('Are you sure you want to delete this salary structure? This cannot be undone.')) {
      this.service.delete(id).subscribe({
        next: () => {
          this.toastr.success('Salary structure deleted.');
          this.loadStructures();
        },
        error: (err: any) => {
          console.error(err);
        }
      });
    }
  }
}
