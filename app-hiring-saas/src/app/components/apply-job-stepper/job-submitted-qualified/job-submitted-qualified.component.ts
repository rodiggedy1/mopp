import { AfterViewInit, Component } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { ApplyJobStepperStatusService } from '../apply-job-stepper-status.service';
import { CommonModule } from '@angular/common';
declare const Calendly: any;

@Component({
  selector: 'app-job-submitted-qualified',
  imports: [TranslateModule, CommonModule],
  templateUrl: './job-submitted-qualified.component.html',
  styleUrl: './job-submitted-qualified.component.scss'
})
export class JobSubmittedQualifiedComponent implements AfterViewInit{
  calendlyProfileUrl: string | null = null;
  showCalendlyPopup = false;
  externalCalendarUrl: string | null = null;

  constructor(
      public _applyJobStepperStatusService: ApplyJobStepperStatusService,
    ) {
      this.calendlyProfileUrl = this._applyJobStepperStatusService.calendlyUrl;
      this.externalCalendarUrl = this._applyJobStepperStatusService.externalCalendarUrl;
    }

    ngAfterViewInit(): void {
      // Ensure Calendly script is available globally
      if (typeof Calendly === 'undefined') {
        const script = document.createElement('script');
        script.src = 'https://assets.calendly.com/assets/external/widget.js';
        script.async = true;
        document.body.appendChild(script);
      }
    }

    openCalendlyPopup() {
      this.showCalendlyPopup = true;
      setTimeout(() => this.loadCalendlyWidget(), 50);
    }

    closeCalendlyPopup() {
      this.showCalendlyPopup = false;
    }

    private loadCalendlyWidget() {
      if (!this.calendlyProfileUrl) return;
      const parent = document.getElementById('calendly-embed');
      if (!parent) return;

      if (typeof Calendly !== 'undefined') {
        Calendly.initInlineWidget({
          url: this.calendlyProfileUrl,
          parentElement: parent,
        });
      } else {
        console.error('Calendly script not loaded yet');
      }
    }

    handleCalendarClick() {
      if (this.calendlyProfileUrl) {
        this.openCalendlyPopup();
      } else if (this.externalCalendarUrl) {
        window.open(this.externalCalendarUrl, '_blank');
      }
    }
}
