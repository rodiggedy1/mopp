import { Component, OnInit } from '@angular/core';
import { SidebarComponent } from '../sidebar/sidebar.component';
import { TranslateModule } from '@ngx-translate/core';
import { CommonModule } from '@angular/common';
import { StepperNavbarComponent } from './stepper-navbar/stepper-navbar.component';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { ApplyJobStepperStatusService } from '../apply-job-stepper/apply-job-stepper-status.service';
import { FormBuilderComponent } from './form-builder/form-builder.component';
import { FormTemplate, jobForm, JobStep, JobStepperRequest } from '../../models/job-stepper.model';
import { VideoComponent } from '../apply-job-stepper/video/video.component';
import { AuthService } from '../../services/auth.service';
import { take } from 'rxjs';
import { JobApplicationSearchRequest } from '../../models/applicant.model';
import { ActivatedRoute, Router } from '@angular/router';
import { DomSanitizer } from '@angular/platform-browser';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatChipsModule } from '@angular/material/chips';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { YourBioComponent } from '../apply-job-stepper/your-bio/your-bio.component';
import { MatSelectModule } from '@angular/material/select';
import { MatTooltipModule } from '@angular/material/tooltip';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-admin-forms',
  imports: [SidebarComponent, TranslateModule, CommonModule, MatChipsModule, MatSelectModule, MatTooltipModule, StepperNavbarComponent, MatFormFieldModule, MatIconModule, FormsModule, ReactiveFormsModule, FormBuilderComponent, VideoComponent, MatProgressSpinnerModule, YourBioComponent],
  templateUrl: './admin-forms.component.html',
  styleUrl: './admin-forms.component.scss'
})
export class AdminFormsComponent implements OnInit {

  showFormBuilder: boolean = false;
  form!: FormGroup;
  jobForms: Array<jobForm> = [];
  activeUserId: number | null = null;
  fieldTags: { [fieldId: string]: string[] } = {};
  currentTagInput: { [fieldId: string]: string } = {};
  selectedTags: { [fieldId: string]: string[] } = {};
  toastMessage: string | null = null;
  radioButtonValues: string[] = [];
  appDomain: string = environment.appDomain;

  constructor(
    private _fb: FormBuilder,
    public _applyJobStepperStatusService: ApplyJobStepperStatusService,
    private _authService: AuthService,
    private router: Router,
    private route: ActivatedRoute,
    private sanitizer: DomSanitizer,
  ){
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
  }

  ngOnInit(): void {
    this._authService.currentUserSubject.subscribe(data => {
      if (data) {
        let isSubscribed = localStorage.getItem('subscriptionStatus');
        if (!isSubscribed || isSubscribed == 'canceled' || isSubscribed == 'incomplete' || isSubscribed == 'incomplete_expired' || isSubscribed == 'unpaid' || isSubscribed == 'none') {
          this.router.navigate(['./subscription']);
          this._authService.isSubscribeDialogOpen = true;
          this._authService.showUnsubscribedMessage.next(true);
        }

        this.activeUserId = data.id;

        const request: JobApplicationSearchRequest = {
          query: null,
          statusId: null,
          jobDetailsCreatedById: data.id,
          jobDetailsId: null,
          status: null,
          dateFrom: null,
          dateTo: null,
          paging: {
            pageNumber: 1,
            pageSize: 10
          },
          sorting: {
            field: 1,
            sortOrder: 0
          }
        };

        this.getFormsData(request);
      }
    });
  }

getFormsData(request: JobApplicationSearchRequest) {
  this._applyJobStepperStatusService.getJobSteppers(request).pipe(take(1)).subscribe(data => {
    this.jobForms = data.items;
  })
}

addNewJobForm(){
  this._applyJobStepperStatusService.addedSteps = [
    {
      name: 'Basic Info',
      icon: this.sanitizer.bypassSecurityTrustHtml(`
        <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24"
          stroke-width="1.5" stroke="currentColor" style="width: 24px; height: 24px">
          <path stroke-linecap="round" stroke-linejoin="round"
            d="M17.982 18.725A7.488 7.488 0 0 0 12 15.75a7.488 7.488 0 0 0-5.982 2.975m11.963
            0a9 9 0 1 0-11.963 0m11.963
            0A8.966 8.966 0 0 1 12 21a8.966 8.966
            0 0 1-5.982-2.275M15 9.75a3 3
            0 1 1-6 0 3 3 0 0 1 6 0Z"></path>
        </svg>
      `),
      isSelected: true,
      previewMode: true,
      fields: [],
      description: '',
      isDefault: true,
    }
  ];

  this._applyJobStepperStatusService.activeStep = 0;
  this._applyJobStepperStatusService.formTitle = '';
  this._applyJobStepperStatusService.formDescription = '';
  this._applyJobStepperStatusService.formId = null;
  this.showFormBuilder = true;
}

