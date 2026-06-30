export interface Holiday {
  id: string;
  name: string;
  holidayDate: string; // ISO Date YYYY-MM-DD
  description?: string;
  isOptional: boolean;
}

export interface AddHoliday {
  name: string;
  holidayDate: string;
  description?: string;
  isOptional: boolean;
}

export interface UpdateHoliday extends AddHoliday {}
