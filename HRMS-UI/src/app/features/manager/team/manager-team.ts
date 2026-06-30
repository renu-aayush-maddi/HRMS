import { Component, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { ManagerService } from '../../../core/services/manager.service';
import { TeamMember } from '../../../core/models/manager.model';
import { 
  LucidePlus, 
  LucideTrash2, 
  LucideSearch, 
  LucideChevronLeft, 
  LucideChevronRight, 
  LucideX, 
  LucideAlertTriangle,
  LucideLoader,
  LucideTarget,
  LucideFileText
} from '@lucide/angular';

@Component({
  selector: 'app-manager-team',
  standalone: true,
  imports: [
    CommonModule, 
    FormsModule, 
    RouterModule,
    LucidePlus, 
    LucideTrash2, 
    LucideSearch, 
    LucideChevronLeft, 
    LucideChevronRight, 
    LucideX, 
    LucideAlertTriangle,
    LucideLoader,
    LucideTarget,
    LucideFileText
  ],
  templateUrl: './manager-team.html',
  styleUrl: './manager-team.css'
})
export class ManagerTeam implements OnInit {
  readonly team = signal<TeamMember[]>([]);
  readonly filteredTeam = signal<TeamMember[]>([]);
  readonly selectedMember = signal<TeamMember | null>(null);
  
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);
  
  // Search & Filter of current team
  readonly searchQuery = signal('');
  readonly statusFilter = signal('');

  // Add Member Modal State
  readonly showAddModal = signal(false);
  readonly eligibleEmployees = signal<any[]>([]);
  readonly eligibleTotalCount = signal(0);
  readonly eligibleSearch = signal('');
  readonly eligiblePage = signal(1);
  readonly eligiblePageSize = 5;
  readonly eligibleLoading = signal(false);
  readonly eligibleError = signal<string | null>(null);
  readonly assigningEmployeeId = signal<string | null>(null);

  readonly totalPages = computed(() => Math.ceil(this.eligibleTotalCount() / this.eligiblePageSize));

  // Remove Member State
  readonly showRemoveConfirmModal = signal(false);
  readonly employeeToRemove = signal<TeamMember | null>(null);
  readonly removingEmployeeId = signal<string | null>(null);

  // Toast Alerts
  readonly toastMessage = signal<string | null>(null);
  readonly toastType = signal<'success' | 'error'>('success');

  constructor(private managerService: ManagerService) {}

  ngOnInit(): void {
    this.loadTeam();
  }

  showToast(message: string, type: 'success' | 'error' = 'success'): void {
    this.toastMessage.set(message);
    this.toastType.set(type);
    setTimeout(() => {
      this.toastMessage.set(null);
    }, 4000);
  }

  loadTeam(): void {
    this.loading.set(true);
    this.error.set(null);
    this.managerService.getTeamMembers().subscribe({
      next: (data) => {
        this.team.set(data);
        this.applyFilters();
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Error loading team members', err);
        this.error.set('Failed to load team directory. Please try again.');
        this.loading.set(false);
      }
    });
  }

  applyFilters(): void {
    let result = [...this.team()];

    if (this.searchQuery().trim()) {
      const q = this.searchQuery().toLowerCase();
      result = result.filter(m => 
        m.fullName.toLowerCase().includes(q) || 
        m.employeeCode.toLowerCase().includes(q) || 
        m.designation.toLowerCase().includes(q)
      );
    }

    if (this.statusFilter()) {
      result = result.filter(m => m.employmentStatus === this.statusFilter());
    }

    this.filteredTeam.set(result);
  }

  viewDetails(member: TeamMember): void {
    this.selectedMember.set(member);
  }

  closeDrawer(): void {
    this.selectedMember.set(null);
  }

  // Add Member Modal Functions
  openAddModal(): void {
    this.eligibleSearch.set('');
    this.eligiblePage.set(1);
    this.eligibleError.set(null);
    this.showAddModal.set(true);
    this.loadEligibleEmployees();
  }

  closeAddModal(): void {
    this.showAddModal.set(false);
  }

  loadEligibleEmployees(): void {
    this.eligibleLoading.set(true);
    this.eligibleError.set(null);
    this.managerService.getEligibleEmployees(
      this.eligibleSearch(),
      this.eligiblePage(),
      this.eligiblePageSize
    ).subscribe({
      next: (res) => {
        this.eligibleEmployees.set(res.employees);
        this.eligibleTotalCount.set(res.totalCount);
        this.eligibleLoading.set(false);
      },
      error: (err) => {
        console.error('Error fetching eligible employees', err);
        this.eligibleError.set('Failed to search employees. Please try again.');
        this.eligibleLoading.set(false);
      }
    });
  }

  onEligibleSearchChange(query: string): void {
    this.eligibleSearch.set(query);
    this.eligiblePage.set(1);
    this.loadEligibleEmployees();
  }

  eligiblePrevPage(): void {
    if (this.eligiblePage() > 1) {
      this.eligiblePage.update(p => p - 1);
      this.loadEligibleEmployees();
    }
  }

  eligibleNextPage(): void {
    if (this.eligiblePage() * this.eligiblePageSize < this.eligibleTotalCount()) {
      this.eligiblePage.update(p => p + 1);
      this.loadEligibleEmployees();
    }
  }

  addEmployee(employee: any): void {
    const isTransfer = !!employee.currentManagerName;
    const confirmMessage = isTransfer
      ? `This employee is currently managed by ${employee.currentManagerName}. Assigning them to your team will transfer them. Proceed?`
      : `Assign ${employee.fullName} to your team?`;

    if (!confirm(confirmMessage)) {
      return;
    }

    this.assigningEmployeeId.set(employee.id);
    this.managerService.addTeamMember(employee.id).subscribe({
      next: (res) => {
        this.showToast(res?.message || 'Employee added successfully!');
        this.loadTeam(); // Reload team directory
        this.loadEligibleEmployees(); // Reload lookup list
        this.assigningEmployeeId.set(null);
      },
      error: (err) => {
        console.error('Error adding employee', err);
        const errMsg = err?.error?.message || 'Failed to add employee to team.';
        this.showToast(errMsg, 'error');
        this.assigningEmployeeId.set(null);
      }
    });
  }

  // Remove Member Functions
  openRemoveConfirm(member: TeamMember): void {
    this.employeeToRemove.set(member);
    this.showRemoveConfirmModal.set(true);
  }

  closeRemoveConfirm(): void {
    this.showRemoveConfirmModal.set(false);
    this.employeeToRemove.set(null);
  }

  confirmRemoveEmployee(): void {
    const member = this.employeeToRemove();
    if (!member) return;

    this.removingEmployeeId.set(member.id);
    this.managerService.removeTeamMember(member.id).subscribe({
      next: (res) => {
        this.showToast(res?.message || 'Employee removed successfully.');
        this.loadTeam();
        this.closeRemoveConfirm();
        this.removingEmployeeId.set(null);
      },
      error: (err) => {
        console.error('Error removing employee', err);
        const errMsg = err?.error?.message || 'Failed to remove employee.';
        this.showToast(errMsg, 'error');
        this.removingEmployeeId.set(null);
      }
    });
  }
}
