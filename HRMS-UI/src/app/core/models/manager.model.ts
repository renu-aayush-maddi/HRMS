export interface Manager {
  id: string;
  employeeCode: string;
  fullName: string;
  designation?: string;
}

export interface ManagerDashboardStats {
  teamSize: number;
  presentToday: number;
  onLeaveToday: number;
  pendingLeaveRequests: number;
  pendingRegularizations: number;
}

export interface TeamMember {
  id: string;
  employeeCode: string;
  fullName: string;
  email: string;
  designation: string;
  department: string;
  employmentStatus: string;
}

export interface LateEmployee {
  employeeName: string;
  checkIn?: string;
}

export interface PendingLeave {
  leaveId: string;
  employeeName: string;
  fromDate: string;
  toDate: string;
  reason?: string;
  status?: string;
}

export interface TeamAttendance {
  employeeName: string;
  attendanceDate: string;
  checkIn?: string;
  checkOut?: string;
  status?: string;
}

export interface ManagerPerformanceReview {
  id: string;
  employeeName: string;
  rating?: number;
  comments?: string;
  reviewDate?: string;
}

export interface AddPerformanceReview {
  employeeId: string;
  performanceCycleId: string;
  rating: number;
  comments: string;
}

export interface ManagerGoal {
  id: string;
  title: string;
  description: string;
  targetDate: string;
  status: string;
  employeeName: string;
}

export interface AddGoal {
  employeeId: string;
  title: string;
  description: string;
  targetDate: string;
}

export interface UpdateGoalStatus {
  status: string;
}

export interface GoalQuery {
  employeeId?: string;
  status?: string;
}