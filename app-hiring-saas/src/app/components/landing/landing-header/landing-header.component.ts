import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-landing-header',
  imports: [TranslateModule, CommonModule],
  templateUrl: './landing-header.component.html',
  styleUrl: './landing-header.component.scss'
})
export class LandingHeaderComponent implements OnInit {

  constructor(private _router: Router) { }

  ngOnInit(): void {}

  onLogin() {
    this._router.navigate(['/auth/login']);
  }

}
