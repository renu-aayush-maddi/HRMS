import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Educations } from './educations';

describe('Educations', () => {
  let component: Educations;
  let fixture: ComponentFixture<Educations>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Educations],
    }).compileComponents();

    fixture = TestBed.createComponent(Educations);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
