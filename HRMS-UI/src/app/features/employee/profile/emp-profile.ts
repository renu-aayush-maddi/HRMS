import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { forkJoin, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { EmployeeSelfService } from '../../../core/services/employee-self.service';
import { EmployeeProfile } from '../../../core/models/employee-profile.model';
import { HierarchyService, EmployeeNode, ManagerInfo } from '../../../core/services/hierarchy.service';

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
  LucideArrowDown
} from '@lucide/angular';

@Component({
  selector: 'app-emp-profile',
  standalone: true,
  imports: [
    CommonModule,
    LucideAlertTriangle,
    LucideMail,
    LucidePhone,
    LucideUser,
    LucideSchool,
    LucideGraduationCap,
    LucideFileText,
    LucideCheckCircle,
    LucideArrowDown
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

  constructor(
    private empService: EmployeeSelfService,
    private hierarchyService: HierarchyService
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
}

