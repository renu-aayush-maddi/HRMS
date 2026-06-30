export interface SalaryStructureResponse {
  id: string;
  name: string;
  basicPercentage: number;
  hraPercentage: number;
  specialAllowancePercentage: number;
  medicalAllowancePercentage: number;
  travelAllowancePercentage: number;
}

export interface CreateSalaryStructure {
  name: string;
  basicPercentage: number;
  hraPercentage: number;
  specialAllowancePercentage: number;
  medicalAllowancePercentage: number;
  travelAllowancePercentage: number;
}

export interface UpdateSalaryStructure {
  name: string;
  basicPercentage: number;
  hraPercentage: number;
  specialAllowancePercentage: number;
  medicalAllowancePercentage: number;
  travelAllowancePercentage: number;
}
