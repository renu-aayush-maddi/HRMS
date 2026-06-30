export interface PerformanceCycle {
  id: string;
  name: string;
  startDate: string;
  endDate: string;
  status: string;
}

export interface AddPerformanceCycle {
  name: string;
  startDate: string;
  endDate: string;
}

export interface UpdatePerformanceCycle {
  name: string;
  startDate: string;
  endDate: string;
  status: string;
}
