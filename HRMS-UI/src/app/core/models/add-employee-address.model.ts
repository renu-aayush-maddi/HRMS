export interface AddEmployeeAddress {

  employeeId: string;

  addressLine1: string;

  addressLine2?: string;

  city: string;

  state: string;

  country: string;

  postalCode: string;

  addressType: string;

}