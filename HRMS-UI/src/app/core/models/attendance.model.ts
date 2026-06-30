export interface AttendanceResponse {
  id: string;
  employeeId: string;
  employeeCode: string;
  employeeName: string;
  attendanceDate: string;
  checkIn?: string;
  checkOut?: string;
  workingHours?: number;
  status: string;
  remarks?: string;
}

export interface AttendanceFilter {
  employeeId?: string;
  status?: string;
  fromDate?: string;
  toDate?: string;
  pageNumber?: number;
  pageSize?: number;
  sortBy?: string;
  descending?: boolean;
}

export interface PagedAttendanceResult {
  data: AttendanceResponse[];
  pageNumber: number;
  pageSize: number;
  totalRecords: number;
  totalPages: number;
}

export interface AttendanceRegularizationResponse {
  id: string;
  employeeId: string;
  employeeCode: string;
  employeeName: string;
  attendanceDate: string;
  requestedCheckIn?: string;
  requestedCheckOut?: string;
  reason: string;
  status: string;
  hrComments?: string;
  reviewedBy?: string;
  reviewedAt?: string;
  createdAt?: string;
}

export interface AttendanceRegularizationFilter {
  employeeId?: string;
  status?: string;
  fromDate?: string;
  toDate?: string;
  pageNumber?: number;
  pageSize?: number;
  sortBy?: string;
  descending?: boolean;
}

export interface PagedRegularizationResult {
  data: AttendanceRegularizationResponse[];
  pageNumber: number;
  pageSize: number;
  totalRecords: number;
  totalPages: number;
}

export interface ApproveAttendanceRegularization {
  hrComments?: string;
}

export interface RejectAttendanceRegularization {
  hrComments: string;
}
