import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ManagerService } from '../../../core/services/manager.service';
import { LeaveService } from '../../../core/services/leave.service';
import { PendingLeave } from '../../../core/models/manager.model';
import { LeaveAction } from '../../../core/models/leave.model';
import { ToastrService } from 'ngx-toastr';

import {
  LucideCheckCircle,
  LucideAlertTriangle,
  LucideCoffee,
  LucideX,
  LucideLoader
} from '@lucide/angular';

@Component({
  selector: 'app-manager-leave',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    LucideCheckCircle,
    LucideAlertTriangle,
    LucideCoffee,
    LucideX,
    LucideLoader
  ],
  templateUrl: './manager-leave.html',
  styleUrl: './manager-leave.css'
})
export class ManagerLeave implements OnInit {
  readonly pendingRequests = signal<PendingLeave[]>([]);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);
  readonly successMessage = signal<string | null>(null);

  // Modal State
  readonly selectedRequest = signal<PendingLeave | null>(null);
  readonly actionType = signal<'approve' | 'reject' | null>(null);
  readonly comments = signal('');
  readonly submitting = signal(false);

  private toastr = inject(ToastrService);

  constructor(
    private managerService: ManagerService,
    private leaveService: LeaveService
  ) {}

  ngOnInit(): void {
    this.loadPendingRequests();
  }

  loadPendingRequests(): void {
    this.loading.set(true);
    this.error.set(null);
    this.managerService.getPendingLeaves().subscribe({
      next: (data) => {
        this.pendingRequests.set(data);
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Error loading pending leave requests', err);
        this.error.set('Failed to load pending leave requests. Please try again.');
        this.loading.set(false);
      }
    });
  }

  openActionModal(request: PendingLeave, type: 'approve' | 'reject'): void {
    this.selectedRequest.set(request);
    this.actionType.set(type);
    this.comments.set('');
  }

  closeModal(): void {
    this.selectedRequest.set(null);
    this.actionType.set(null);
    this.comments.set('');
  }

  submitAction(): void {
    const selected = this.selectedRequest();
    const type = this.actionType();
    if (!selected || !type) return;

    this.submitting.set(true);
    const actionDto: LeaveAction = {
      managerComments: this.comments()
    };

    const action$ = type === 'approve'
      ? this.leaveService.approveLeave(selected.leaveId, actionDto)
      : this.leaveService.rejectLeave(selected.leaveId, actionDto);

    action$.subscribe({
      next: () => {
        this.toastr.success(`Leave request successfully ${type === 'approve' ? 'approved' : 'rejected'}.`);
        this.successMessage.set(`Leave request successfully ${type === 'approve' ? 'approved' : 'rejected'}.`);
        this.loadPendingRequests();
        this.closeModal();
        this.submitting.set(false);
        
        // Hide success alert after 3 seconds
        setTimeout(() => this.successMessage.set(null), 3000);
      },
      error: (err) => {
        console.error(`Error performing leave action ${type}`, err);
        this.submitting.set(false);
      }
    });
  }

  getLeaveDuration(fromDate: string, toDate: string): number {
    const start = new Date(fromDate).getTime();
    const end = new Date(toDate).getTime();
    return Math.round((end - start) / 86400000) + 1;
  }
}
