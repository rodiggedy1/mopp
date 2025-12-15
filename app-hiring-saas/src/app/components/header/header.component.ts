import { Component } from '@angular/core';
import { LanguageComponent } from '../language/language.component';

@Component({
  selector: 'app-header',
  imports: [LanguageComponent],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss'
})
export class HeaderComponent {

}
