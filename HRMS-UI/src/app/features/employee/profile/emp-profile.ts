import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { forkJoin, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { EmployeeSelfService } from '../../../core/services/employee-self.service';
import { EmployeeProfile } from '../../../core/models/employee-profile.model';
import { HierarchyService, EmployeeNode, ManagerInfo } from '../../../core/services/hierarchy.service';
import { ToastrService } from 'ngx-toastr';
import { AuthStore } from '../../../stores/auth/auth.store';
import { environment } from '../../../../environments/environment';

type ProfileTab = 'overview' | 'addresses' | 'education' | 'experience' | 'emergency' | 'documents' | 'org';

import {
  LucideAlertTriangle,
  LucideMail,
  LucidePhone,
  LucideUser,
  LucideSchool,
  LucideGraduationCap,
  LucideFileText,
  LucideCheckCircle,
  LucideArrowDown,
  LucideCamera,
  LucideTrash2,
  LucideEdit3,
  LucideX
} from '@lucide/angular';

@Component({
  selector: 'app-emp-profile',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    LucideAlertTriangle,
    LucideMail,
    LucidePhone,
    LucideUser,
    LucideSchool,
    LucideGraduationCap,
    LucideFileText,
    LucideCheckCircle,
    LucideArrowDown,
    LucideCamera,
    LucideTrash2,
    LucideEdit3,
    LucideX
  ],
  templateUrl: './emp-profile.html',
  styleUrl: './emp-profile.css',
})
export class EmpProfile implements OnInit {
  readonly profile = signal<EmployeeProfile | null>(null);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);
  readonly activeTab = signal<ProfileTab>('overview');

  // Org tab signals
  readonly reportingChain = signal<EmployeeNode[]>([]);
  readonly managerInfo = signal<ManagerInfo | null>(null);
  readonly directReports = signal<EmployeeNode[]>([]);
  readonly loadingOrg = signal(false);
  readonly orgError = signal<string | null>(null);

  // Profile photo & editing signals
  readonly showPreviewModal = signal(false);
  readonly uploading = signal(false);
  readonly isEditingPhone = signal(false);
  readonly phoneInput = signal('');
  readonly savingPhone = signal(false);

  constructor(
    private empService: EmployeeSelfService,
    private hierarchyService: HierarchyService,
    private toastr: ToastrService,
    private authStore: AuthStore
  ) {}

  ngOnInit(): void {
    this.loadProfile();
  }

  loadProfile(): void {
    this.loading.set(true);
    this.empService.getMyProfile().subscribe({
      next: (data) => {
        this.profile.set(data);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Failed to load your profile. Please try again.');
        this.loading.set(false);
      }
    });
  }

  setTab(tab: ProfileTab): void {
    this.activeTab.set(tab);
    if (tab === 'org') {
      this.loadOrgHierarchy();
    }
  }

  loadOrgHierarchy(): void {
    const prof = this.profile();
    if (!prof) return;

    this.loadingOrg.set(true);
    this.orgError.set(null);

    forkJoin({
      chain: this.hierarchyService.getReportingChain(prof.id),
      manager: this.hierarchyService.getManagerInfo(prof.id).pipe(catchError(() => of(null))),
      reports: this.hierarchyService.getDirectReports(prof.id)
    }).subscribe({
      next: (res) => {
        this.reportingChain.set(res.chain);
        this.managerInfo.set(res.manager);
        this.directReports.set(res.reports);
        this.loadingOrg.set(false);
      },
      error: () => {
        this.orgError.set('Failed to load your organizational hierarchy.');
        this.loadingOrg.set(false);
      }
    });
  }

  getInitials(): string {
    const prof = this.profile();
    if (!prof) return '';
    return `${prof.firstName?.charAt(0) ?? ''}${prof.lastName?.charAt(0) ?? ''}`.toUpperCase();
  }

  getInitialsForName(firstName: string, lastName: string): string {
    return `${firstName?.charAt(0) ?? ''}${lastName?.charAt(0) ?? ''}`.toUpperCase();
  }

  getFullImageUrl(url: string | null | undefined): string | null {
    if (!url) return null;
    if (url.startsWith('http://') || url.startsWith('https://') || url.startsWith('data:')) {
      return url;
    }
    const baseUrl = environment.apiUrl.replace('/api', '');
    return `${baseUrl}${url}`;
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (!input.files || input.files.length === 0) return;

    const file = input.files[0];

    // Validate size (max 5 MB)
    if (file.size > 5 * 1024 * 1024) {
      this.toastr.error('File size cannot exceed 5 MB.');
      return;
    }

    // Validate extension
    const allowed = ['.jpg', '.jpeg', '.png', '.webp'];
    const ext = file.name.substring(file.name.lastIndexOf('.')).toLowerCase();
    if (!allowed.includes(ext)) {
      this.toastr.error('Invalid file type. Only JPG, JPEG, PNG, and WEBP files are allowed.');
      return;
    }

    this.uploading.set(true);
    this.empService.uploadProfilePhoto(file).subscribe({
      next: (res) => {
        this.toastr.success('Profile photo updated successfully!');
        this.uploading.set(false);
        this.loadProfile();

        const user = this.authStore.currentUser();
        if (user) {
          this.authStore.setCurrentUser({
            ...user,
            profilePhotoUrl: res.profilePhotoUrl
          });
        }
      },
      error: (err) => {
        console.error(err);
        this.toastr.error(err.error?.message || 'Failed to upload profile photo.');
        this.uploading.set(false);
      }
    });
  }

  deletePhoto(event: Event): void {
    event.stopPropagation();
    if (!confirm('Are you sure you want to delete your profile photo?')) return;

    this.empService.deleteProfilePhoto().subscribe({
      next: () => {
        this.toastr.success('Profile photo deleted successfully!');
        this.loadProfile();

        const user = this.authStore.currentUser();
        if (user) {
          this.authStore.setCurrentUser({
            ...user,
            profilePhotoUrl: null
          });
        }
      },
      error: (err) => {
        console.error(err);
        this.toastr.error('Failed to delete profile photo.');
      }
    });
  }

  startEditPhone(): void {
    const currentPhone = this.profile()?.phone || '';
    this.phoneInput.set(currentPhone);
    this.isEditingPhone.set(true);
  }

  cancelEditPhone(): void {
    this.isEditingPhone.set(false);
  }

  savePhone(): void {
    const phone = this.phoneInput().trim();
    this.savingPhone.set(true);
    this.empService.updateMyProfile({ phone }).subscribe({
      next: () => {
        this.toastr.success('Phone number updated successfully!');
        this.isEditingPhone.set(false);
        this.savingPhone.set(false);
        this.loadProfile();
      },
      error: (err) => {
        console.error(err);
        this.toastr.error(err.error?.message || 'Failed to update phone number.');
        this.savingPhone.set(false);
      }
    });
  }

  openPreviewModal(): void {
    if (this.profile()?.profilePhotoUrl) {
      this.showPreviewModal.set(true);
    }
  }

  closePreviewModal(): void {
    this.showPreviewModal.set(false);
  }
}

