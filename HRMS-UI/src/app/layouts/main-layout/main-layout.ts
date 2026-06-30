import { Component, inject, signal } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators, AbstractControl, ValidationErrors } from '@angular/forms';

import { Sidebar } from '../../shared/components/sidebar/sidebar';
import { AuthStore } from '../../stores/auth/auth.store';
import { AuthService } from '../../core/services/auth.service';
import { NotificationService } from '../../core/services/notification.service';
import { LucideMenu, LucideLogOut, LucideBell, LucideBellOff, LucideKey, LucideX, LucideEye, LucideEyeOff } from '@lucide/angular';

function passwordMatchValidatorChange(control: AbstractControl): ValidationErrors | null {
  const password = control.get('newPassword');
  const confirmPassword = control.get('confirmPassword');
  if (password && confirmPassword && password.value !== confirmPassword.value) {
    return { passwordMismatch: true };
  }
  return null;
}

@Component({
  selector: 'app-main-layout',
  imports: [
    CommonModule,
    RouterOutlet,
    Sidebar,
    LucideMenu,
    LucideLogOut,
    LucideBell,
    LucideBellOff,
    LucideKey,
    LucideX,
    LucideEye,
    LucideEyeOff,
    ReactiveFormsModule
  ],
  templateUrl: './main-layout.html',
  styleUrl: './main-layout.css'
})
export class MainLayout {
  authStore = inject(AuthStore);
  notificationService = inject(NotificationService);
  private authService = inject(AuthService);
  private router = inject(Router);
  private fb = inject(FormBuilder);

  readonly isSidebarCollapsed = signal(false);
  readonly isMobileMenuOpen = signal(false);
  readonly isNotificationOpen = signal(false);

  // Change Password state
  readonly showChangePasswordModal = signal(false);
  readonly changingPassword = signal(false);
  readonly changeSuccessMessage = signal<string | null>(null);
  readonly changeErrorMessage = signal<string | null>(null);

  readonly showCurrentPassword = signal(false);
  readonly showNewPassword = signal(false);
  readonly showConfirmPassword = signal(false);

  changePasswordForm = this.fb.group({
    currentPassword: ['', [Validators.required]],
    newPassword: ['', [Validators.required, Validators.minLength(6)]],
    confirmPassword: ['', [Validators.required]]
  }, { validators: passwordMatchValidatorChange });

  toggleSidebar(): void {
    this.isSidebarCollapsed.update(val => !val);
  }

  toggleMobileMenu(): void {
    this.isMobileMenuOpen.update(val => !val);
  }

  closeMobileMenu(): void {
    this.isMobileMenuOpen.set(false);
  }

  toggleNotification(): void {
    this.isNotificationOpen.update(val => !val);
  }

  markAsRead(id: string): void {
    this.notificationService.markAsReadLocal(id);
  }

  formatTime(dateStr: string): string {
    if (!dateStr) return '';
    const date = new Date(dateStr);
    const now = new Date();
    const diffMs = now.getTime() - date.getTime();
    
    if (diffMs < 0) return 'Just now';
    
    const diffMins = Math.round(diffMs / 60000);
    if (diffMins < 1) return 'Just now';
    if (diffMins < 60) return `${diffMins}m ago`;
    
    const diffHours = Math.round(diffMins / 60);
    if (diffHours < 24) return `${diffHours}h ago`;
    
    return date.toLocaleDateString(undefined, { month: 'short', day: 'numeric' });
  }

  openChangePasswordModal() {
    this.changePasswordForm.reset();
    this.changeSuccessMessage.set(null);
    this.changeErrorMessage.set(null);
    this.showCurrentPassword.set(false);
    this.showNewPassword.set(false);
    this.showConfirmPassword.set(false);
    this.showChangePasswordModal.set(true);
  }

  closeChangePasswordModal() {
    this.showChangePasswordModal.set(false);
  }

  submitChangePassword() {
    if (this.changePasswordForm.invalid) {
      this.changePasswordForm.markAllAsTouched();
      return;
    }

    this.changingPassword.set(true);
    this.changeErrorMessage.set(null);
    this.changeSuccessMessage.set(null);

    const currentPassword = this.changePasswordForm.value.currentPassword!;
    const newPassword = this.changePasswordForm.value.newPassword!;

    this.authService.changePassword(currentPassword, newPassword).subscribe({
      next: (res) => {
        this.changeSuccessMessage.set(res?.message || 'Password changed successfully!');
        this.changingPassword.set(false);
        setTimeout(() => {
          this.closeChangePasswordModal();
        }, 2000);
      },
      error: (err) => {
        this.changeErrorMessage.set(err?.error?.message || 'Failed to change password. Please check your current password.');
        this.changingPassword.set(false);
      }
    });
  }

  logout(): void {
    localStorage.removeItem('token');
    this.authStore.clear();
    this.router.navigate(['/login']);
  }
}