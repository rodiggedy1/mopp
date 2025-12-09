import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { ApplyJobStepperStatusService } from '../apply-job-stepper/apply-job-stepper-status.service';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { JobStep } from '../../models/job-stepper.model';

@Component({
  selector: 'app-main-sidenav',
  imports: [RouterModule, CommonModule, TranslateModule],
  templateUrl: './main-sidenav.component.html',
  styleUrl: './main-sidenav.component.scss'
})
export class MainSidenavComponent {

  furthestStep: any;

  @Output() selectedStep: EventEmitter<number> = new EventEmitter();
  selectedStepIndex: number = 0;
  @Input() stepsList: Array<JobStep> | undefined = [];

  constructor(
    public _applyJobStepperStatusService: ApplyJobStepperStatusService,
    private _router: Router,
  ) {
  }

  selectedNavPage(i: number) {
    if (this._applyJobStepperStatusService.jobSteps[i].isValid || this._applyJobStepperStatusService.jobSteps[i].isActive) {
      this._applyJobStepperStatusService.jobSteps.forEach(el => el.isSelected = false);
      this._applyJobStepperStatusService.jobSteps[i].isSelected = true;
      this._applyJobStepperStatusService.activeJobStep = i;
      this.selectedStep.emit(i);
    }
  }

  ngOnInit(): void {
  }
}