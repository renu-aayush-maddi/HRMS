export interface LeaveResponse {
  id: string;
  employeeName: string;
  fromDate: string;
  toDate: string;
  reason?: string;
  status?: string;
  managerComments?: string;
  leaveType: string;
}

export interface LeaveFilter {
  employeeId?: string;
  leaveTypeId?: string;
  status?: string;
  fromDate?: string;
  toDate?: string;
  pageNumber?: number;
  pageSize?: number;
  sortBy?: string;
  descending?: boolean;
}

export interface LeaveAction {
  managerComments?: string;
}

export interface PagedLeaveResult {
  data: LeaveResponse[];
  pageNumber: number;
  pageSize: number;
  totalRecords: number;
  totalPages: number;
}
