import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators, AbstractControl, ValidationErrors } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

function passwordMatchValidator(control: AbstractControl): ValidationErrors | null {
  const password = control.get('newPassword');
  const confirmPassword = control.get('confirmPassword');
  if (password && confirmPassword && password.value !== confirmPassword.value) {
    return { passwordMismatch: true };
  }
  return null;
}

import {
  LucideEye,
  LucideEyeOff
} from '@lucide/angular';

@Component({
  selector: 'app-reset-password',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterLink,
    LucideEye,
    LucideEyeOff
  ],
  templateUrl: './reset-password.html',
  styleUrl: './reset-password.css'
})
export class ResetPassword implements OnInit {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);

  token: string | null = null;
  
  readonly isValidatingToken = signal(true);
  readonly isTokenValid = signal(false);
  readonly loading = signal(false);
  readonly success = signal(false);
  readonly errorMessage = signal<string | null>(null);

  readonly showPassword = signal(false);
  readonly showConfirmPassword = signal(false);

  resetForm = this.fb.group({
    newPassword: ['', [Validators.required, Validators.minLength(6)]],
    confirmPassword: ['', [Validators.required]]
  }, { validators: passwordMatchValidator });

  ngOnInit() {
    this.token = this.route.snapshot.queryParamMap.get('token');
    
    if (!this.token) {
      this.isValidatingToken.set(false);
      this.isTokenValid.set(false);
      this.errorMessage.set('Invalid or missing password reset token.');
      return;
    }

    this.authService.validateResetToken(this.token).subscribe({
      next: (res) => {
        this.isTokenValid.set(res.valid);
        if (!res.valid) {
          this.errorMessage.set('The password reset link is invalid, expired, or has already been used.');
        }
        this.isValidatingToken.set(false);
      },
      error: () => {
        this.isTokenValid.set(false);
        this.errorMessage.set('An error occurred while validating the token. Please try again.');
        this.isValidatingToken.set(false);
      }
    });
  }

  toggleShowPassword() {
    this.showPassword.update(v => !v);
  }

  toggleShowConfirmPassword() {
    this.showConfirmPassword.update(v => !v);
  }

  submit() {
    if (this.resetForm.invalid || !this.token) {
      this.resetForm.markAllAsTouched();
      return;
    }

    this.loading.set(true);
    this.errorMessage.set(null);

    const newPassword = this.resetForm.value.newPassword!;

    this.authService.resetPassword(this.token, newPassword).subscribe({
      next: () => {
        this.success.set(true);
        this.loading.set(false);
        setTimeout(() => {
          this.router.navigate(['/login']);
        }, 3000);
      },
      error: (err) => {
        this.errorMessage.set(err?.error?.message || 'Failed to reset password. Please try again.');
        this.loading.set(false);
      }
    });
  }
}
