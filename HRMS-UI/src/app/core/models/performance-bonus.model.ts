export interface PerformanceBonusRuleResponse {
  id: string;
  minRating: number;
  maxRating: number;
  bonusPercentage: number;
  isActive: boolean;
}

export interface AddPerformanceBonusRule {
  minRating: number;
  maxRating: number;
  bonusPercentage: number;
}

export interface UpdatePerformanceBonusRule {
  minRating: number;
  maxRating: number;
  bonusPercentage: number;
}

export interface PerformanceBonusRecommendationResponse {
  id: string;
  employeeId: string;
  employeeName: string;
  averageRating: number;
  recommendedPercentage: number;
  recommendedAmount: number;
  approvedAmount?: number;
  status: string;
  remarks?: string;
}

export interface UpdatePerformanceBonusRecommendation {
  approvedAmount: number;
  remarks?: string;
}

export interface EmployeePerformance {
  employeeId: string;
  employeeName: string;
  averageRating: number;
}

export interface DepartmentRating {
  departmentName: string;
  averageRating: number;
}

export interface HrDashboardResponse {
  totalEmployees: number;
  totalReviews: number;
  averageRating: number;
  reviewCompletionPercentage: number;
  topPerformers: EmployeePerformance[];
  lowestPerformers: EmployeePerformance[];
  departmentRatings: DepartmentRating[];
}
