export interface EmployeeProfile {

  id: string;

  employeeCode: string;

  firstName: string;

  lastName: string;

  email: string;

  phone?: string;

  designation?: string;

  department?: string;

  managerName?: string;

  employmentStatus?: string;

  salary?: number;

  educations: EmployeeEducation[];

  experiences: EmployeeExperience[];

  emergencyContacts: EmployeeEmergencyContact[];

  addresses: EmployeeAddress[];

  documents: EmployeeDocument[];
}

export interface EmployeeEducation {

  id: string;

  degree: string;

  specialization: string;

  institutionName: string;

  graduationYear: number;

  percentage?: number;
}

export interface EmployeeExperience {

  id: string;

  companyName: string;

  designation: string;

  startDate?: string;

  endDate?: string;

  responsibilities?: string;
}

export interface EmployeeEmergencyContact {

  id: string;

  contactName: string;

  relationship: string;

  phone: string;

  email: string;
}

export interface EmployeeAddress {

  id: string;

  addressLine1: string;

  addressLine2?: string;

  city: string;

  state: string;

  country: string;

  postalCode: string;

  addressType: string;
}

export interface EmployeeDocument {

  id: string;

  documentName: string;

  documentType: string;

  fileUrl: string;

  isVerified?: boolean;

  uploadedAt?: string;
}