import { Component, inject } from '@angular/core';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';

import { Router, RouterLink } from '@angular/router';

import { AuthService } from '../../../core/services/auth.service';
import { AuthStore } from '../../../stores/auth/auth.store';
import { ToastrService } from 'ngx-toastr';
import { CommonModule } from '@angular/common';

import {
  LucideMail,
  LucideKey,
  LucideEye,
  LucideArrowRight,
  LucideLoader
} from '@lucide/angular';

@Component({
  selector: 'app-login',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterLink,
    LucideMail,
    LucideKey,
    LucideEye,
    LucideArrowRight,
    LucideLoader
  ],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {

  private fb = inject(FormBuilder);

  private authService = inject(AuthService);

  private authStore = inject(AuthStore);

  private router = inject(Router);

  private toastr = inject(ToastrService);

  loading = false;

  loginForm = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required]]
  });

  login() {

    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.loading = true;

    this.authService
      .login(this.loginForm.getRawValue() as any)
      .subscribe({
        next: (response) => {

          localStorage.setItem(
            'token',
            response.token
          );

          this.authStore.setToken(
            response.token
          );

          this.authStore.setCurrentUser({
            userId: response.userId,
            employeeId: response.employeeId,
            email: response.email,
            role: response.role
          });

          switch (response.role) {

            case 'Admin':
              this.router.navigate([
                '/admin/dashboard'
              ]);
              break;

            case 'HR':
              this.router.navigate([
                '/hr/dashboard'
              ]);
              break;

            case 'Manager':
              this.router.navigate([
                '/manager/dashboard'
              ]);
              break;

            case 'Employee':
              this.router.navigate([
                '/employee/dashboard'
              ]);
              break;
          }
        },
        error: (error) => {
          console.error(error);
          this.toastr.error('Login Failed');
          this.loading = false;
        },
        complete: () => {
          this.loading = false;
        }
      });
  }
}