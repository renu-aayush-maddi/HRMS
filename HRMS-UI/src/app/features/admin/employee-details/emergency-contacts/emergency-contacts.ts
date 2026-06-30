import { Component, OnInit, signal, effect, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { EmployeeEmergencyContactService } from '../../../../core/services/employee-emergency-contact.service';
import { EmployeeDetailsStore } from '../../../../stores/employee/employee-details.store';
import { EmployeeEmergencyContact, AddEmployeeEmergencyContact } from '../../../../core/models/employee-subprofile.model';
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
  selector: 'app-emergency-contacts',
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
  templateUrl: './emergency-contacts.html',
  styleUrl: './emergency-contacts.css',
})
export class EmergencyContacts implements OnInit {
  readonly contacts = signal<EmployeeEmergencyContact[]>([]);
  readonly loading = signal(false);
  readonly submitting = signal(false);

  readonly pageNumber = signal(1);
  readonly pageSize = signal(10);
  readonly totalPages = signal(0);
  readonly totalRecords = signal(0);
  
  private toastr = inject(ToastrService);

  readonly showModal = signal(false);
  editingId: string | null = null;
  contactForm;
  readonly selectedFile = signal<File | null>(null);

  constructor(
    private contactService: EmployeeEmergencyContactService,
    public employeeStore: EmployeeDetailsStore,
    private fb: FormBuilder
  ) {
    this.contactForm = this.fb.group({
      contactName: ['', [Validators.required, Validators.maxLength(100)]],
      relationship: ['', [Validators.required, Validators.maxLength(50)]],
      phone: ['', [Validators.required, Validators.pattern(/^[0-9+\-()\s]{7,20}$/)]],
      email: ['', [Validators.required, Validators.email, Validators.maxLength(100)]]
    });

    // Auto-reload on employee changes
    effect(() => {
      const employeeId = this.employeeStore.selectedEmployeeId();
      this.pageNumber.set(1);
      this.loadContacts();
    });
  }

  ngOnInit(): void {
    // Rely on effect for load
  }

  loadContacts(): void {
    const employeeId = this.employeeStore.selectedEmployeeId();
    if (!employeeId) {
      this.contacts.set([]);
      this.totalPages.set(0);
      this.totalRecords.set(0);
      return;
    }

    this.loading.set(true);
    this.contactService.getEmergencyContacts(employeeId, this.pageNumber(), this.pageSize()).subscribe({
      next: (response) => {
        this.contacts.set(response.data);
        this.totalPages.set(response.totalPages);
        this.totalRecords.set(response.totalRecords);
        this.loading.set(false);
      },
      error: (err) => {
        console.error(err);
        this.loading.set(false);
      }
    });
  }

  previousPage(): void {
    if (this.pageNumber() <= 1) return;
    this.pageNumber.update(p => p - 1);
    this.loadContacts();
  }

  nextPage(): void {
    if (this.pageNumber() >= this.totalPages()) return;
    this.pageNumber.update(p => p + 1);
    this.loadContacts();
  }

  openAddModal(): void {
    this.editingId = null;
    this.contactForm.reset({
      contactName: '',
      relationship: '',
      phone: '',
      email: ''
    });
    this.showModal.set(true);
  }

  openEditModal(contact: EmployeeEmergencyContact): void {
    this.editingId = contact.id;
    this.contactForm.patchValue({
      contactName: contact.contactName,
      relationship: contact.relationship,
      phone: contact.phone,
      email: contact.email
    });
    this.showModal.set(true);
  }

  closeModal(): void {
    this.showModal.set(false);
    this.editingId = null;
    this.contactForm.reset();
  }

  saveContact(): void {
    if (this.contactForm.invalid) {
      this.contactForm.markAllAsTouched();
      return;
    }

    const employeeId = this.employeeStore.selectedEmployeeId();
    if (!employeeId) {
      this.toastr.warning('Please select an employee first.');
      return;
    }

    const formVal = this.contactForm.value;
    this.submitting.set(true);
    const request: AddEmployeeEmergencyContact = {
      employeeId: employeeId,
      contactName: formVal.contactName!,
      relationship: formVal.relationship!,
      phone: formVal.phone!,
      email: formVal.email!
    };

    if (this.editingId) {
      this.contactService.updateEmergencyContact(this.editingId, request).subscribe({
        next: () => {
          this.toastr.success('Emergency contact updated.');
          this.closeModal();
          this.loadContacts();
          this.submitting.set(false);
        },
        error: (err) => {
          console.error(err);
          this.submitting.set(false);
        }
      });
    } else {
      this.contactService.createEmergencyContact(request).subscribe({
        next: () => {
          this.toastr.success('Emergency contact added.');
          this.closeModal();
          this.loadContacts();
          this.submitting.set(false);
        },
        error: (err) => {
          console.error(err);
          this.submitting.set(false);
        }
      });
    }
  }

  deleteContact(id: string): void {
    if (confirm('Are you sure you want to delete this emergency contact?')) {
      this.contactService.deleteEmergencyContact(id).subscribe({
        next: () => {
          this.toastr.success('Emergency contact deleted.');
          this.loadContacts();
        },
        error: (err) => {
          console.error(err);
        }
      });
    }
  }

  exportContacts(): void {
    const employeeId = this.employeeStore.selectedEmployeeId();
    if (!employeeId) return;

    this.contactService.exportEmergencyContacts(employeeId).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `emergency_contacts_${employeeId.slice(0, 8)}.xlsx`;
        a.click();
        window.URL.revokeObjectURL(url);
        this.toastr.success('Emergency contacts list exported successfully.');
      },
      error: (err) => {
        console.error(err);
      }
    });
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.selectedFile.set(input.files[0]);
    }
  }

  importContacts(): void {
    if (!this.selectedFile()) {
      this.toastr.warning('Select an Excel file first.');
      return;
    }
    this.submitting.set(true);

    this.contactService.importEmergencyContacts(this.selectedFile()!).subscribe({
      next: () => {
        this.toastr.success('Import completed.');
        this.selectedFile.set(null);
        this.loadContacts();
        this.submitting.set(false);
      },
      error: (err) => {
        console.error(err);
        this.submitting.set(false);
      }
    });
  }
}
