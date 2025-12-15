import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-payment-cancel',
  imports: [TranslateModule],
  templateUrl: './payment-cancel.component.html',
  styleUrl: './payment-cancel.component.scss'
})
export class PaymentCancelComponent implements OnInit{

  constructor(private _router: Router){}
  ngOnInit(): void {
    setTimeout(() => {
      localStorage.setItem('subscriptionStatus', 'canceled');
      this._router.navigate(['/subscription']);
    }, 3000);
  }

}
