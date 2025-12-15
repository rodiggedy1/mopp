import { Component } from '@angular/core';
import { ApplyJobStepperStatusService } from '../apply-job-stepper-status.service';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-income',
  imports: [TranslateModule],
  templateUrl: './income.component.html',
  styleUrl: './income.component.scss'
})
export class IncomeComponent {

  constructor(
    private _applyJobStepperStatusService: ApplyJobStepperStatusService,
    private _router: Router,
    private _route: ActivatedRoute
  ){
  }

  goToIncome() {
    this._applyJobStepperStatusService.setIncomeFormValid(true);
    this._router.navigate(['../your-bio'], { relativeTo: this._route }); 
  }
}
