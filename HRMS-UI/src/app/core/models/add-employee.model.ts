export interface AddEmployee {
  firstName: string;
  lastName: string;
  email: string;
  phone?: string;
  designation: string;
  departmentId: string;
  salary: number;
  role: string;
  managerId?: string;
}