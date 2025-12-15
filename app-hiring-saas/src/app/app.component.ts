import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { TranslateAppService } from './services/translate.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'hiring-saas-mvp';

  constructor(
    private _translateService: TranslateAppService
  ){
      this._translateService.setTranslateDefault();
  }
}
