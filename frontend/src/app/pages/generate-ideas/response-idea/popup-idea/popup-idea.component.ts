import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-popup-idea',
  templateUrl: './popup-idea.component.html',
  styleUrls: ['./popup-idea.component.css']
})
export class PopupIdeaComponent {

  constructor(
    public dialogRef: MatDialogRef<PopupIdeaComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
  ) {}

  onNoClick(): void {
    this.dialogRef.close();
  }

  onGenerateWithCurrentData(): void {
    this.dialogRef.close('generateWithCurrentData');
  }

  onLoadNewData(): void {
    this.dialogRef.close('loadNewData');
  }
}
