export interface HrDashboardStats {
  totalEmployees: number;
  activeEmployees: number;
  onLeaveEmployees: number;
  probationEmployees: number;
  departments: number;
  pendingLeaves: number;
  approvedLeaves: number;
  payrollGeneratedThisMonth: number;
  totalMonthlyPayroll: number;
  presentToday: number;
  absentToday: number;
  lateEmployees: number;
  attendancePercentage: number;
  newHiresThisMonth: number;
}

export interface DepartmentSummary {
  departmentName: string;
  employeeCount: number;
  activeEmployees: number;
  onLeaveEmployees: number;
}

export interface LeaveSummary {
  pendingLeaves: number;
  approvedLeaves: number;
  rejectedLeaves: number;
  totalLeaves: number;
}

export interface PayrollSummary {
  totalPayroll: number;
  averageSalary: number;
  highestSalary: number;
  employeesPaid: number;
}
