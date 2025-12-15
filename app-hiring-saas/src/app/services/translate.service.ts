import { Inject, Injectable, PLATFORM_ID } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { Subject, take } from 'rxjs';
import { environment } from '../../environments/environment';
import { isPlatformBrowser } from '@angular/common';

@Injectable({
  providedIn: 'root'
})
export class TranslateAppService {

  userSettingsLanguage: string = environment.appDefailtLanguage;
  activeLanguage!: string;
  activeLanguageSubscribe: Subject<string> = new Subject();

  constructor(
    public translate: TranslateService,
    @Inject(PLATFORM_ID) private platformId: Object,
  ) { }

  setTranslateDefault() {
    if (isPlatformBrowser(this.platformId)) {
        const lng = localStorage.getItem('templateLng');
        this.activeLanguage = lng ? lng : this.userSettingsLanguage;
        this.translate.setDefaultLang(lng ? lng : this.userSettingsLanguage);
        this.translate.use(lng ? lng : this.userSettingsLanguage).pipe(take(1)).subscribe(
            {
                next: () =>{
                this.activeLanguageSubscribe.next(this.activeLanguage);
                }
            }
        );
    }
  }

  setTranslate(lng: string) {
    this.activeLanguage = lng;
    this.translate.setDefaultLang(lng);
    this.translate.use(lng).pipe(take(1)).subscribe(
      {
        next: () =>{
            if (isPlatformBrowser(this.platformId)) {
                localStorage.setItem('templateLng', lng);
            }
            this.activeLanguageSubscribe.next(this.activeLanguage);
        }
      }

    );
  }

  getCurrentTranslate() {
    return this.activeLanguage;
  }
}
