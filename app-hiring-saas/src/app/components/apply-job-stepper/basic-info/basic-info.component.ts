import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { ApplyJobStepperStatusService } from '../apply-job-stepper-status.service';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-basic-info',
  imports: [ReactiveFormsModule, CommonModule, TranslateModule],
  templateUrl: './basic-info.component.html',
  styleUrl: './basic-info.component.scss'
})
export class BasicInfoComponent {
  form: FormGroup;

  constructor(
    private _fb: FormBuilder,
    private _applyJobStepperStatusService: ApplyJobStepperStatusService,
    private _router: Router,
    private _route: ActivatedRoute
  ) {
    this.form = this._fb.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      phone: [
        '',
        [
          Validators.required,
          Validators.pattern(/^\d{3}-\d{3}-\d{4}$/), // 302-123-4567 format
        ],
      ],
      streetAddress: ['', Validators.required],
      aptSuite: [''],
      city: ['', Validators.required],
      state: ['', Validators.required],
      zip: ['', Validators.required],
    });

    const savedValues = this._applyJobStepperStatusService.getBasicInfoFormValues();
    if (savedValues) {
      this.form.patchValue(savedValues);
    }

    this.form.valueChanges.subscribe((values) => {
      this._applyJobStepperStatusService.setBasicInfoFormValid(this.form.valid);
      this._applyJobStepperStatusService.setBasicInfoFormValues(values);
    });
  }

  submitForm() {
    if (this.form.valid) {
      console.log('Form Submitted ✅', this.form.value);
    } else {
      this.form.markAllAsTouched();
    }
  }

  formatPhone(event: Event) {
    const input = event.target as HTMLInputElement;
    let value = input.value.replace(/\D/g, ''); // Remove non-digit characters

    // Limit to 10 digits
    if (value.length > 10) {
      value = value.substring(0, 10);
    }

    // Insert dashes
    if (value.length > 6) {
      value = `${value.substring(0, 3)}-${value.substring(3, 6)}-${value.substring(6)}`;
    } else if (value.length > 3) {
      value = `${value.substring(0, 3)}-${value.substring(3)}`;
    }

    // Update the input value and form control
    input.value = value;
    this.form.get('phone')?.setValue(value, { emitEvent: false }); // prevent infinite loop
  }

  // convenience getter
  get f() {
    return this.form.controls;
  }

  goToRequirements() {
    if (this.form.valid) {
      this.form.markAllAsTouched();
      this._router.navigate(['../requirements'], { relativeTo: this._route }); 
    } else {
      this.form.markAllAsTouched();
    }
  }
}
