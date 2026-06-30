import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LeaveTypes } from './leave-types';

describe('LeaveTypes', () => {
  let component: LeaveTypes;
  let fixture: ComponentFixture<LeaveTypes>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LeaveTypes],
    }).compileComponents();

    fixture = TestBed.createComponent(LeaveTypes);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
