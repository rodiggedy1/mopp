import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialogModule } from '@angular/material/dialog';
import { DragDropModule } from '@angular/cdk/drag-drop'

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    MatDialogModule,
    DragDropModule
  ],
  exports: [
    MatDialogModule,
    DragDropModule
  ]
})
export class MaterialModule { }
