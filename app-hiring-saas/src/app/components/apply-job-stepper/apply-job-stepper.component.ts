import { Component, OnInit } from '@angular/core';
import { MainSidenavComponent } from '../main-sidenav/main-sidenav.component';
import { ActivatedRoute, Router, RouterOutlet } from '@angular/router';
import { ApplyJobStepperStatusService } from './apply-job-stepper-status.service';
import { take } from 'rxjs';
import { JobResponse } from '../../models/job-stepper.model';
import { CommonModule } from '@angular/common';
import { DomSanitizer } from '@angular/platform-browser';
import { WelcomeComponent } from './welcome/welcome.component';
import { VideoComponent } from './video/video.component';
import { TranslateModule } from '@ngx-translate/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatChipInputEvent, MatChipsModule } from '@angular/material/chips';
import { MatFormFieldModule } from '@angular/material/form-field';
import { ENTER, COMMA } from '@angular/cdk/keycodes';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';

@Component({
  selector: 'app-apply-job-stepper',
  imports: [MainSidenavComponent, RouterOutlet, CommonModule, MatChipsModule, MatSelectModule, MatIconModule, WelcomeComponent, VideoComponent, MatFormFieldModule, TranslateModule, FormsModule, ReactiveFormsModule, MatProgressSpinnerModule],
  templateUrl: './apply-job-stepper.component.html',
  styleUrl: './apply-job-stepper.component.scss'
})
export class ApplyJobStepperComponent implements OnInit {
  readonly separatorKeysCodes: number[] = [ENTER, COMMA];
  selectedJob!: JobResponse | undefined;
  selectedNavStep: number = 0;
  form: FormGroup; // Basic Info Default
  dynamicForm: FormGroup; // Dynamic form
  photoURL: any = null;
  videoURL: string | null = null;
  isLoading: boolean =  false;

