import { Component, OnInit, AfterViewInit, ElementRef, ViewChild, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HierarchyService, EmployeeNode } from '../../core/services/hierarchy.service';
import { OrgChart as D3OrgChart } from 'd3-org-chart';
import { AuthStore } from '../../stores/auth/auth.store';
import {
  LucideSearch,
  LucidePlus,
  LucideMinus,
  LucideRotateCcw,
  LucideUser,
  LucideUsers,
  LucideBuilding,
  LucideX
} from '@lucide/angular';

@Component({
  selector: 'app-org-chart',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    LucideSearch,
    LucidePlus,
    LucideMinus,
    LucideRotateCcw,
    LucideUser,
    LucideUsers,
    LucideBuilding,
    LucideX
  ],
  templateUrl: './org-chart.html',
  styleUrl: './org-chart.css'
})
export class OrgChartComponent implements OnInit {
  private hierarchyService = inject(HierarchyService);
  private authStore = inject(AuthStore);

  @ViewChild('chartContainer', { static: true }) chartContainer!: ElementRef;
  
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);
  
  chart: any = null;
  treeData: EmployeeNode[] = [];
  flatData: any[] = [];
  currentEmployeeId: string | null = null;

  searchQuery = '';
  searchResults: any[] = [];

  ngOnInit(): void {
    // Attempt to get employee ID of the logged in user
    this.currentEmployeeId = this.authStore.currentUser()?.employeeId || null;
    this.loadHierarchy();
  }

  loadHierarchy(): void {
    this.loading.set(true);
    this.error.set(null);
    this.hierarchyService.getTree().subscribe({
      next: (data) => {
        this.treeData = data;
        this.flatData = this.flattenTree(data);
        this.loading.set(false);
        
        // Initialize chart after data is loaded and DOM rendered
        setTimeout(() => {
          this.initChart();
        }, 100);
      },
      error: (err) => {
        this.error.set('Failed to load organization hierarchy data. Please try again.');
        this.loading.set(false);
      }
    });
  }

  flattenTree(tree: EmployeeNode[]): any[] {
    const result: any[] = [];
    const traverse = (node: EmployeeNode) => {
      result.push({
        id: node.id,
        parentId: node.managerId || '',
        name: node.name,
        employeeCode: node.employeeCode,
        firstName: node.firstName,
        lastName: node.lastName,
        email: node.email,
        designation: node.designation,
        departmentId: node.departmentId,
        departmentName: node.departmentName,
        employmentStatus: node.employmentStatus,
        profilePhotoUrl: node.profilePhotoUrl,
        directReportsCount: node.directReportsCount,
        warning: node.warning
      });
      if (node.children && node.children.length > 0) {
        node.children.forEach(traverse);
      }
    };
    tree.forEach(traverse);
    return result;
  }

  initChart(): void {
    if (!this.chartContainer || this.flatData.length === 0) return;

    // Clear container first
    this.chartContainer.nativeElement.innerHTML = '';

    this.chart = new D3OrgChart()
      .container(this.chartContainer.nativeElement)
      .data(this.flatData)
      .nodeWidth((d: any) => 260)
      .nodeHeight((d: any) => 140)
      .childrenMargin((d: any) => 50)
      .compactMarginBetween((d: any) => 35)
      .compactMarginPair((d: any) => 30)
      .nodeButtonWidth((d: any) => 36)
      .nodeButtonHeight((d: any) => 18)
      .nodeContent((d: any, i: number, arr: any[], state: any) => {
        return this.getNodeHtml(d.data);
      });

    this.chart.render();

    // Expand top elements
    this.chart.expandAll();
    this.chart.fit();

    // Center logged in employee if available
    if (this.currentEmployeeId) {
      const exists = this.flatData.some(e => e.id === this.currentEmployeeId);
      if (exists) {
        setTimeout(() => {
          this.centerOnEmployee(this.currentEmployeeId!);
        }, 300);
      }
    }
  }

  getNodeHtml(node: any): string {
    const initials = `${node.firstName?.charAt(0) ?? ''}${node.lastName?.charAt(0) ?? ''}`.toUpperCase();
    
    const photoHtml = node.profilePhotoUrl
      ? `<img src="${node.profilePhotoUrl}" class="node-photo" alt="${node.name}" style="width: 50px; height: 50px; border-radius: 50%; object-fit: cover;" />`
      : `<div class="node-avatar-initials" style="width: 50px; height: 50px; border-radius: 50%; background-color: var(--primary-light, #eef2ff); color: var(--primary, #4f46e5); display: flex; align-items: center; justify-content: center; font-weight: 600; font-size: 16px; border: 2px solid var(--border-color, #e5e7eb);">${initials}</div>`;
      
    const warningHtml = node.warning
      ? `<div class="node-warning" title="${node.warning}" style="position: absolute; top: -5px; right: -5px; background: #ef4444; color: white; border-radius: 50%; width: 20px; height: 20px; display: flex; align-items: center; justify-content: center; font-size: 11px; font-weight: bold; box-shadow: 0 2px 4px rgba(0,0,0,0.1);"><svg xmlns="http://www.w3.org/2000/svg" width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"><path d="m21.73 18-8-14a2 2 0 0 0-3.48 0l-8 14A2 2 0 0 0 4 21h16a2 2 0 0 0 1.73-3Z"/><line x1="12" y1="9" x2="12" y2="13"/><line x1="12" y1="17" x2="12.01" y2="17"/></svg></div>`
      : '';

    const isCurrentUser = node.id === this.currentEmployeeId ? 'current-user-card' : '';

    return `
      <div class="node-card ${isCurrentUser}" style="padding: 12px; background: white; border-radius: 12px; border: 1px solid var(--border-color, #e5e7eb); box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.05); font-family: inherit; width: 260px; box-sizing: border-box; position: relative;">
        <div class="node-card-body" style="display: flex; gap: 12px; align-items: center;">
          <div class="node-avatar-container" style="position: relative; flex-shrink: 0;">
            ${photoHtml}
            ${warningHtml}
          </div>
          <div class="node-details" style="min-width: 0; flex-grow: 1;">
            <div class="node-name" style="font-weight: 600; color: #1f2937; font-size: 14px; white-space: nowrap; overflow: hidden; text-overflow: ellipsis; line-height: 1.2;" title="${node.name}">${node.name}</div>
            <div class="node-code" style="font-size: 11px; color: #9ca3af; margin-bottom: 2px;">ID: ${node.employeeCode}</div>
            <div class="node-designation" style="font-size: 12px; color: #4b5563; white-space: nowrap; overflow: hidden; text-overflow: ellipsis; line-height: 1.2;" title="${node.designation || ''}">${node.designation || 'Staff'}</div>
            <div class="node-department" style="font-size: 11px; color: #6b7280; font-weight: 500; white-space: nowrap; overflow: hidden; text-overflow: ellipsis;">${node.departmentName || 'General'}</div>
          </div>
        </div>
        <div class="node-card-footer" style="margin-top: 10px; padding-top: 8px; border-top: 1px solid #f3f4f6; display: flex; justify-content: space-between; align-items: center;">
          <span class="status-badge ${node.employmentStatus?.toLowerCase()}" style="font-size: 10px; font-weight: 600; padding: 2px 6px; border-radius: 9999px;">
            ${node.employmentStatus || 'Active'}
          </span>
          ${node.directReportsCount > 0 
            ? `<span class="reports-count" style="font-size: 11px; color: #4f46e5; font-weight: 600; background: #eef2ff; padding: 2px 6px; border-radius: 4px;">${node.directReportsCount} reports</span>` 
            : ''
          }
        </div>
      </div>
    `;
  }

  onSearchInput(): void {
    if (!this.searchQuery.trim()) {
      this.searchResults = [];
      return;
    }
    const q = this.searchQuery.toLowerCase();
    this.searchResults = this.flatData.filter(emp => 
      emp.name.toLowerCase().includes(q) ||
      emp.employeeCode.toLowerCase().includes(q) ||
      (emp.designation && emp.designation.toLowerCase().includes(q)) ||
      (emp.departmentName && emp.departmentName.toLowerCase().includes(q))
    ).slice(0, 5);
  }

  selectEmployee(emp: any): void {
    this.searchQuery = emp.name;
    this.searchResults = [];
    this.centerOnEmployee(emp.id);
  }

  centerOnEmployee(id: string): void {
    if (!this.chart) return;
    
    // Clear previous highlight
    this.chart.setHighlighted(id);
    this.chart.setUpToTheRootHighlighted(id);
    
    // Expand to node
    this.chart.zoomToNode(id);
    this.chart.render();
  }

  clearSearch(): void {
    this.searchQuery = '';
    this.searchResults = [];
  }

  zoomIn(): void {
    if (this.chart) this.chart.zoomIn();
  }

  zoomOut(): void {
    if (this.chart) this.chart.zoomOut();
  }

  resetChart(): void {
    if (this.chart) {
      this.chart.fit();
    }
  }

  centerMe(): void {
    if (this.currentEmployeeId) {
      this.centerOnEmployee(this.currentEmployeeId);
    }
  }

  expandAll(): void {
    if (this.chart) {
      this.chart.expandAll().render();
    }
  }

  collapseAll(): void {
    if (this.chart) {
      this.chart.collapseAll().render();
    }
  }
}
