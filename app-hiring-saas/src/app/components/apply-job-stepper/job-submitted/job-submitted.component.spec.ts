import { ComponentFixture, TestBed } from '@angular/core/testing';

import { JobSubmittedComponent } from './job-submitted.component';

describe('JobSubmittedComponent', () => {
  let component: JobSubmittedComponent;
  let fixture: ComponentFixture<JobSubmittedComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [JobSubmittedComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(JobSubmittedComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
