import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { PerformanceBonusService } from '../../core/services/performance-bonus.service';
import { PerformanceCycleService } from '../../core/services/performance-cycle.service';
import { 
  PerformanceBonusRuleResponse, AddPerformanceBonusRule,
  PerformanceBonusRecommendationResponse, HrDashboardResponse
} from '../../core/models/performance-bonus.model';
import { PerformanceCycle } from '../../core/models/performance-cycle.model';
import { ToastrService } from 'ngx-toastr';

import {
  LucideAlertTriangle,
  LucideStar,
  LucideBuilding2,
  LucidePlus,
  LucideRefreshCw,
  LucideX,
  LucideLoader
} from '@lucide/angular';

@Component({
  selector: 'app-performance-bonus',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    LucideAlertTriangle,
    LucideStar,
    LucideBuilding2,
    LucidePlus,
    LucideRefreshCw,
    LucideX,
    LucideLoader
  ],
  templateUrl: './performance-bonus.html',
  styleUrl: './performance-bonus.css'
})
export class PerformanceBonus implements OnInit {
  readonly activeSubTab = signal<'dashboard' | 'rules' | 'recommendations'>('dashboard');
  readonly loading = signal(false);
  readonly submitting = signal(false);

  // Cycles drop down list
  readonly cycles = signal<PerformanceCycle[]>([]);
  readonly selectedCycleId = signal('');
  
  private toastr = inject(ToastrService);

  // Dashboard state
  readonly dashboardData = signal<HrDashboardResponse | null>(null);

  // Rules state
  readonly rules = signal<PerformanceBonusRuleResponse[]>([]);
  readonly showRuleModal = signal(false);
  readonly isRuleEdit = signal(false);
  selectedRuleId: string | null = null;
  ruleForm;

  // Recommendations state
  readonly recommendations = signal<PerformanceBonusRecommendationResponse[]>([]);
  readonly showRecModal = signal(false);
  selectedRecId = '';
  recForm;

  constructor(
    private pbService: PerformanceBonusService,
    private cycleService: PerformanceCycleService,
    private fb: FormBuilder
  ) {
    this.ruleForm = this.fb.group({
      minRating: [1, [Validators.required, Validators.min(1), Validators.max(5)]],
      maxRating: [5, [Validators.required, Validators.min(1), Validators.max(5)]],
      bonusPercentage: [0, [Validators.required, Validators.min(0)]]
    });

    this.recForm = this.fb.group({
      approvedAmount: [0, [Validators.required, Validators.min(0)]],
      remarks: ['', [Validators.maxLength(300)]]
    });
  }

  ngOnInit(): void {
    this.loadCycles();
    this.loadRules();
  }

  setSubTab(tab: 'dashboard' | 'rules' | 'recommendations'): void {
    this.activeSubTab.set(tab);
    if (tab === 'dashboard') {
      this.loadDashboard();
    } else if (tab === 'rules') {
      this.loadRules();
    } else if (tab === 'recommendations') {
      this.loadRecommendations();
    }
  }

  loadCycles(): void {
    this.cycleService.getCycles().subscribe({
      next: (res: PerformanceCycle[]) => {
        this.cycles.set(res);
        if (this.cycles().length > 0) {
          this.selectedCycleId.set(this.cycles()[0].id);
          this.loadDashboard();
        }
      },
      error: (err: any) => {
        console.warn('Could not load performance cycles (might be restricted).', err);
      }
    });
  }

  // Dashboard Loader
  loadDashboard(): void {
    if (!this.selectedCycleId()) return;
    this.loading.set(true);
    this.pbService.getHrDashboard(this.selectedCycleId()).subscribe({
      next: (res: HrDashboardResponse) => {
        this.dashboardData.set(res);
        this.loading.set(false);
      },
      error: (err: any) => {
        console.error(err);
        this.loading.set(false);
      }
    });
  }

  // Rules CRUD
  loadRules(): void {
    this.loading.set(true);
    this.pbService.getRules().subscribe({
      next: (res: PerformanceBonusRuleResponse[]) => {
        this.rules.set(res);
        this.loading.set(false);
      },
      error: (err: any) => {
        console.error(err);
        this.loading.set(false);
      }
    });
  }

  openAddRuleModal(): void {
    this.isRuleEdit.set(false);
    this.selectedRuleId = null;
    this.ruleForm.reset({
      minRating: 1,
      maxRating: 5,
      bonusPercentage: 10
    });
    this.showRuleModal.set(true);
  }

  openEditRuleModal(rule: PerformanceBonusRuleResponse): void {
    this.isRuleEdit.set(true);
    this.selectedRuleId = rule.id;
    this.ruleForm.reset({
      minRating: rule.minRating,
      maxRating: rule.maxRating,
      bonusPercentage: rule.bonusPercentage
    });
    this.showRuleModal.set(true);
  }

