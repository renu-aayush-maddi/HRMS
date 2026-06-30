import { Component, OnInit, signal, effect, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { EmployeeAddressService } from '../../../../core/services/employee-address.service';
import { EmployeeDetailsStore } from '../../../../stores/employee/employee-details.store';
import { EmployeeAddress } from '../../../../core/models/employee-address.model';
import { AddEmployeeAddress } from '../../../../core/models/add-employee-address.model';
import { UpdateEmployeeAddress } from '../../../../core/models/update-employee-address.model';
import { ToastrService } from 'ngx-toastr';

import {
  LucideUpload,
  LucideDownload,
  LucidePlus,
  LucideEdit,
  LucideTrash2,
  LucideChevronLeft,
  LucideChevronRight,
  LucideX,
  LucideLoader
} from '@lucide/angular';

@Component({
  selector: 'app-addresses',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    LucideUpload,
    LucideDownload,
    LucidePlus,
    LucideEdit,
    LucideTrash2,
    LucideChevronLeft,
    LucideChevronRight,
    LucideX,
    LucideLoader
  ],
  templateUrl: './addresses.html',
  styleUrl: './addresses.css'
})
export class Addresses implements OnInit {
  readonly addresses = signal<EmployeeAddress[]>([]);
  readonly loading = signal(false);
  readonly submitting = signal(false);

  readonly pageNumber = signal(1);
  readonly pageSize = signal(10);
  
  private toastr = inject(ToastrService);
  readonly totalPages = signal(0);
  readonly totalRecords = signal(0);

  readonly showModal = signal(false);
  editingId: string | null = null;
  addressForm;
  readonly selectedFile = signal<File | null>(null);

  addressTypes = ['Home', 'Office', 'Permanent', 'Current'];

  constructor(
    private addressService: EmployeeAddressService,
    public employeeStore: EmployeeDetailsStore,
    private fb: FormBuilder
  ) {
    this.addressForm = this.fb.group({
      addressType: ['Home', Validators.required],
      addressLine1: ['', [Validators.required, Validators.maxLength(200)]],
      addressLine2: ['', [Validators.maxLength(200)]],
      city: ['', [Validators.required, Validators.maxLength(100)]],
      state: ['', [Validators.required, Validators.maxLength(100)]],
      country: ['', [Validators.required, Validators.maxLength(100)]],
      postalCode: ['', [Validators.required, Validators.maxLength(20)]]
    });

    // Auto-reload on employee changes
    effect(() => {
      const employeeId = this.employeeStore.selectedEmployeeId();
      this.pageNumber.set(1);
      this.loadAddresses();
    });
  }

  ngOnInit(): void {
    // Rely on effect for load
  }

  loadAddresses(): void {
    const employeeId = this.employeeStore.selectedEmployeeId();
    if (!employeeId) {
      this.addresses.set([]);
      this.totalPages.set(0);
      this.totalRecords.set(0);
      return;
    }

    this.loading.set(true);
    this.addressService
      .getAddresses(employeeId, this.pageNumber(), this.pageSize())
      .subscribe({
        next: response => {
          this.addresses.set(response.data);
          this.totalPages.set(response.totalPages);
          this.totalRecords.set(response.totalRecords);
          this.loading.set(false);
        },
        error: error => {
          console.error(error);
          this.loading.set(false);
        }
      });
  }

  previousPage(): void {
    if (this.pageNumber() <= 1) return;
    this.pageNumber.update(p => p - 1);
    this.loadAddresses();
  }

  nextPage(): void {
    if (this.pageNumber() >= this.totalPages()) return;
    this.pageNumber.update(p => p + 1);
    this.loadAddresses();
  }

  openAddModal(): void {
    this.editingId = null;
    this.addressForm.reset({
      addressType: 'Home',
      addressLine1: '',
      addressLine2: '',
      city: '',
      state: '',
      country: '',
      postalCode: ''
    });
    this.showModal.set(true);
  }

  openEditModal(address: EmployeeAddress): void {
    this.editingId = address.id;
    this.addressForm.patchValue({
      addressType: address.addressType,
      addressLine1: address.addressLine1,
      addressLine2: address.addressLine2 || '',
      city: address.city,
      state: address.state,
      country: address.country,
      postalCode: address.postalCode
    });
    this.showModal.set(true);
  }

  closeModal(): void {
    this.showModal.set(false);
    this.editingId = null;
    this.addressForm.reset();
  }

  saveAddress(): void {
    if (this.addressForm.invalid) {
      this.addressForm.markAllAsTouched();
      return;
    }

    const employeeId = this.employeeStore.selectedEmployeeId();
    if (!employeeId) {
      this.toastr.warning('Please select an employee first.');
      return;
    }

    const formVal = this.addressForm.value;
    this.submitting.set(true);

    if (this.editingId) {
      const request: UpdateEmployeeAddress = {
        addressLine1: formVal.addressLine1!,
        addressLine2: formVal.addressLine2 || undefined,
        city: formVal.city!,
        state: formVal.state!,
        country: formVal.country!,
        postalCode: formVal.postalCode!,
        addressType: formVal.addressType!
      };

      this.addressService.updateAddress(this.editingId, request).subscribe({
        next: () => {
          this.toastr.success('Address updated successfully.');
          this.closeModal();
          this.loadAddresses();
          this.submitting.set(false);
        },
        error: error => {
          console.error(error);
          this.submitting.set(false);
        }
      });
    } else {
      const request: AddEmployeeAddress = {
        employeeId: employeeId,
        addressLine1: formVal.addressLine1!,
        addressLine2: formVal.addressLine2 || undefined,
        city: formVal.city!,
        state: formVal.state!,
        country: formVal.country!,
        postalCode: formVal.postalCode!,
        addressType: formVal.addressType!
      };

      this.addressService.createAddress(request).subscribe({
        next: () => {
          this.toastr.success('Address added successfully.');
          this.closeModal();
          this.loadAddresses();
          this.submitting.set(false);
        },
        error: error => {
          console.error(error);
          this.submitting.set(false);
        }
      });
    }
  }

  deleteAddress(id: string): void {
    if (confirm('Are you sure you want to delete this address?')) {
      this.addressService.deleteAddress(id).subscribe({
        next: () => {
          this.toastr.success('Address deleted successfully.');
          this.loadAddresses();
        },
        error: error => {
          console.error(error);
        }
      });
    }
  }

  exportAddresses(): void {
    const employeeId = this.employeeStore.selectedEmployeeId();
    if (!employeeId) return;

    this.addressService.exportAddresses(employeeId).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `employee_addresses_${employeeId.slice(0, 8)}.xlsx`;
        a.click();
        window.URL.revokeObjectURL(url);
        this.toastr.success('Addresses list exported successfully.');
      },
      error: error => {
        console.error(error);
      }
    });
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.selectedFile.set(input.files[0]);
    }
  }

  importAddresses(): void {
    if (!this.selectedFile()) {
      this.toastr.warning('Please select an Excel file first.');
      return;
    }
    this.submitting.set(true);

    this.addressService.importAddresses(this.selectedFile()!).subscribe({
      next: () => {
        this.toastr.success('Import completed successfully.');
        this.selectedFile.set(null);
        this.loadAddresses();
        this.submitting.set(false);
      },
      error: error => {
        console.error(error);
        this.submitting.set(false);
      }
    });
  }
}