 onSelectJobForm(stepper: jobForm, i: number) {
    stepper.jobFormSections.forEach((el, i) => {
      let step = { name: el.name, icon: this.sanitizer.bypassSecurityTrustHtml(el.icon?.changingThisBreaksApplicationSecurity), isSelected: i == 0, previewMode: i == 0, fields: el.jobFormSectionProperties, description: el.description, isDefault: el.code == 'Default' }
      this._applyJobStepperStatusService.addedSteps.push(step);
    });

    if (!this._applyJobStepperStatusService.addedSteps || this._applyJobStepperStatusService.addedSteps.length == 0) {
      this._applyJobStepperStatusService.addedSteps = [
        {
          name: 'Basic Info',
          icon: this.sanitizer.bypassSecurityTrustHtml(`
            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24"
              stroke-width="1.5" stroke="currentColor" style="width: 24px; height: 24px">
              <path stroke-linecap="round" stroke-linejoin="round"
                d="M17.982 18.725A7.488 7.488 0 0 0 12 15.75a7.488 7.488 0 0 0-5.982 2.975m11.963
                0a9 9 0 1 0-11.963 0m11.963
                0A8.966 8.966 0 0 1 12 21a8.966 8.966
                0 0 1-5.982-2.275M15 9.75a3 3
                0 1 1-6 0 3 3 0 0 1 6 0Z"></path>
            </svg>
          `),
          isSelected: true,
          previewMode: true,
          fields: [],
          description: '',
          isDefault: true,
        }
      ];
    }

    this._applyJobStepperStatusService.activeStep = i;
    this._applyJobStepperStatusService.formTitle = stepper.title ? stepper.title : '';
    this._applyJobStepperStatusService.formDescription = stepper.description ? stepper.description : '';
    this._applyJobStepperStatusService.formId = stepper.id;
    this.showFormBuilder = true;
    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: { form: stepper.title },
      queryParamsHandling: 'merge'
    });
  }

  onHideFormBuilder() {
    this._applyJobStepperStatusService.addedSteps = [];
    this._applyJobStepperStatusService.activeStep = 0;
    this._applyJobStepperStatusService.formTitle = '';
    this._applyJobStepperStatusService.formDescription = '';
    this.showFormBuilder = false;
    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: {},
      queryParamsHandling: ''
    });
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

   addTag(fieldId: string, event: any): void {
    const input = event.input;
    const value = (event.value || '').trim();

    if (!this.fieldTags[fieldId]) {
      this.fieldTags[fieldId] = [];
    }

    if (value) {
      this.fieldTags[fieldId].push(value);
    }

    // reset the input
    if (input) {
      input.value = '';
    }

    this.currentTagInput[fieldId] = '';
   }
   removeTag(fieldId: string, tag: string): void {
      const index = this.fieldTags[fieldId]?.indexOf(tag);
      if (index >= 0) {
        this.fieldTags[fieldId].splice(index, 1);
      }
    }
   onTagInputKeydown(event: KeyboardEvent, fieldId: string): void {
    if (event.key === 'Enter') {
      event.preventDefault();
      const value = this.currentTagInput[fieldId]?.trim();
      if (value) {
        if (!this.fieldTags[fieldId]) {
          this.fieldTags[fieldId] = [];
        }
        this.fieldTags[fieldId].push(value);
        this.currentTagInput[fieldId] = '';
      }
    }
    }

  // convenience getter
  get f() {
    return this.form.controls;
  }

  saveCustomForm(event: FormTemplate) {
    this._applyJobStepperStatusService.addedSteps[this._applyJobStepperStatusService.activeStep].fields = event.fields;
    this._applyJobStepperStatusService.addedSteps[this._applyJobStepperStatusService.activeStep].description = event.description;
    this._applyJobStepperStatusService.addedSteps[this._applyJobStepperStatusService.activeStep].previewMode = true;
  }

  editStep() {
    this._applyJobStepperStatusService.addedSteps[this._applyJobStepperStatusService.activeStep].previewMode = false;
  }

  getAllForms() {
    const request: JobApplicationSearchRequest = {
      query: null,
      statusId: null,
      jobDetailsCreatedById: this.activeUserId,
      jobDetailsId: null,
      status: null,
      dateFrom: null,
      dateTo: null,
      paging: {
        pageNumber: 1,
        pageSize: 10
      },
      sorting: {
        field: 1,
        sortOrder: 0
      }
    };

    this.getFormsData(request);
  }

  toggleTag(fieldId: string, option: string) {
    if (!this.selectedTags[fieldId]) this.selectedTags[fieldId] = [];

    if (this.selectedTags[fieldId].includes(option)) {
      this.selectedTags[fieldId] = this.selectedTags[fieldId].filter(o => o !== option);
    } else if (this.selectedTags['undefined']?.length < 3) {
      this.selectedTags[fieldId].push(option);
    }
  }

  copyToClipboard(uniqueHash: string) {
    let url: string = this.appDomain + '/apply/' + uniqueHash;

    // Copy to clipboard
    navigator.clipboard.writeText(url).then(() => {
      // Show toast
      this.toastMessage = "Form URL copied to clipboard!";

      // Hide toast after 3 seconds
      setTimeout(() => {
        this.toastMessage = null;
      }, 3000);
    }).catch(err => {
      console.error('Could not copy text: ', err);
    });
  }

  getRadioBtnValue(index: number): string {
    if (!this.radioButtonValues[index]) {
      this.radioButtonValues[index] = '';
    }
    return this.radioButtonValues[index];
  }

  setRadioBtnValue(index: number, option: string) {
    this.radioButtonValues[index] = option.toLowerCase();
  }
}
