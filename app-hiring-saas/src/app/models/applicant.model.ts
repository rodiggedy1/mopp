// Request models
export interface JobApplicationSearchRequest {
  query: string | null;
  statusId: number | null;
  jobDetailsCreatedById: number | null;
  jobDetailsId: number | null;
  status: number[] | null;
  dateFrom: string | null;
  dateTo: string | null;
  paging: PagingRequest;
  sorting: SortingRequest;
}

export interface PagingRequest {
  pageNumber: number;
  pageSize: number;
}

export interface SortingRequest {
  field: number;
  sortOrder: number;
}

// Response models
export interface JobApplicationSearchResponse {
  items: Applicant[];
  pageNumber: number;
  totalPages: number;
  totalCount: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export interface Applicant {
  id: number;
  firstName: string;
  lastName: string;
  emailAddress: string;
  phoneNumber: string;
  streetAddress: string;
  apartment: string | null;
  city: string;
  state: string;
  zipCode: string;
  hasCleaningExperience: boolean;
  hasBankAccountForDirectDeposit: boolean;
  isLegallyAuthorizedToWorkInUS: boolean;
  allowsConsentToFreeBackgroundCheck: boolean;
  cleaningExperienceDescription: string;
  media: MediaCollection;
  profilePicture: MediaItem | null;
  applicationVideo: MediaItem | null;
  created: string | Date;
  kanbanSortOrder: number | null;
  jobDetailsId: number;
  status: StatusLookup;
  jobDetails: JobDetails;
  jobApplicationSections: any;
  statusUpdatedDate: string | Date;
}

export interface MediaCollection {
  items: MediaItem[];
}

export interface MediaItem {
  id: string;
  name: string;
  isMain: boolean;
  sortOrder: number;
  size: number;
  url: string;
  type: number;
  extension: string;
}

export interface StatusLookup {
  id: number;
  name: string | null;
}

export interface JobDetails {
  id: number;
  title: string;
  description: string;
  location: string;
  employmentType: string;
  price: number;
  created: string;
  jobForm: JobForm;
  createdBy: number;
}

export interface JobForm {
  id: number;
  title: string;
  description: string;
  createdBy: number;
}

export interface JobApplicationStatistics {
  applicationsThisMonthCount: number;
  applicationsLastMonthCompared: number;
  inScreeningThisMonthCount: number;
  inScreeningLastMonthCompared: number;
  hiredThisMonthCount: number;
  hiredLastMonthCompared: number;
  successRateThisMonth: number;
  successRateLastMonthCompared: number;
}

export interface HiredStatistics {
  hiredCount: number;
  hiredThisMonthCount: number;
  successRate: number;
}