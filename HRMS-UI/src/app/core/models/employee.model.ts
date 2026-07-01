export interface Employee {
  id: string;
  employeeCode: string;
  firstName: string;
  lastName: string;
  email: string;
  phone?: string;
  designation?: string;
  department: string;
  salary?: number;
  employmentStatus?: string;
  managerId?: string;
  managerName?: string;
}

export interface EmployeeFilter {
  search?: string;
  departmentId?: string;
  managerId?: string;
  employmentStatus?: string;
  designation?: string;
  sortBy?: string;
  descending?: boolean;
  pageNumber?: number;
  pageSize?: number;
}