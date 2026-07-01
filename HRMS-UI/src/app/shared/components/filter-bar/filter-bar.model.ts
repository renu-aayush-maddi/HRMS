export interface FilterOption {
  value: any;
  label: string;
}

export interface FilterField {
  key: string;
  label: string;
  type: 'select' | 'text' | 'date' | 'number';
  options?: FilterOption[];
  placeholder?: string;
}

export interface SortOption {
  value: string;
  label: string;
}
