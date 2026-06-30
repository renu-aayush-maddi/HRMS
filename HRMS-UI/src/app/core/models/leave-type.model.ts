export interface LeaveType {
  id: string;
  name: string;
  annualAllocation: number;
  carryForwardAllowed: boolean;
  maxCarryForward: number;
  negativeBalanceAllowed: boolean;
  isActive: boolean;
}

export interface AddLeaveType {
  name: string;
  annualAllocation: number;
  carryForwardAllowed: boolean;
  maxCarryForward: number;
  negativeBalanceAllowed: boolean;
}

export interface UpdateLeaveType extends AddLeaveType {
  isActive: boolean;
}
