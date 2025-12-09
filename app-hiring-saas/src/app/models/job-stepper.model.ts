export interface FormField {
  id: string;
  type: 'text' | 'email' | 'phone' | 'number' | 'textarea' | 'select' | 'checkbox' | 'radio' | 'date' | 'file' | 'video' | 'tags';
  label: string;
  placeholder?: string;
  required: boolean;
  options?: string[];
  interviewRequired?: string[];
  requiredForInterview?: string[];
  position: number;
  validation?: {
    minLength?: number;
    maxLength?: number;
    pattern?: string;
  };
}
  
export interface FormTemplate {
  id: string;
  name: string;
  fields: FormField[];
  createdAt: Date;
  updatedAt: Date;
  description: string;
  isDefault: boolean
}

export interface JobStepperRequest {
    title: string;
    description: string;
    jobFormSections: Array<JobStep>;
    id: number | null;
}

export interface JobStepperResponse {
  items: jobForm[];
  pageNumber: number;
  totalPages: number;
  totalCount: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export interface JobResponse {
  id: number;
  title: string;
  description: string;
  location: string;
  employmentType: string;
  price: number;
  created: Date | string;
  jobForm: jobForm,
  uniqueHash: string,
  creator: JobCreator;
}

export interface JobCreator {
  id: number;
  firstName: string;
  lastName: string;
  picture: string | null;
  calendlyProfileUrl : string | null;
}

export interface jobForm {
  title: string | null;
  jobFormSections: Array<JobStep>;
  description: string;
  id: number;
  createdBy: number,
  uniqueHash: string
}

export interface JobStep {
  name: string;
  title: string | null;
  description: string;
  code: string;
  position: number;
  icon: any;
  jobFormSectionProperties: Array<JobStepField>
  jobFormSections: Array<JobStepField>
}

export interface JobStepField {
  type: string;
  label: string;
  required: boolean;
  position: number;
  placeHolder: string;
  options: string[];
  interviewRequired: string[];
  validation: any;
  selectedOption?: string | null;
}

export interface CalendlyUserCredentials {
    calendlyClientId: string | null;
    calendlyClientSecret: string | null;
    calendlyAccessToken: string | null;
    calendlyRefreshToken: string | null;
    calendlyCode: string | null;
    calendlyRedirectUri: string | null;
    CalendlyTokenExpiresAt?: string | null;
}