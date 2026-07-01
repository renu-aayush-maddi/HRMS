export interface PayrollResponse {
  id: string;
  employeeName: string;
  payMonth: number;
  payYear: number;
  basicSalary: number;
  bonus?: number;
  deductions?: number;
  netSalary?: number;
  basicComponent?: number;
  hraComponent?: number;
  specialAllowanceComponent?: number;
  medicalAllowanceComponent?: number;
  travelAllowanceComponent?: number;
  status: string;
  generatedAt?: string;
}

export interface PayrollFilter {
  employeeId?: string;
  payMonth?: number;
  payYear?: number;
  status?: string;
  minNetSalary?: number;
  maxNetSalary?: number;
  sortBy?: string;
  descending?: boolean;
}

export interface GeneratePayroll {
  employeeId: string;
  payMonth: number;
  payYear: number;
}

export interface GenerateMonthlyPayroll {
  payMonth: number;
  payYear: number;
}
