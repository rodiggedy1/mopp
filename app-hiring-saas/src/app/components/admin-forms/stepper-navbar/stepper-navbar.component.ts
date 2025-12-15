import { Component, EventEmitter, Output } from '@angular/core';
import { ApplyJobStepperStatusService } from '../../apply-job-stepper/apply-job-stepper-status.service';
import { TranslateModule } from '@ngx-translate/core';
import { CommonModule } from '@angular/common';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { AddStepDialogComponent } from '../add-step-dialog/add-step-dialog.component';
import { CdkDragDrop, DragDropModule, moveItemInArray } from '@angular/cdk/drag-drop';
import { FormField, JobStep, JobStepField, JobStepperRequest } from '../../../models/job-stepper.model';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-stepper-navbar',
  imports: [CommonModule, TranslateModule, FormsModule, MatDialogModule, DragDropModule, RouterModule],
  templateUrl: './stepper-navbar.component.html',
  styleUrl: './stepper-navbar.component.scss'
})
export class StepperNavbarComponent {

  @Output() showJobForms: EventEmitter<void> = new EventEmitter();
  @Output() refreshForms: EventEmitter<void> = new EventEmitter();
  isLoading: boolean = true;

  constructor(
    public _applyJobStepperStatusService: ApplyJobStepperStatusService,
    private sanitizer: DomSanitizer,
    private dialog: MatDialog
  ) {
    // this._applyJobStepperStatusService.addedSteps = [
    //   {
    //     name: 'Basic Info',
    //     icon: this.sanitizer.bypassSecurityTrustHtml(`
    //       <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24"
    //         stroke-width="1.5" stroke="currentColor" style="width: 24px; height: 24px">
    //         <path stroke-linecap="round" stroke-linejoin="round"
    //           d="M17.982 18.725A7.488 7.488 0 0 0 12 15.75a7.488 7.488 0 0 0-5.982 2.975m11.963 
    //           0a9 9 0 1 0-11.963 0m11.963 
    //           0A8.966 8.966 0 0 1 12 21a8.966 8.966 
    //           0 0 1-5.982-2.275M15 9.75a3 3 
    //           0 1 1-6 0 3 3 0 0 1 6 0Z"></path>
    //       </svg>
    //     `),
    //     isSelected: true,
    //     previewMode: true,
    //     fields: [],
    //     description: '',
    //     isDefault: true,
    //   }
    // ];
  }

  ngOnInit(): void {}

  svg(inner: string): SafeHtml {
    return this.sanitizer.bypassSecurityTrustHtml(`
      <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24"
        viewBox="0 0 24 24" fill="none"
        stroke="currentColor" stroke-width="2"
        stroke-linecap="round" stroke-linejoin="round"
        class="w-6 h-6">
        ${inner}
      </svg>
    `);
  }

  openAddStepDialog(existingStep: any = null, index: number | null = null) {
    const dialogRef = this.dialog.open(AddStepDialogComponent, {
      width: '600px',
      data: existingStep
        ? { name: existingStep.name, icon: existingStep.icon }
        : {}
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        if (index !== null) {
          // Edit mode → replace existing
          this._applyJobStepperStatusService.addedSteps[index] = {
            ...this._applyJobStepperStatusService.addedSteps[index],
            ...result
          };
        } else {
          // Add new step
          this._applyJobStepperStatusService.addedSteps.forEach(el => el.isSelected = false);
          result.isSelected = true;
          result.isDefault = false;
          this._applyJobStepperStatusService.addedSteps.push(result);
          this._applyJobStepperStatusService.activeStep =
            this._applyJobStepperStatusService.activeStep + 1;
        }
      }
    });
  }

  selectStep(i: number) {
    this._applyJobStepperStatusService.addedSteps.forEach(el => el.previewMode = true);
    this._applyJobStepperStatusService.activeStep = i;
  }

  editStep(i: number, event: Event) {
    event.stopPropagation(); // prevent triggering selectStep
    const step = this._applyJobStepperStatusService.addedSteps[i];
    this.openAddStepDialog(step, i);
  }

  removeStep(i: number, event: Event) {
    event.stopPropagation(); // prevent triggering selectStep
    this._applyJobStepperStatusService.addedSteps.splice(i, 1);
    if (this._applyJobStepperStatusService.activeStep >= this._applyJobStepperStatusService.addedSteps.length) {
      this._applyJobStepperStatusService.activeStep =
        this._applyJobStepperStatusService.addedSteps.length - 1;
    }
  }

  saveStepper() {
    let request: JobStepperRequest = {
      title: this._applyJobStepperStatusService.formTitle,
      description: this._applyJobStepperStatusService.formDescription,
      id: this._applyJobStepperStatusService.formId,
      jobFormSections: this.mapSteps(),
    }
    this._applyJobStepperStatusService.isLoading = true;
    debugger
    if (this._applyJobStepperStatusService.formId) {
      this._applyJobStepperStatusService.updateJobStepper(request).subscribe({
            next: (response) => {
              setTimeout(() => {
              this._applyJobStepperStatusService.isLoading = false;
                this.showJobForms.emit();
                this.refreshForms.emit();
              }, 1000)
            },
            error: (error) => {
              console.error('Error:', error);
            }
          });
    } 
    else {
      this._applyJobStepperStatusService.saveJobStepper(request).subscribe({
        next: (response) => {
          setTimeout(() => {
            this._applyJobStepperStatusService.isLoading = false;
            this.showJobForms.emit();
            this.refreshForms.emit();
          }, 1000)
        },
        error: (error) => {
          console.error('Error:', error);
        }
      });
    }

  }

  mapSteps() {
    let mappedSteps: Array<JobStep> = []
    this._applyJobStepperStatusService.addedSteps.forEach((el, i) => {
      let step: JobStep = {
        name: el.name,
        title: el.name,
        description: el.description,
        code: el.isDefault ? 'Default' : 'Custom',
        position: i,
        icon: el.icon,
        jobFormSectionProperties: this.mapField(el.fields),
        jobFormSections: []
      }
      mappedSteps.push(step);
    });
    
    return mappedSteps;
  }

  mapField(fields: Array<FormField>) {
    let mappedFields: Array<JobStepField> = []
    fields.forEach((el, i) => {
      let field: JobStepField = {
        type: el.type,
        label: el.label,
        required: el.required,
        position: el.position,
        placeHolder: el.placeholder ? el.placeholder : '',
        options: el.options ? el.options : [],
        interviewRequired: el.requiredForInterview ? el.requiredForInterview : [],
        validation: el.validation ? el.validation : {pattern: ''}
      }
      mappedFields.push(field);
    })

    return mappedFields;
  }

  drop(event: CdkDragDrop<any[]>) {
    moveItemInArray(
      this._applyJobStepperStatusService.addedSteps,
      event.previousIndex,
      event.currentIndex
    );
  }

  backToForms() {
    this.showJobForms.emit();
  }
}