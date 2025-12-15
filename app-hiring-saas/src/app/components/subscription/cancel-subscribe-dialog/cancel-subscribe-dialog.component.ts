import { CommonModule } from '@angular/common';
import { Component, Inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { AuthService } from '../../../services/auth.service';
import { take } from 'rxjs';
import { LookupValue, paymentCancelAlert, paymentSendImprovement } from '../../../models/shared.model';

@Component({
  selector: 'app-cancel-subscribe-dialog',
  imports: [CommonModule, FormsModule],
  templateUrl: './cancel-subscribe-dialog.component.html',
  styleUrl: './cancel-subscribe-dialog.component.scss'
})
export class CancelSubscribeDialogComponent implements OnInit {

  activeStep: number = 1;
  selectedReason: string = '';
  missingFeaturesValue: string = '';
  otherReasonValue: string = '';
  improvementAction: paymentSendImprovement = new paymentSendImprovement();
  onCloseAlert: paymentCancelAlert = new paymentCancelAlert();
  noteTypes: LookupValue[] = [];

  constructor(
    public dialogRef: MatDialogRef<CancelSubscribeDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private _authService: AuthService
  ){}

  ngOnInit(): void {
    this._authService.getNoteTypes().pipe(take(1)).subscribe({
      next: (res) => {
        this.noteTypes = res;
      }
    });
  }

  closeDialog() {
    this.dialogRef.close();
  }

  changeStepTo(step: number) {
    this.activeStep = step;
  }

  sendImprovement(type: string) {
    if(type === 'FeatureRequest') {
      this.improvementAction.message = this.missingFeaturesValue;
      this.improvementAction.noteType = this.noteTypes.find(nt => nt.name === 'FeatureRequest')?.id;
    } else if(type === 'CancellationReason') {
      this.improvementAction.message = this.otherReasonValue;
      this.improvementAction.noteType = this.noteTypes.find(nt => nt.name === 'CancellationReason')?.id;
    }
    this._authService.sendImprove(this.improvementAction).pipe(take(1)).subscribe({
      next: (res) => {
        this.onCloseAlert.color = 'green';
        this.onCloseAlert.message = 'Thank you for your feedback! We appreciate your input and will use it to improve our services.';
        this.dialogRef.close(this.onCloseAlert);
      }
    });
  }

  extendTrialOneMonth() {
    this._authService.extendTrialOneMonth().pipe(take(1)).subscribe({
      next: (res) => {
        this.onCloseAlert.color = 'blue';
        this.onCloseAlert.message = 'Your trial has been extended by one month.';
        this.dialogRef.close(this.onCloseAlert);
      }
    });
  }

  pasueForTwoMonts() {
    this._authService.pausePaymentForTwoMonths().pipe(take(1)).subscribe({
      next: (res) => {
        this.onCloseAlert.color = 'blue';
        this.onCloseAlert.message = 'Your subscription has been paused for 2 months.';
        this.dialogRef.close(this.onCloseAlert);
      }
    });
  }

  claimFiftyPercentOff() {
    this._authService.claimFiftyPercentOff().pipe(take(1)).subscribe({
      next: (res) => {
        this.onCloseAlert.color = 'green';
        this.onCloseAlert.message = 'You have successfully claimed 50% off for the next 3 months.';
        this.dialogRef.close(this.onCloseAlert);
      }
    });
  }

  cancelSubscription() {
    this._authService.cancelPaymentSubscription().pipe(take(1)).subscribe({
      next: (res) => {
        this.onCloseAlert.color = 'green';
        this.onCloseAlert.message = 'Your subscription has been cancelled successfully.';
        this.dialogRef.close(this.onCloseAlert);
      }
    });
  }
}