  constructor(
    public _applyJobStepperStatusService: ApplyJobStepperStatusService,
    private route: ActivatedRoute,
    private sanitizer: DomSanitizer,
    private _fb: FormBuilder,
    private router: Router
  ){
    // Default form
    this.form = this._fb.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      phone: [
        '',
        [Validators.required, Validators.pattern(/^\d{3}-\d{3}-\d{4}$/)],
      ],
      streetAddress: ['', Validators.required],
      aptSuite: [''],
      city: ['', Validators.required],
      state: ['', Validators.required],
      zip: ['', Validators.required],
    });

    this.dynamicForm = this._fb.group({}); // init empty

    this.router.navigate([], {
      queryParams: { step: 'welcome' },
      queryParamsHandling: 'merge' // keep other params
    });
  }

  ngOnInit(): void {
    const hashCode = this.route.snapshot.paramMap.get('id');
    if (hashCode) {
      this._applyJobStepperStatusService.getLandingJobs(hashCode)
        .pipe(take(1))
        .subscribe(data => {
          this.selectedJob = data;
          this._applyJobStepperStatusService.calendlyUrl = data.creator?.calendlyProfileUrl || '';
          this._applyJobStepperStatusService.externalCalendarUrl = data.creator?.externalCalendarUrl || '';
          this._applyJobStepperStatusService.selectedJob = this.selectedJob;
          this.selectedJob?.jobForm?.jobFormSections.forEach(el => {
            el.icon = this.sanitizer.bypassSecurityTrustHtml(
              el.icon?.changingThisBreaksApplicationSecurity
            );
          });

          // build dynamic form controls
          this.buildDynamicForm();

          // push steps for sidenav
          this._applyJobStepperStatusService.jobSteps.push({
            isSelected: true,
            isActive: true,
            isValid: true,
            isDefault: false,
          });

          this.selectedJob?.jobForm.jobFormSections.forEach(el => {
            let navStep = {
              isSelected: false,
              isActive: false,
              isValid: false,
              isDefault: el.code == 'Default',
            };
            this._applyJobStepperStatusService.jobSteps.push(navStep);
          });
        });
    }
  }

  buildDynamicForm() {
    if (!this.selectedJob) return;

    this.selectedJob.jobForm.jobFormSections.forEach(section => {
      section.jobFormSectionProperties.forEach(field => {
        const validators = [];
        if (field.required) {
          validators.push(Validators.required);
        }
        if (field.type === 'email') {
          validators.push(Validators.email);
        }
        if (field.type === 'phone') {
          validators.push(Validators.pattern(/^\d{3}-\d{3}-\d{4}$/));
        }
        if (field.type === 'number') {
          validators.push(Validators.pattern(/^\d+$/));
        }

        // Handle different field types
        switch (field.type) {
          case 'checkbox':
            // Add one FormControl per checkbox option
            field.options.forEach(option => {
              this.dynamicForm.addControl(
                `${field.label}_${option}`,
                this._fb.control(false) // default unchecked
              );
            });
            break;
          case 'radio':
            // Single FormControl for radio group
            this.dynamicForm.addControl(
              field.label,
              this._fb.control('', validators)
            );
            break;
          case 'tags':
          this.dynamicForm.addControl(
            field.label,
            this._fb.control([], validators) // store as string[] array
          );
          break;
          default:
            // Text, email, number, textarea, select, date, file
            this.dynamicForm.addControl(
              field.label,
              this._fb.control('', validators)
            );
            break;
        }
      });
    });
    console.log(this.dynamicForm)
  }

  loadFirstFormStep() {
    this._applyJobStepperStatusService.jobSteps.forEach(el => {el.isSelected = false; el.isActive = false});
    this._applyJobStepperStatusService.jobSteps[1].isSelected = true;
    this._applyJobStepperStatusService.jobSteps[1].isActive = true;
    this._applyJobStepperStatusService.activeJobStep = 1;

    const stepTitle = this.jobSectionTitle;
      if (stepTitle) {
        this.router.navigate([], {
          queryParams: { step: stepTitle },
          queryParamsHandling: 'merge'
        });
    }
  }

  setSelectedStep(i: number) {
    if (i == 0) {
      this.router.navigate([], {
        queryParams: { step: 'welcome' },
        queryParamsHandling: 'merge' // keep other params
      });
    } else {
      const stepTitle = this.jobSectionTitle;
      if (stepTitle) {
        this.router.navigate([], {
          queryParams: { step: stepTitle },
          queryParamsHandling: 'merge'
        });
      }
    }
  }

  onContinueStep() {
    // Mark current step as valid before moving
    this._applyJobStepperStatusService.jobSteps[this._applyJobStepperStatusService.activeJobStep].isValid = true;

    this._applyJobStepperStatusService.jobSteps.forEach(el => {el.isSelected = false; el.isActive = false});
    this._applyJobStepperStatusService.activeJobStep = this._applyJobStepperStatusService.activeJobStep + 1;
    this._applyJobStepperStatusService.jobSteps[this._applyJobStepperStatusService.activeJobStep].isSelected = true;
    this._applyJobStepperStatusService.jobSteps[this._applyJobStepperStatusService.activeJobStep].isActive = true;

    const stepTitle = this.jobSectionTitle;
    this.router.navigate([], {
      queryParams: { step: stepTitle },
      queryParamsHandling: 'merge'
    });

    console.log(this._applyJobStepperStatusService.activeJobStep)
    console.log(this._applyJobStepperStatusService.jobSteps.length)
  }

  // NEW METHOD: Check if current step is valid
  isCurrentStepValid(): boolean {
    const currentStepProperties = this.jobSectionProperties;

    if (!currentStepProperties || currentStepProperties.length === 0) {
      return true; // If no fields, consider valid
    }

    // Check each field in the current step
    for (const field of currentStepProperties) {
      if (field.required) {
        switch (field.type) {
          case 'checkbox':
            // For checkboxes, check if at least one option is selected
            const hasSelectedCheckbox = field.options.some(option => {
              const controlName = `${field.label}_${option}`;
              return this.dynamicForm.get(controlName)?.value === true;
            });
            if (!hasSelectedCheckbox) {
              return false;
            }
            break;

          case 'file':
            // For file uploads, check if a file is selected (you might need to adjust this based on your file handling)
            if (field.label.toLowerCase().includes('photo') && !this.photoURL) {
              return false;
            }
            // Add other file field validations as needed
            break;

          default:
          // For other field types (text, email, radio, select, etc.)
          const defaultControl = this.dynamicForm.get(field.label);
          if (!defaultControl || defaultControl.invalid || !defaultControl.value ||
              (typeof defaultControl.value === 'string' && defaultControl.value.trim() === '')) {
            return false;
          }
          break;
        }
      }
    }

    return true;
  }

  get jobSectionProperties() {
    return this.selectedJob?.jobForm?.jobFormSections?.[this._applyJobStepperStatusService.activeJobStep - 1]?.jobFormSectionProperties ?? [];
  }

  get jobSectionTitle(){
    return this.selectedJob?.jobForm?.jobFormSections?.[this._applyJobStepperStatusService.activeJobStep - 1]?.name;
  }

  get jobSectionDescription() {
    return this.selectedJob?.jobForm?.jobFormSections?.[this._applyJobStepperStatusService.activeJobStep - 1]?.description;
  }

  formatPhone(event: Event) {
    const input = event.target as HTMLInputElement;
    let value = input.value.replace(/\D/g, '');
    if (value.length > 10) {
      value = value.substring(0, 10);
    }
    if (value.length > 6) {
      value = `${value.substring(0, 3)}-${value.substring(3, 6)}-${value.substring(6)}`;
    } else if (value.length > 3) {
      value = `${value.substring(0, 3)}-${value.substring(3)}`;
    }
    input.value = value;

    // Update the form control with the formatted value
    const fieldLabel = (event.target as HTMLInputElement).getAttribute('formControlName');
    if (fieldLabel) {
      this.dynamicForm.get(fieldLabel)?.setValue(value);
    }
  }

  onPhotoSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files[0]) {
      const file = input.files[0];
      const reader = new FileReader();
      reader.onload = e => {
        this.photoURL = reader.result;
      };
      reader.readAsDataURL(file);
    }
  }

  setRecordedVideo(event: string) {
    this.videoURL = event;
  }

  get df() {
    return this.dynamicForm.controls;
  }

  get f() {
    return this.form.controls;
  }


  async submitForm() {
    const formData = new FormData();

    //  Basic Info
    formData.append("JobDetailsId", String(this.selectedJob?.id));

    formData.append("FirstName", this.form.get('firstName')?.value || "");
    formData.append("LastName", this.form.get('lastName')?.value || "");
    formData.append("EmailAddress", this.form.get('email')?.value || "");
    formData.append("PhoneNumber", this.form.get('phone')?.value || "");
    formData.append("StreetAddress", this.form.get('streetAddress')?.value || "");
    formData.append("Apartment", this.form.get('aptSuite')?.value || "");
    formData.append("City", this.form.get('city')?.value || "");
    formData.append("State", this.form.get('state')?.value || "");
    formData.append("ZipCode", this.form.get('zip')?.value || "");
    if (this.photoURL) {
      const profilePicFile = this.base64ToFile(this.photoURL, "profile.png");
      formData.append("ProfilePicture", profilePicFile);
    }
    if (this.videoURL) {
      const videoFile = await this.blobUrlToFile(this.videoURL, "application-video.webm");
      formData.append("ApplicationVideo", videoFile);
    }

    // Map dynamic form sections
    let  jobApplicationSections = this.mapDynamicFormSections();
    jobApplicationSections = jobApplicationSections.filter(
      (section: any) => section.code !== 'Default'
    );
    formData.append("JobApplicationSectionsJson", JSON.stringify(jobApplicationSections));

    this.isLoading = true;
    this._applyJobStepperStatusService.sendJobApplication(formData)
      .pipe(take(1))
      .subscribe({
        next: (response) => {
          this.isLoading = false;

          // Check if user qualifies for interview
          const qualifiesForInterview = this.checkInterviewQualification();

          if (qualifiesForInterview) {
            this.router.navigate(['../job-submitted-qualified']);
          } else {
            this.router.navigate(['../job-submitted']);
          }
        },
        error: (error) => {
          this.isLoading = false;
          if (error.error.detail && error.error.detail === 'Account not verified'){
            // Handle verification error
          }
        }
      });
  }

  private checkInterviewQualification(): boolean {
    if (!this.selectedJob?.jobForm?.jobFormSections) {
      return false;
    }

    // Iterate through all form sections
    for (const section of this.selectedJob.jobForm.jobFormSections) {
      // Iterate through all fields in the section
      for (const field of section.jobFormSectionProperties) {
        // Skip if field has no interviewRequired values
        if (!field.interviewRequired || field.interviewRequired.length === 0) {
          continue;
        }

        // Get the selected value(s) for this field based on field type
        let selectedValues: string[] = [];

        switch (field.type) {
          case 'checkbox':
            // Collect all selected checkbox options
            field.options.forEach(option => {
              const controlName = `${field.label}_${option}`;
              const control = this.dynamicForm.get(controlName);
              if (control?.value === true) {
                selectedValues.push(option);
              }
            });
            break;

          case 'radio':
          case 'select':
            // Single selection
            const singleValue = this.dynamicForm.get(field.label)?.value;
            if (singleValue) {
              selectedValues.push(singleValue);
            }
            break;

          case 'tags':
            // Multiple tags
            const tagsValue = this.dynamicForm.get(field.label)?.value || [];
            selectedValues = Array.isArray(tagsValue) ? tagsValue : [];
            break;

          default:
            // For other field types, get string value
            const defaultValue = this.dynamicForm.get(field.label)?.value;
            if (defaultValue) {
              selectedValues.push(String(defaultValue));
            }
            break;
        }

        // Check if any selected value matches interviewRequired values
        const hasQualifyingValue = selectedValues.some(selectedValue =>
          field.interviewRequired.includes(selectedValue)
        );

        if (hasQualifyingValue) {
          return true; // User qualifies for interview
        }
      }
    }

    return false; // No qualifying values selected
  }

  private mapDynamicFormSections(): any[] {
    if (!this.selectedJob?.jobForm?.jobFormSections) {
      return [];
    }

    return this.selectedJob.jobForm.jobFormSections.map(section => {
      return {
        name: section.name,
        description: section.description,
        code: section.code,
        position: section.position,
        icon: {
          changingThisBreaksApplicationSecurity: section.icon?.changingThisBreaksApplicationSecurity || ""
        },
        jobApplicationSectionProperties: section.jobFormSectionProperties.map(field => {
          const mappedProperty: any = {
            type: field.type,
            label: field.label,
            required: field.required,
            position: field.position,
            placeHolder: field.placeHolder,
            options: field.options || [],
            stringValue: null,
            integerValue: null,
            dateTimeValue: null,
            booleanValue: null,
            validation: field.validation || null
          };

          // Get form value based on field type
          let formValue = null;

          switch (field.type) {
            case 'checkbox':
              // For checkboxes, collect all selected options
              const selectedCheckboxes: string[] = [];
              field.options.forEach(option => {
                const controlName = `${field.label}_${option}`;
                const control = this.dynamicForm.get(controlName);
                if (control?.value === true) {
                  selectedCheckboxes.push(option);
                }
              });
              formValue = selectedCheckboxes.join(', ');
              mappedProperty.stringValue = formValue;
              break;

            case 'radio':
            case 'select':
            case 'text':
            case 'textarea':
            case 'email':
            case 'phone':
              formValue = this.dynamicForm.get(field.label)?.value || '';
              mappedProperty.stringValue = formValue;
              break;

            case 'number':
              formValue = this.dynamicForm.get(field.label)?.value;
              if (formValue !== null && formValue !== undefined && formValue !== '') {
                mappedProperty.integerValue = parseInt(formValue, 10) || 0;
                mappedProperty.stringValue = String(formValue);
              }
              break;

            case 'date':
              formValue = this.dynamicForm.get(field.label)?.value;
              if (formValue) {
                // Convert to ISO string for dateTimeValue
                const dateValue = new Date(formValue);
                mappedProperty.dateTimeValue = dateValue.toISOString();
                mappedProperty.stringValue = formValue;
              }
              break;

            case 'boolean':
              formValue = this.dynamicForm.get(field.label)?.value;
              mappedProperty.booleanValue = Boolean(formValue);
              mappedProperty.stringValue = String(formValue);
              break;

            case 'file':
              // For file fields, you might want to handle them differently
              // This assumes the file value is stored as a string reference
              formValue = this.dynamicForm.get(field.label)?.value || '';
              mappedProperty.stringValue = formValue;
              break;
            case 'tags':
              formValue = this.dynamicForm.get(field.label)?.value || [];
              mappedProperty.stringValue = (formValue as string[]).join(', ');
              break;

            default:
              formValue = this.dynamicForm.get(field.label)?.value || '';
              mappedProperty.stringValue = formValue;
              break;
          }

          return mappedProperty;
        })
      };
    });
  }

  base64ToFile(base64Data: string, filename: string): File {
    const arr = base64Data.split(",");
    const mime = arr[0].match(/:(.*?);/)![1];
    const bstr = atob(arr[1]);
    let n = bstr.length;
    const u8arr = new Uint8Array(n);
    while (n--) {
      u8arr[n] = bstr.charCodeAt(n);
    }
    return new File([u8arr], filename, { type: mime });
  }

  async blobUrlToFile(blobUrl: string, filename: string): Promise<File> {
    const response = await fetch(blobUrl);
    const blob = await response.blob();
    return new File([blob], filename, { type: blob.type });
  }

  onTagsBlur(fieldLabel: string) {
    const value = this.dynamicForm.get(fieldLabel)?.value || '';
    if (typeof value === 'string') {
      const tagsArray = value.split(',')
                            .map((tag: string) => tag.trim())
                            .filter((tag: string) => tag.length > 0);
      this.dynamicForm.get(fieldLabel)?.setValue(tagsArray);
    }
  }

  addTag(fieldId: string, event: MatChipInputEvent): void {
    const input = event.input;
    const value = (event.value || '').trim();

    if (value) {
      const fieldLabel = this.findFieldLabelById(fieldId);
      const currentTags = this.dynamicForm.get(fieldLabel)?.value || [];
      this.dynamicForm.get(fieldLabel)?.setValue([...currentTags, value]);
    }

    // clear the input
    if (input) {
      input.value = '';
    }
  }

  removeTag(fieldId: string, tag: string): void {
    const fieldLabel = this.findFieldLabelById(fieldId);
    const tags = this.dynamicForm.get(fieldLabel)?.value || [];
    const index = tags.indexOf(tag);

    if (index >= 0) {
      tags.splice(index, 1);
      this.dynamicForm.get(fieldLabel)?.setValue([...tags]);
    }
  }

  onTagInputKeydown(event: KeyboardEvent, fieldId: string): void {
    // handle Enter without comma if needed
    if (event.key === 'Enter') {
      event.preventDefault(); // prevent form submit
      const input = event.target as HTMLInputElement;
      const value = input.value.trim();
      if (value) {
        this.addTag(fieldId, { value, input } as MatChipInputEvent);
      }
    }
  }

  private findFieldLabelById(fieldKey: string): string {
    const field = this.selectedJob?.jobForm?.jobFormSections?.[this._applyJobStepperStatusService.activeJobStep - 1]?.jobFormSectionProperties.find(el => el.label == fieldKey);
    return field ? field.label : '';
  }

  isVideoStep(){
    return this.selectedJob?.jobForm?.jobFormSections?.[this._applyJobStepperStatusService.activeJobStep - 1]?.jobFormSectionProperties.find(el => el.type == 'video');
  }

  selectedVideoButton(event: string) {
    if (event == 'continue') {
      this.onContinueStep();
    } else
    if (event == 'submit') {
      this.submitForm();
    }
  }

  toggleTag(fieldLabel: string, option: string): void {
  const control = this.dynamicForm.get(fieldLabel);
  if (!control) return;

  const current = control.value || [];
  const index = current.indexOf(option);

  if (index >= 0) {
    // remove
    control.setValue(current.filter((o: string) => o !== option));
  } else {
    // add (only if less than 3)
    if (current.length < 3) {
      control.setValue([...current, option]);
    }
  }
}

isTagSelected(fieldLabel: string, option: string): boolean {
  const control = this.dynamicForm.get(fieldLabel);
  return control?.value?.includes(option) || false;
}
}