import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-job-submitted',
  imports: [TranslateModule, CommonModule],
  templateUrl: './job-submitted.component.html',
  styleUrl: './job-submitted.component.scss'
})
export class JobSubmittedComponent {
}