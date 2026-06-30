export interface EmployeeSalaryResponse {
  id: string;
  employeeName: string;
  salaryStructureName: string;
  annualCtc: number;
  effectiveFrom: string;
  isActive?: boolean;
}

export interface AssignEmployeeSalary {
  employeeId: string;
  salaryStructureId: string;
  annualCtc: number;
  effectiveFrom: string;
}

export interface SalaryHistoryResponse {
  id: string;
  employeeName: string;
  salaryStructureName: string;
  annualCtc: number;
  effectiveFrom: string;
  isActive?: boolean;
}
