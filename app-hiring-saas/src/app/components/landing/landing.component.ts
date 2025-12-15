import { Component, OnInit } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { LandingHeaderComponent } from './landing-header/landing-header.component';
import { ApplyJobStepperStatusService } from '../apply-job-stepper/apply-job-stepper-status.service';
import { take } from 'rxjs';
import { JobResponse } from '../../models/job-stepper.model';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-landing',
  imports: [TranslateModule, LandingHeaderComponent, CommonModule],
  templateUrl: './landing.component.html',
  styleUrl: './landing.component.scss'
})
export class LandingComponent implements OnInit {

  jobList: JobResponse[] = [];

  constructor(
    private _applyJobStepperStatusService: ApplyJobStepperStatusService,
  ){}

  ngOnInit(): void {
    this._applyJobStepperStatusService.getLandingJobsById(1).pipe(take(1)).subscribe(
      data => {
        this.jobList = data;
      }
    )
  }

  scrollToSection(element: HTMLElement) {
    element.scrollIntoView({ behavior: 'smooth', block: 'start' });
  }

  openJobInNewTab(id: number) {
    const url = `/apply/${id}`;
    window.open(url, '_blank');
  }
}
