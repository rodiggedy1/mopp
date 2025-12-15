import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StepperNavbarComponent } from './stepper-navbar.component';

describe('StepperNavbarComponent', () => {
  let component: StepperNavbarComponent;
  let fixture: ComponentFixture<StepperNavbarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StepperNavbarComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StepperNavbarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
