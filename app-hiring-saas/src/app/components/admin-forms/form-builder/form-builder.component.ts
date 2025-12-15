import { CdkDragDrop, DragDropModule, moveItemInArray } from '@angular/cdk/drag-drop';
import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Output, signal } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatChipsModule } from '@angular/material/chips';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { FormField, FormTemplate } from '../../../models/job-stepper.model';
import { ApplyJobStepperStatusService } from '../../apply-job-stepper/apply-job-stepper-status.service';
import { VideoComponent } from '../../apply-job-stepper/video/video.component';
import { MatSelectModule } from '@angular/material/select';

@Component({
  selector: 'app-form-builder',
  imports: [
    FormsModule, 
    ReactiveFormsModule,
    DragDropModule, 
    CommonModule, 
    VideoComponent,
    MatChipsModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MatSelectModule
  ],
  templateUrl: './form-builder.component.html',
  styleUrl: './form-builder.component.scss'
})
export class FormBuilderComponent {
  fieldTypes = [
    { type: 'text', label: 'Text Input', description: 'Single line text' },
    { type: 'email', label: 'Email', description: 'Email address' },
    { type: 'phone', label: 'Phone', description: 'Phone number' },
    { type: 'number', label: 'Number', description: 'Numeric input' },
    { type: 'textarea', label: 'Long Text', description: 'Multi-line text' },
    { type: 'about-textarea', label: 'Cleaning experience', description: 'Multi-line text' },
    { type: 'select', label: 'Dropdown', description: 'Select from options' },
    { type: 'checkbox', label: 'Checkboxes', description: 'Multiple selection' },
    { type: 'radio', label: 'Radio Buttons', description: 'Single selection' },
    { type: 'date', label: 'Date', description: 'Date picker' },
    { type: 'file', label: 'Image Upload', description: 'File attachment' },
    { type: 'tags', label: 'Skills', description: 'Multiple selection' },
    { type: 'video', label: 'Video', description: 'Record a video' },
  ];

  currentForm: FormTemplate = {
    id: '',
    name: '',
    description: '',
    fields: [],
    createdAt: new Date(),
    updatedAt: new Date(),
    isDefault: false
  };

  selectedField: FormField | null = null;
  showPreview = false;

  // Track tags for each field (for preview)
  fieldTags: { [fieldId: string]: string[] } = {};
  currentTagInput: { [fieldId: string]: string } = {};

  @Output() savedForm: EventEmitter<FormTemplate> = new EventEmitter()
  selectedTags: { [fieldId: string]: string[] } = {};
  radioButtonValues: string[] = [];
  
  constructor(
    public _applyJobStepperStatusService: ApplyJobStepperStatusService,
  ){}

  ngOnInit() {
    this.currentForm.id = this.generateId();

    if (this._applyJobStepperStatusService.activeStep > 0) {
      this.currentForm = {
        id: this.generateId(),
        name: this._applyJobStepperStatusService.addedSteps[this._applyJobStepperStatusService.activeStep].name,
        description: this._applyJobStepperStatusService.addedSteps[this._applyJobStepperStatusService.activeStep].description,
        fields: this._applyJobStepperStatusService.addedSteps[this._applyJobStepperStatusService.activeStep].fields,
        createdAt: new Date(),
        updatedAt: new Date(),
        isDefault: this._applyJobStepperStatusService.addedSteps[this._applyJobStepperStatusService.activeStep].isDefault
      }
    }

    // Initialize tags for existing fields
    this.currentForm.fields.forEach(field => {
      if (field.type === 'tags') {
        this.fieldTags[field.id] = [];
        this.currentTagInput[field.id] = '';
      }
      
      // Initialize requiredForInterview from interviewRequired data
      this.initializeRequiredForInterview(field);
    });
  }

  private initializeRequiredForInterview(field: FormField) {
    if (field.interviewRequired && field.options) {
      // Map interviewRequired values to requiredForInterview for UI
      field.requiredForInterview = field.options.filter(option => 
        field.interviewRequired!.includes(option)
      );
    } else if (!field.requiredForInterview) {
      field.requiredForInterview = [];
    }
  }

  onFieldDrop(event: CdkDragDrop<any[]>) {

    if (event.previousContainer === event.container) {
      // Reordering within the form
      moveItemInArray(event.container.data, event.previousIndex, event.currentIndex);
      this.updateFieldPositions();
    } else {
      // Adding new field from palette
      const draggedData = event.item.data || event.previousContainer.data[event.previousIndex];

      if (draggedData) {
        const newField = this.createField(draggedData.type, draggedData.label);

        // Insert at the correct position
        if (event.currentIndex >= this.currentForm.fields.length) {
          this.currentForm.fields.push(newField);
        } else {
          this.currentForm.fields.splice(event.currentIndex, 0, newField);
        }

        this.updateFieldPositions();
        this.selectedField = newField;
      }
    }
  }

