export interface LeaveBalanceResponse {
  id: string;
  employeeName: string;
  leaveType: string;
  allocatedDays: number;
  usedDays: number;
  remainingDays: number;
}

export interface AllocateLeaveBalance {
  employeeId: string;
  leaveTypeId: string;
  allocatedDays: number;
}