  closeRuleModal(): void {
    this.showRuleModal.set(false);
    this.selectedRuleId = null;
    this.ruleForm.reset();
  }

  submitRuleForm(): void {
    if (this.ruleForm.invalid) {
      this.ruleForm.markAllAsTouched();
      return;
    }

    const val = this.ruleForm.value;
    const payload = {
      minRating: Number(val.minRating) || 0,
      maxRating: Number(val.maxRating) || 0,
      bonusPercentage: Number(val.bonusPercentage) || 0
    };

    if (payload.minRating > payload.maxRating) {
      this.toastr.warning('Minimum rating cannot exceed maximum rating.');
      return;
    }

    this.submitting.set(true);
    if (this.isRuleEdit() && this.selectedRuleId) {
      this.pbService.updateRule(this.selectedRuleId, payload).subscribe({
        next: () => {
          this.toastr.success('Bonus rule updated successfully.');
          this.closeRuleModal();
          this.loadRules();
          this.submitting.set(false);
        },
        error: (err: any) => {
          console.error(err);
          this.submitting.set(false);
        }
      });
    } else {
      this.pbService.addRule(payload).subscribe({
        next: () => {
          this.toastr.success('Bonus rule added successfully.');
          this.closeRuleModal();
          this.loadRules();
          this.submitting.set(false);
        },
        error: (err: any) => {
          console.error(err);
          this.submitting.set(false);
        }
      });
    }
  }

  deleteRule(id: string): void {
    if (confirm('Delete this bonus rule threshold?')) {
      this.pbService.deleteRule(id).subscribe({
        next: () => {
          this.toastr.success('Rule deleted.');
          this.loadRules();
        },
        error: (err) => {
          console.error(err);
        }
      });
    }
  }

  // Recommendations logic
  loadRecommendations(): void {
    this.loading.set(true);
    this.pbService.getRecommendations().subscribe({
      next: (res: PerformanceBonusRecommendationResponse[]) => {
        this.recommendations.set(res);
        this.loading.set(false);
      },
      error: (err: any) => {
        console.error(err);
        this.loading.set(false);
      }
    });
  }

  generateRecs(): void {
    if (!this.selectedCycleId()) {
      this.toastr.warning('Please select or input a Performance Cycle ID.');
      return;
    }
    this.submitting.set(true);
    this.pbService.generateRecommendations(this.selectedCycleId()).subscribe({
      next: () => {
        this.toastr.success('Recommendations generated successfully.');
        this.loadRecommendations();
        this.submitting.set(false);
      },
      error: (err: any) => {
        console.error(err);
        this.submitting.set(false);
      }
    });
  }

  openRecModal(rec: PerformanceBonusRecommendationResponse): void {
    this.selectedRecId = rec.id;
    this.recForm.reset({
      approvedAmount: rec.approvedAmount ?? rec.recommendedAmount,
      remarks: rec.remarks || ''
    });
    this.showRecModal.set(true);
  }

  closeRecModal(): void {
    this.showRecModal.set(false);
    this.selectedRecId = '';
    this.recForm.reset();
  }

  submitRecForm(): void {
    if (this.recForm.invalid) {
      this.recForm.markAllAsTouched();
      return;
    }

    const val = this.recForm.value;
    const payload = {
      approvedAmount: Number(val.approvedAmount) || 0,
      remarks: val.remarks || ''
    };

    this.submitting.set(true);
    this.pbService.updateRecommendation(this.selectedRecId, payload).subscribe({
      next: () => {
        this.toastr.success('Recommendation amount modified.');
        this.closeRecModal();
        this.loadRecommendations();
        this.submitting.set(false);
      },
      error: (err: any) => {
        console.error(err);
        this.submitting.set(false);
      }
    });
  }

  approveRec(id: string): void {
    if (confirm('Approve this bonus recommendation?')) {
      this.submitting.set(true);
      this.pbService.approveRecommendation(id).subscribe({
        next: () => {
          this.toastr.success('Recommendation approved.');
          this.loadRecommendations();
          this.submitting.set(false);
        },
        error: (err) => {
          console.error(err);
          this.submitting.set(false);
        }
      });
    }
  }

  rejectRec(id: string): void {
    if (confirm('Reject this bonus recommendation?')) {
      this.submitting.set(true);
      this.pbService.rejectRecommendation(id).subscribe({
        next: () => {
          this.toastr.success('Recommendation rejected.');
          this.loadRecommendations();
          this.submitting.set(false);
        },
        error: (err) => {
          console.error(err);
          this.submitting.set(false);
        }
      });
    }
  }
}
