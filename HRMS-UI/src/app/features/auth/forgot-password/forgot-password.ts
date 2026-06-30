import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './forgot-password.html',
  styleUrl: './forgot-password.css'
})
export class ForgotPassword {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);

  readonly loading = signal(false);
  readonly successMessage = signal<string | null>(null);
  readonly errorMessage = signal<string | null>(null);

  forgotForm = this.fb.group({
    email: ['', [Validators.required, Validators.email]]
  });

  submit() {
    if (this.forgotForm.invalid) {
      this.forgotForm.markAllAsTouched();
      return;
    }

    this.loading.set(true);
    this.successMessage.set(null);
    this.errorMessage.set(null);

    const email = this.forgotForm.value.email!;

    this.authService.forgotPassword(email).subscribe({
      next: (res) => {
        this.successMessage.set(res?.message || 'If an account exists with this email, a password reset link has been sent.');
        this.forgotForm.reset();
        this.loading.set(false);
      },
      error: (err) => {
        this.successMessage.set('If an account exists with this email, a password reset link has been sent.');
        this.forgotForm.reset();
        this.loading.set(false);
      }
    });
  }
}
