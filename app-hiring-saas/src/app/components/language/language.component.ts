import { Component } from '@angular/core';
import { TranslateAppService } from '../../services/translate.service';
import { LangChangeEvent, TranslateModule, TranslateService } from '@ngx-translate/core';
import { BaseComponent } from '../base/base.component';
import { takeUntil } from 'rxjs';
import { MatMenuModule } from '@angular/material/menu';
import { CommonModule } from '@angular/common';

interface Language {
  code: string;
  translationKey: string;
  flag: string;
}

@Component({
  selector: 'app-language',
  imports: [MatMenuModule, TranslateModule, CommonModule],
  templateUrl: './language.component.html',
  styleUrl: './language.component.scss'
})
export class LanguageComponent extends BaseComponent {

  languages: Language[] = [];
  selectedLanguage?: Language;

  constructor(
    private _translateService: TranslateAppService,
    private _translationService: TranslateService
  ){
    super();
  }

  ngOnInit(): void {
    this._initializeFlags();
    this._setInitialLanguage();
    this._subscribeToLanguageChanges();
  }

  private _setInitialLanguage(): void {
    this._setSelectedLanguage(this._translateService.getCurrentTranslate());
  }

  private _subscribeToLanguageChanges(): void {
    this._translationService.onLangChange
      .pipe(takeUntil(this._unsubscribeAll))
      .subscribe({
        next: (eventData: LangChangeEvent) => {
          this._setSelectedLanguage(eventData.lang);
        }
      });
  }

  private _setSelectedLanguage(languageCode: string): void {
    this.selectedLanguage = this.languages.find(l => l.code === languageCode)
  }

  setLanguage(language: Language): void {
    this.selectedLanguage = language;
    this._translateService.setTranslate(language.code);
  }

  private _initializeFlags(): void {
    this.languages = [
      { code: 'en', translationKey: 'SHARED.ENGLISH', flag: '../../assets/images/en.png' },
      { code: 'mk', translationKey: 'SHARED.MACEDONIAN', flag: '../../assets/images/mk.png' },
    ];
  }
}
