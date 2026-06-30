import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Resignations } from './resignations';

describe('Resignations', () => {
  let component: Resignations;
  let fixture: ComponentFixture<Resignations>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Resignations],
    }).compileComponents();

    fixture = TestBed.createComponent(Resignations);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
