import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ApplyJobStepperStatusService } from '../apply-job-stepper-status.service';
import { TranslateModule } from '@ngx-translate/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-requirements',
  imports: [ReactiveFormsModule, CommonModule, TranslateModule],
  templateUrl: './requirements.component.html',
  styleUrl: './requirements.component.scss'
})
export class RequirementsComponent {
form: FormGroup;

  constructor(
    private fb: FormBuilder,
    private _applyJobStepperStatusService: ApplyJobStepperStatusService,
    private _router: Router,
    private _route: ActivatedRoute
  ) {
    this.form = this.fb.group({
      hasCleaningExperience: [null, Validators.required],
      hasBankAccount: [null, Validators.required],
      isAuthorizedToWork: [null, Validators.required],
      consentBackgroundCheck: [null, Validators.required],
      cleaningExperienceDescription: ['', Validators.required]
    });

    const savedValues = this._applyJobStepperStatusService.getRequirementsFormValues();
    if (savedValues) {
      this.form.patchValue(savedValues);
    }

    this.form.valueChanges.subscribe((values) => {
      this._applyJobStepperStatusService.setRequirementsFormValid(this.form.valid);
      this._applyJobStepperStatusService.setRequirementsFormValues(values);
    });
  }

  get f() {
    return this.form.controls;
  }

  selectOption(controlName: string, value: boolean) {
    this.form.patchValue({ [controlName]: value });
  }

  submitForm() {
    if (this.form.valid) {
    } else {
      this.form.markAllAsTouched();
    }
  }

  goToIncome() {
    if (this.form.valid) {
      this.form.markAllAsTouched();
      this._router.navigate(['../income'], { relativeTo: this._route }); 
    } else {
      this.form.markAllAsTouched();
    }
  }
}
