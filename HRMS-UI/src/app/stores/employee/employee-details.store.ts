import { Injectable, signal } from '@angular/core';

import { EmployeeProfile } from '../../core/models/employee-profile.model';

@Injectable({
  providedIn: 'root'
})
export class EmployeeDetailsStore {

  selectedEmployeeId = signal<string>('');

  profile = signal<EmployeeProfile | null>(null);

  setSelectedEmployee(id: string): void {

    this.selectedEmployeeId.set(id);

  }

  setProfile(profile: EmployeeProfile): void {

    this.profile.set(profile);

  }

  clear(): void {

    this.selectedEmployeeId.set('');

    this.profile.set(null);

  }

}