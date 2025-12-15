import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ApplyJobStepperComponent } from './apply-job-stepper.component';

describe('ApplyJobStepperComponent', () => {
  let component: ApplyJobStepperComponent;
  let fixture: ComponentFixture<ApplyJobStepperComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ApplyJobStepperComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ApplyJobStepperComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
