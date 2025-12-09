import { Component, ElementRef, Inject, ViewChild } from '@angular/core';
import { MaterialModule } from '../../modules/material/material.module';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { TranslateModule } from '@ngx-translate/core';

export interface VideoDialogData {
  candidateName?: string;
  videoUrl?: string;
}

@Component({
  selector: 'app-video-dialog',
  imports: [MaterialModule, TranslateModule],
  templateUrl: './video-dialog.component.html',
  styleUrl: './video-dialog.component.scss'
})
export class VideoDialogComponent {
  @ViewChild('videoPlayer') videoPlayer!: ElementRef<HTMLVideoElement>;

  constructor(
    public dialogRef: MatDialogRef<VideoDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: VideoDialogData
  ) {}

  closeDialog(): void {
    if (this.videoPlayer?.nativeElement) {
      this.videoPlayer.nativeElement.pause();
    }
    this.dialogRef.close();
  }

  ngOnDestroy() {
    if (this.videoPlayer?.nativeElement) {
      this.videoPlayer.nativeElement.pause();
    }
  }
}
