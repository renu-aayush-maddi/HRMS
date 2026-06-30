export interface BonusResponse {
  id: string;
  employeeId: string;
  employeeName: string;
  amount: number;
  reason: string;
  bonusMonth: number;
  bonusYear: number;
  status: string;
  createdAt?: string;
}

export interface CreateBonus {
  employeeId: string;
  amount: number;
  reason: string;
  bonusMonth: number;
  bonusYear: number;
}

export interface BonusFilter {
  employeeId?: string;
  status?: string;
  bonusMonth?: number;
  bonusYear?: number;
  minAmount?: number;
  maxAmount?: number;
  pageNumber?: number;
  pageSize?: number;
  sortBy?: string;
  descending?: boolean;
}

export interface PagedBonusResult {
  data: BonusResponse[];
  pageNumber: number;
  pageSize: number;
  totalRecords: number;
  totalPages: number;
}

export interface DeductionResponse {
  id: string;
  employeeId: string;
  employeeName: string;
  amount: number;
  reason: string;
  deductionMonth: number;
  deductionYear: number;
  status: string;
  createdAt?: string;
}

export interface CreateDeduction {
  employeeId: string;
  amount: number;
  reason: string;
  deductionMonth: number;
  deductionYear: number;
}

export interface DeductionFilter {
  employeeId?: string;
  status?: string;
  deductionMonth?: number;
  deductionYear?: number;
  minAmount?: number;
  maxAmount?: number;
  pageNumber?: number;
  pageSize?: number;
  sortBy?: string;
  descending?: boolean;
}

export interface PagedDeductionResult {
  data: DeductionResponse[];
  pageNumber: number;
  pageSize: number;
  totalRecords: number;
  totalPages: number;
}