  createField(type: string, label: string): FormField {
    const field: FormField = {
      id: this.generateId(),
      type: type as FormField['type'],
      label: label,
      required: false,
      position: this.currentForm.fields.length
    };

    // Set default placeholder
    if (this.needsPlaceholder(field.type)) {
      field.placeholder = `Enter ${label.toLowerCase()}`;
    }

    // Initialize options for select/checkbox/radio
    if (this.needsOptions(field.type)) {
      field.options = ['Option 1', 'Option 2'];
    }

    // Initialize validation for text-based fields
    if (this.needsValidation(field.type)) {
      field.validation = {};
    }

    // Initialize tags storage for tags field
    if (field.type === 'tags') {
      this.fieldTags[field.id] = [];
      this.currentTagInput[field.id] = '';
      field.placeholder = 'Add tags...';
    }

    return field;
  }

  editField(field: FormField) {
    this.selectedField = field;
    // Initialize requiredForInterview array if it doesn't exist
    if (!this.selectedField.requiredForInterview) {
      this.selectedField.requiredForInterview = [];
    }
  }

  removeField(index: number) {
    const currentField = this.currentForm.fields[index];
      this.currentForm.fields.splice(index, 1);
      this.updateFieldPositions();
      if (this.selectedField === currentField) {
        this.selectedField = null;
      }
      // Clean up tags data
      // delete this.fieldTags[fieldId];
      // delete this.currentTagInput[fieldId];
  }

  addOption() {
    if (this.selectedField && this.selectedField.options) {
      this.selectedField.options.push(`Option ${this.selectedField.options.length + 1}`);
      
      // Initialize requiredForInterview array if it doesn't exist
      if (!this.selectedField.requiredForInterview) {
        this.selectedField.requiredForInterview = [];
      }
    }
  }

  removeOption(index: number) {
    if (this.selectedField && this.selectedField.options) {
      const removedOption = this.selectedField.options[index];
      this.selectedField.options.splice(index, 1);
      
      // Also remove from requiredForInterview if it exists there
      if (this.selectedField.requiredForInterview) {
        const requiredIndex = this.selectedField.requiredForInterview.indexOf(removedOption);
        if (requiredIndex > -1) {
          this.selectedField.requiredForInterview.splice(requiredIndex, 1);
        }
      }
    }
  }

  toggleRequiredForInterview(index: number, event: any) {
    if (this.selectedField && this.selectedField.options) {
      const optionValue = this.selectedField.options[index];
      
      // Initialize requiredForInterview array if it doesn't exist
      if (!this.selectedField.requiredForInterview) {
        this.selectedField.requiredForInterview = [];
      }
      
      if (event.target.checked) {
        // Add to requiredForInterview if not already present
        if (!this.selectedField.requiredForInterview.includes(optionValue)) {
          this.selectedField.requiredForInterview.push(optionValue);
        }
      } else {
        // Remove from requiredForInterview
        const requiredIndex = this.selectedField.requiredForInterview.indexOf(optionValue);
        if (requiredIndex > -1) {
          this.selectedField.requiredForInterview.splice(requiredIndex, 1);
        }
      }
    }
  }

  isOptionRequiredForInterview(optionValue: string): boolean {
    if (!this.selectedField) {
      return false;
    }
    
    // First check if we have interviewRequired data from DB
    if (this.selectedField.interviewRequired) {
      return this.selectedField.interviewRequired.includes(optionValue);
    }
    
    // Fallback to requiredForInterview for new fields
    if (this.selectedField.requiredForInterview) {
      return this.selectedField.requiredForInterview.includes(optionValue);
    }
    
    return false;
  }

  needsPlaceholder(type: string): boolean {
    return ['text', 'email', 'phone', 'number', 'textarea', 'tags'].includes(type);
  }

  needsOptions(type: string): boolean {
    return ['select', 'checkbox', 'radio', 'tags'].includes(type);
  }

  needsValidation(type: string): boolean {
    return ['text', 'email', 'phone', 'textarea', 'about-textarea'].includes(type);
  }

  updateFieldPositions() {
    this.currentForm.fields.forEach((field, index) => {
      field.position = index;
    });
  }

  previewForm() {
    this.showPreview = true;
  }

  closePreview() {
    this.showPreview = false;
  }

  saveForm() {
    // Sync the UI state (requiredForInterview) to the DB property (interviewRequired)
    this.currentForm.fields.forEach(field => {
      if (field.requiredForInterview && field.requiredForInterview.length > 0) {
        field.interviewRequired = [...field.requiredForInterview];
      } else {
        field.interviewRequired = [];
      }
    });
    
    this.currentForm.updatedAt = new Date();
    this.savedForm.emit(this.currentForm);
  }

  trackByFieldId(index: number, field: FormField): string {
    return field.id;
  }

  private generateId(): string {
    return Math.random().toString(36).substr(2, 9);
  }

  trackByIndex(index: number, item: any): number {
  return index;
  }

  toggleTag(fieldId: string, option: string) {
    if (!this.selectedTags[fieldId]) this.selectedTags[fieldId] = [];

    if (this.selectedTags[fieldId].includes(option)) {
      this.selectedTags[fieldId] = this.selectedTags[fieldId].filter(o => o !== option);
    } else if (this.selectedTags['undefined']?.length < 3) {
      this.selectedTags[fieldId].push(option);
    }
  }

  getRadioValue(index: number): string {
    if (!this.radioButtonValues[index]) {
      this.radioButtonValues[index] = ''; // no default, unless you want 'yes'
    }
    return this.radioButtonValues[index];
  }

  setRadioButtonValue(index: number, option: string) {
    this.radioButtonValues[index] = option.toLowerCase();
  }
}