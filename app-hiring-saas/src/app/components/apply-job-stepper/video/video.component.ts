import { Component, ElementRef, ViewChild, OnInit, ChangeDetectorRef, OnDestroy, EventEmitter, Output, Input } from '@angular/core';
import { ApplyJobStepperStatusService } from '../apply-job-stepper-status.service';
import { TranslateModule } from '@ngx-translate/core';
import { CommonModule } from '@angular/common';
import { take, takeUntil } from 'rxjs';
import { Router } from '@angular/router';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { JobResponse } from '../../../models/job-stepper.model';

@Component({
  selector: 'app-video',
  imports: [TranslateModule, CommonModule, MatProgressSpinnerModule],
  templateUrl: './video.component.html',
  styleUrls: ['./video.component.scss']
})
export class VideoComponent implements OnInit, OnDestroy {

  @ViewChild('preview') preview!: ElementRef<HTMLVideoElement>;
  @ViewChild('recordingPreview') recordingPreview!: ElementRef<HTMLVideoElement>;
  @ViewChild('recordedPreview') recordedPreview!: ElementRef<HTMLVideoElement>;
  @ViewChild('videoUpload') videoUpload!: ElementRef<HTMLInputElement>;

  mediaRecorder!: MediaRecorder;
  recordedChunks: BlobPart[] = [];
  recordedVideoURL: string | null = null;
  stream!: MediaStream;
  isRecording = false;

  isLoading: boolean = false;
  errorMessage = '';
  recodVideoLater: boolean = false;
  recordingStopped = false;
  @Output() recordedVideo: EventEmitter<string> = new EventEmitter();

  // New UI properties
  currentStep: 'ready' | 'recording' | 'review' = 'ready';
  recordingTime = '0:00';
  finalRecordingTime = '0:00';
  recordingProgress = 0;
  showReviewOverlay = true;
  selectedJob!: JobResponse | undefined;
  
  // Timer properties
  private recordingTimer: any;
  private recordingStartTime: number = 0;
  private maxRecordingTime = 60; // 60 seconds

  @Input() buttonAction: string = '';
  @Output() buttonActionClicked: EventEmitter<string> = new EventEmitter();
  @Input() loading: boolean = false;

  constructor(
    private _applyJobStepperStatusService: ApplyJobStepperStatusService,
    private _cdRef: ChangeDetectorRef,
    private _router: Router,
  ) {}

  ngOnInit() {
    // Initialize any required data
    this.selectedJob = this._applyJobStepperStatusService.selectedJob;
  }

  ngOnDestroy() {
    // Clean up timer and stream
    if (this.recordingTimer) {
      clearInterval(this.recordingTimer);
    }
    if (this.stream) {
      this.stream.getTracks().forEach(track => track.stop());
    }
  }

  ngAfterViewInit() {
    const savedVideo = this._applyJobStepperStatusService.getVideosFormValues();
    if (savedVideo) {
      this.recordedVideoURL = savedVideo;
      if (this.recordedPreview?.nativeElement) {
        this.recordedPreview.nativeElement.src = this.recordedVideoURL!;
        this.recordedPreview.nativeElement.hidden = false;
      }
      this.recordingStopped = true;
      this.currentStep = 'review';
      this.showReviewOverlay = false;
      this._cdRef.detectChanges();
    }
  }

