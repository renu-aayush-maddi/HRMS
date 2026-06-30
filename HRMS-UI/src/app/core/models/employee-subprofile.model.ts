// Education
export interface EmployeeEducation {
  id: string;
  employeeId: string;
  degree: string;
  specialization: string;
  institutionName: string;
  graduationYear: number;
  percentage?: number;
}

export interface AddEmployeeEducation {
  employeeId?: string;
  degree: string;
  specialization: string;
  institutionName: string;
  graduationYear: number;
  percentage?: number;
}

export interface UpdateEmployeeEducation extends AddEmployeeEducation {}

// Experience
export interface EmployeeExperience {
  id: string;
  employeeId: string;
  companyName: string;
  designation: string;
  startDate?: string;
  endDate?: string;
  responsibilities?: string;
}

export interface AddEmployeeExperience {
  employeeId?: string;
  companyName: string;
  designation: string;
  startDate?: string;
  endDate?: string;
  responsibilities?: string;
}

export interface UpdateEmployeeExperience extends AddEmployeeExperience {}

// Emergency Contact
export interface EmployeeEmergencyContact {
  id: string;
  employeeId: string;
  contactName: string;
  relationship: string;
  phone: string;
  email: string;
}

export interface AddEmployeeEmergencyContact {
  employeeId?: string;
  contactName: string;
  relationship: string;
  phone: string;
  email: string;
}

export interface UpdateEmployeeEmergencyContact extends AddEmployeeEmergencyContact {}

// Document
export interface EmployeeDocument {
  id: string;
  employeeId: string;
  documentName: string;
  documentType: string;
  fileUrl: string;
  isVerified?: boolean;
  uploadedAt?: string;
}
