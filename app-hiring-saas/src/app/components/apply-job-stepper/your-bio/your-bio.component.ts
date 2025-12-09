import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { ApplyJobStepperStatusService } from '../apply-job-stepper-status.service';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-your-bio',
  imports: [ReactiveFormsModule, CommonModule, TranslateModule],
  templateUrl: './your-bio.component.html',
  styleUrl: './your-bio.component.scss'
})
export class YourBioComponent {

  photoURL: string | ArrayBuffer | null = null;

  constructor(
    private _applyJobStepperStatusService: ApplyJobStepperStatusService,
    private _router: Router,
    private _route: ActivatedRoute
  ){
    // const savedValues = this._applyJobStepperStatusService.getYourBioFormValues();
    // if (savedValues) {
    //   this.photoURL = savedValues;
    // }
  }

  onPhotoSelected(event: Event) {
    const input = event.target as HTMLInputElement;

    if (input.files && input.files[0]) {
      const file = input.files[0];

      const reader = new FileReader();
      reader.onload = e => {
        this.photoURL = reader.result;
        // this._applyJobStepperStatusService.setYourBioFormValid(true);
        // this._applyJobStepperStatusService.setYourBioFormValues(this.photoURL);
      };
      reader.readAsDataURL(file);
    }
  }

  // Continue button
  goToVideos() {
    if ( this.photoURL) {
      this._router.navigate(['../videos'], { relativeTo: this._route }); 
    }
  }
}