  async startRecording() {
  this.recodVideoLater = false;
  this.recordedVideoURL = null;
  this.recordedChunks = [];
  this.recordingTime = '0:00';
  this.recordingProgress = 0;

  try {
    // Always re-request media for fresh start (solves stuck camera)
    this.stream = await navigator.mediaDevices.getUserMedia({
      video: { width: 1280, height: 720 },
      audio: {
        echoCancellation: true,
        noiseSuppression: true,
        autoGainControl: true
      }
    });

    // Assign stream to live preview
    if (this.recordingPreview?.nativeElement) {
      this.recordingPreview.nativeElement.srcObject = this.stream;
       this.recordingPreview.nativeElement.muted = true;
      this.recordingPreview.nativeElement.hidden = false;
    }

    // Improved MediaRecorder config
    const options = {
      mimeType: 'video/webm;codecs=vp8,opus',
      audioBitsPerSecond: 128000,
      videoBitsPerSecond: 2500000
    };
    this.mediaRecorder = new MediaRecorder(this.stream, options);

    this.mediaRecorder.ondataavailable = (event: BlobEvent) => {
      if (event.data && event.data.size > 0) this.recordedChunks.push(event.data);
    };

    this.mediaRecorder.onstop = async () => {
      // Assemble final blob
      const blob = new Blob(this.recordedChunks, { type: 'video/webm' });
      this.recordedVideoURL = URL.createObjectURL(blob);
      this.recordedVideo.emit(this.recordedVideoURL);

      // Show recorded video
      if (this.recordedPreview?.nativeElement) {
        this.recordedPreview.nativeElement.src = this.recordedVideoURL;
        this.recordedPreview.nativeElement.hidden = false;
      }

      // Hide live preview
      if (this.recordingPreview?.nativeElement) {
        this.recordingPreview.nativeElement.srcObject = null;
        this.recordingPreview.nativeElement.hidden = true;
      }

      // ✅ Stop all tracks (camera + mic)
      this.stream.getTracks().forEach(track => track.stop());
      this.stream = null!;

      // UI state updates
      this.isRecording = false;
      this._applyJobStepperStatusService.setVideosFormValues(this.recordedVideoURL);
      this._applyJobStepperStatusService.setVideosFormValid(true);
      this.currentStep = 'review';
      this.showReviewOverlay = true;

      setTimeout(() => {
        this.showReviewOverlay = false;
        this._cdRef.detectChanges();
      }, 2000);

      if (this.recordingTimer) clearInterval(this.recordingTimer);
    };

    // Start recording every second for smoother audio
    this.mediaRecorder.start(1000);
    this.isRecording = true;
    this.currentStep = 'recording';
    this.recordingStartTime = Date.now();
    this.startRecordingTimer();

  } catch (error) {
    console.error('Error starting recording:', error);
    this.errorMessage = 'Unable to access camera and microphone. Please check your permissions.';
  }
}

  private startRecordingTimer() {
    this.recordingTimer = setInterval(() => {
      const elapsed = Math.floor((Date.now() - this.recordingStartTime) / 1000);
      this.recordingTime = this.formatTime(elapsed);
      this.recordingProgress = (elapsed / this.maxRecordingTime) * 100;

      // Auto-stop at max time
      if (elapsed >= this.maxRecordingTime) {
        this.stopRecording();
      }

      this._cdRef.detectChanges();
    }, 1000);
  }

  private formatTime(seconds: number): string {
    const mins = Math.floor(seconds / 60);
    const secs = seconds % 60;
    return `${mins}:${secs.toString().padStart(2, '0')}`;
  }

  stopRecording() {
    if (this.mediaRecorder && this.isRecording) {
      this.finalRecordingTime = this.formatTime(
        Math.floor((Date.now() - this.recordingStartTime) / 1000)
      );
      this.isRecording = false;
      this.mediaRecorder.stop();
      if (this.recordingTimer) clearInterval(this.recordingTimer);
    }
  }

  retakeRecording() {
    // Reset all states
    this.recordedVideoURL = null;
    this.recordingStopped = false;
    this.recordingTime = '0:00';
    this.recordingProgress = 0;
    this.showReviewOverlay = true;

    // Hide recorded preview
    if (this.recordedPreview?.nativeElement) {
      this.recordedPreview.nativeElement.hidden = true;
    }

    // Show preview again
    if (this.preview?.nativeElement) {
      this.preview.nativeElement.hidden = false;
    }

    // Reset service state
    this._applyJobStepperStatusService.setVideosFormValues(null);
    this._applyJobStepperStatusService.setVideosFormValid(false);

    // Clean up timer if running
    if (this.recordingTimer) {
      clearInterval(this.recordingTimer);
    }

    // Go back to ready state
    this.currentStep = 'recording';
    this.startRecording();
    this._cdRef.detectChanges();
  }

  continueAfterRecording() {
    this.submit();
  }

  recordLater() {
    this.recodVideoLater = true;
    this.currentStep = 'ready'
    this._cdRef.detectChanges();
    this._applyJobStepperStatusService.setVideosFormValid(true);
  }

  onVideoSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files[0]) {
      const file = input.files[0];
      this.recordedVideoURL = URL.createObjectURL(file);

      if (this.recordedPreview?.nativeElement) {
        this.recordedPreview.nativeElement.src = this.recordedVideoURL;
        this.recordedPreview.nativeElement.hidden = false;
      }

      if (this.preview?.nativeElement) {
        this.preview.nativeElement.hidden = true;
      }

      // Save uploaded video in service and mark valid
      this._applyJobStepperStatusService.setVideosFormValues(this.recordedVideoURL);
      this._applyJobStepperStatusService.setVideosFormValid(true);

      // Set to review state
      this.currentStep = 'review';
      this.showReviewOverlay = false;
      this.recordingStopped = true;
    }
  }

  async submit() {
    const basicInfo = this._applyJobStepperStatusService.getBasicInfoFormValues();
    const requirements = this._applyJobStepperStatusService.getRequirementsFormValues();
    const bioBase64 = this._applyJobStepperStatusService.getYourBioFormValues();
    const videoUrl = this._applyJobStepperStatusService.getVideosFormValues();

    const formData = new FormData();

    //  Basic Info
    formData.append("JobDetailsId", String(1)); // GET FROM API

    formData.append("FirstName", basicInfo.firstName || "");
    formData.append("LastName", basicInfo.lastName || "");
    formData.append("EmailAddress", basicInfo.email || "");
    formData.append("PhoneNumber", basicInfo.phone || "");
    formData.append("StreetAddress", basicInfo.streetAddress || "");
    formData.append("Apartment", basicInfo.aptSuite || "");
    formData.append("City", basicInfo.city || "");
    formData.append("State", basicInfo.state || "");
    formData.append("ZipCode", basicInfo.zip || "");

    // Requirements
    formData.append("HasCleaningExperience", String(requirements.hasCleaningExperience));
    formData.append("HasBankAccountForDirectDeposit", String(requirements.hasBankAccount));
    formData.append("IsLegallyAuthorizedToWorkInUS", String(requirements.isAuthorizedToWork));
    formData.append("AllowsConsentToFreeBackgroundCheck", String(requirements.consentBackgroundCheck));
    formData.append("CleaningExperienceDescription", requirements.cleaningExperienceDescription || "");

    // Example: If you have a selected JobDetailsId from somewhere
    formData.append("JobDetailsId", "123"); // replace with real value

    // Profile Picture (convert base64 → File)
    if (bioBase64) {
      const profilePicFile = this.base64ToFile(bioBase64, "profile.png");
      formData.append("ProfilePicture", profilePicFile);
    }

    // Video (convert blob URL → File)
    if (videoUrl) {
      const videoFile = await this.blobUrlToFile(videoUrl, "application-video.webm");
      formData.append("ApplicationVideo", videoFile);
    }

    this.isLoading = true;
    this._applyJobStepperStatusService.sendJobApplication(formData)
      .pipe(take(1))
      .subscribe({
        next: (response) => {
          this.isLoading = false;
          this._router.navigate(['../../job-submitted']);
        },
        error: (error) => {
          this.isLoading = false;
          if (error.error.detail && error.error.detail === 'Account not verified'){
            // Handle verification error
          }
          error.error.detail ? this.errorMessage = error.error.detail : this.handleLoginError(error);
        }
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

  // Convert blob URL to File
  async blobUrlToFile(blobUrl: string, filename: string): Promise<File> {
    const response = await fetch(blobUrl);
    const blob = await response.blob();
    return new File([blob], filename, { type: blob.type });
  }

  private handleLoginError(error: any): void {
    if (error.status === 401) {
      this.errorMessage = 'Invalid email or password. Please try again.';
    } else if (error.status === 429) {
      this.errorMessage = 'Too many login attempts. Please try again later.';
    } else if (error.status === 0) {
      this.errorMessage = 'Unable to connect to server. Please check your internet connection.';
    } else {
      this.errorMessage = error.error?.message || 'Login failed. Please try again.';
    }
  }

  onContinueStep() {
    this.buttonActionClicked.emit('continue');
  }

  submitForm() {
    this.buttonActionClicked.emit('submit');
  }
}