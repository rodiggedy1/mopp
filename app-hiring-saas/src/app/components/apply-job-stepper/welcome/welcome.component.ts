import { Component, EventEmitter, Output, Renderer2 } from '@angular/core';
import { ApplyJobStepperStatusService } from '../apply-job-stepper-status.service';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-welcome',
  imports: [],
  templateUrl: './welcome.component.html',
  styleUrl: './welcome.component.scss'
})
export class WelcomeComponent {

  @Output() goToNextStep: EventEmitter<void> = new EventEmitter();

  constructor(
    private _applyJobStepperStatusService: ApplyJobStepperStatusService,
    private _router: Router,
    private _route: ActivatedRoute,
    private renderer: Renderer2
  ){}

  ngAfterViewInit(): void {
    const script1 = this.renderer.createElement('script');
    script1.src = 'https://fast.wistia.com/embed/medias/hwmi77abbz.jsonp';
    script1.async = true;
    this.renderer.appendChild(document.body, script1);

    const script2 = this.renderer.createElement('script');
    script2.src = 'https://fast.wistia.com/assets/external/E-v1.js';
    script2.async = true;
    this.renderer.appendChild(document.body, script2);
  }

  goToBasicInfo() {
    // this._router.navigate(['../basic-info'], { relativeTo: this._route }); 
    this.goToNextStep.emit();
  }
}
