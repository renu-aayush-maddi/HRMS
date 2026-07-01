export interface EmployeeResignation {
  id: string;
  employeeId: string;
  employeeCode: string;
  employeeName: string;
  resignationDate: string;
  lastWorkingDate?: string;
  reason?: string;
  status: string;
  hrComments?: string;
  finalSettlementStatus: string;
  approvedBy?: string;
  approvedAt?: string;
  rejectedBy?: string;
  rejectedAt?: string;
  withdrawnAt?: string;
  createdAt?: string;
  updatedAt?: string;
}

export interface ResignationFilter {
  employeeId?: string;
  status?: string;
  finalSettlementStatus?: string;
  search?: string;
  fromResignationDate?: string;
  toResignationDate?: string;
  sortBy?: string;
  descending?: boolean;
}

export interface RejectResignation {
  reason: string;
}

export interface UpdateSettlementStatus {
  status: string;
}
