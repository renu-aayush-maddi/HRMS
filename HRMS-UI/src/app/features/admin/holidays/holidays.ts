import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { HolidayService } from '../../../core/services/holiday.service';
import { Holiday, AddHoliday, UpdateHoliday } from '../../../core/models/holiday.model';
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
  selector: 'app-holidays',
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
  templateUrl: './holidays.html',
  styleUrl: './holidays.css',
})
export class Holidays implements OnInit {
  readonly holidays = signal<Holiday[]>([]);
  readonly loading = signal(false);
  readonly submitting = signal(false);
  readonly isAdmin = signal(false);

  readonly showModal = signal(false);
  editingId: string | null = null;
  holidayForm;

  private toastr = inject(ToastrService);

  constructor(
    private holidayService: HolidayService,
    private fb: FormBuilder,
    private authStore: AuthStore
  ) {
    this.holidayForm = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(200)]],
      holidayDate: ['', [Validators.required]],
      description: ['', [Validators.maxLength(1000)]],
      isOptional: [false]
    });
  }

  ngOnInit(): void {
    this.isAdmin.set(this.authStore.currentUser()?.role === 'Admin');
    this.loadHolidays();
  }

  loadHolidays(): void {
    this.loading.set(true);
    this.holidayService.getHolidays().subscribe({
      next: (data) => {
        // Sort by date ascending
        this.holidays.set(data.sort((a, b) => a.holidayDate.localeCompare(b.holidayDate)));
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
    this.holidayForm.reset({
      name: '',
      holidayDate: '',
      description: '',
      isOptional: false
    });
    this.showModal.set(true);
  }

  openEditModal(holiday: Holiday): void {
    this.editingId = holiday.id;
    this.holidayForm.patchValue({
      name: holiday.name,
      holidayDate: holiday.holidayDate,
      description: holiday.description || '',
      isOptional: !!holiday.isOptional
    });
    this.showModal.set(true);
  }

  closeModal(): void {
    this.showModal.set(false);
    this.editingId = null;
    this.holidayForm.reset();
  }

  saveHoliday(): void {
    if (this.holidayForm.invalid) {
      this.holidayForm.markAllAsTouched();
      return;
    }

    const formVal = this.holidayForm.value;
    const dto: AddHoliday = {
      name: formVal.name!,
      holidayDate: formVal.holidayDate!,
      description: formVal.description || undefined,
      isOptional: !!formVal.isOptional
    };

    this.submitting.set(true);
    if (this.editingId) {
      this.holidayService.updateHoliday(this.editingId, dto as UpdateHoliday).subscribe({
        next: (msg) => {
          this.toastr.success(msg || 'Holiday updated successfully.');
          this.closeModal();
          this.loadHolidays();
          this.submitting.set(false);
        },
        error: (error) => {
          console.error(error);
          this.submitting.set(false);
        }
      });
    } else {
      this.holidayService.createHoliday(dto).subscribe({
        next: (msg) => {
          this.toastr.success(msg || 'Holiday created successfully.');
          this.closeModal();
          this.loadHolidays();
          this.submitting.set(false);
        },
        error: (error) => {
          console.error(error);
          this.submitting.set(false);
        }
      });
    }
  }

  deleteHoliday(id: string): void {
    if (confirm('Are you sure you want to delete this holiday?')) {
      this.holidayService.deleteHoliday(id).subscribe({
        next: (msg) => {
          this.toastr.success(msg || 'Holiday deleted successfully.');
          this.loadHolidays();
        },
        error: (error) => {
          console.error(error);
        }
      });
    }
  }
}
