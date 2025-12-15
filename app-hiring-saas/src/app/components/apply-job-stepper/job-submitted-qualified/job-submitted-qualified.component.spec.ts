import { ComponentFixture, TestBed } from '@angular/core/testing';

import { JobSubmittedQualifiedComponent } from './job-submitted-qualified.component';

describe('JobSubmittedQualifiedComponent', () => {
  let component: JobSubmittedQualifiedComponent;
  let fixture: ComponentFixture<JobSubmittedQualifiedComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [JobSubmittedQualifiedComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(JobSubmittedQualifiedComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
