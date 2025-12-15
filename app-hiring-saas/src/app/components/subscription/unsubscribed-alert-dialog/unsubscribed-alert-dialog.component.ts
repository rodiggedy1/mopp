import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-unsubscribed-alert-dialog',
  imports: [],
  templateUrl: './unsubscribed-alert-dialog.component.html',
  styleUrl: './unsubscribed-alert-dialog.component.scss'
})
export class UnsubscribedAlertDialogComponent {

  title: string = '';
  description: string = '';

  constructor(
    public dialogRef: MatDialogRef<UnsubscribedAlertDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ){
    let isSubscribed = localStorage.getItem('subscriptionStatus');
    this.getSubscriptionPopupContent(isSubscribed);
  }

  getSubscriptionPopupContent(status: string | null) {
    debugger
    switch (status) {
      case 'canceled':
        this.title = 'Subscription Canceled';
        this.description = 'Your subscription has been canceled. You can renew anytime to continue enjoying premium features.'
        break;
      case 'none':
        this.title = 'Subscription Canceled';
        this.description = 'Your subscription has been canceled. You can renew anytime to continue enjoying premium features.'
        break;
      case 'incomplete':
        this.title = 'Incomplete Subscription';
        this.description = 'It looks like your subscription payment wasn’t completed. Please check your payment method and try again.'
        break;
      case 'incomplete_expired':
        this.title = 'Subscription Expired';
        this.description = 'Your subscription setup wasn’t finished in time and has expired. You can start a new subscription whenever you’re ready.'
        break;
    }
  }

  cancel() {
    this.dialogRef.close();
  }

  subscribe(){
    this.dialogRef.close('subscribe');
  }

}
