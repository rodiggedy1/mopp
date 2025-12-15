import { CommonModule } from '@angular/common';
import { Component, Inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';

@Component({
  selector: 'app-add-step-dialog',
  imports: [CommonModule, FormsModule],
  templateUrl: './add-step-dialog.component.html',
  styleUrl: './add-step-dialog.component.scss'
})
export class AddStepDialogComponent {

  newStepName = '';
  selectedIcon: SafeHtml | null = null;
  icons: SafeHtml[] = [];

  constructor(
    private sanitizer: DomSanitizer,
    public dialogRef: MatDialogRef<AddStepDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    this.icons = [
      this.svg(`<svg _ngcontent-ng-c2797985867="" xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" class="lucide lucide-layout-dashboard h-5 w-5 shrink-0"><rect _ngcontent-ng-c2797985867="" width="7" height="9" x="3" y="3" rx="1"></rect><rect _ngcontent-ng-c2797985867="" width="7" height="5" x="14" y="3" rx="1"></rect><rect _ngcontent-ng-c2797985867="" width="7" height="9" x="14" y="12" rx="1"></rect><rect _ngcontent-ng-c2797985867="" width="7" height="5" x="3" y="16" rx="1"></rect></svg>`),
      this.svg(`<svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="1.5" class="h-5 w-5 shrink-0"><path stroke-linecap="round" stroke-linejoin="round" d="M3 9.75L12 3l9 6.75V21a.75.75 0 01-.75.75h-5.25a.75.75 0 01-.75-.75v-4.5a.75.75 0 00-.75-.75h-3a.75.75 0 00-.75.75v4.5a.75.75 0 01-.75.75H3.75A.75.75 0 013 21V9.75z"></path></svg>`),
      this.svg(`<svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor" class="h-5 w-5 shrink-0"><path stroke-linecap="round" stroke-linejoin="round" d="M17.982 18.725A7.488 7.488 0 0 0 12 15.75a7.488 7.488 0 0 0-5.982 2.975m11.963 0a9 9 0 1 0-11.963 0m11.963 0A8.966 8.966 0 0 1 12 21a8.966 8.966 0 0 1-5.982-2.275M15 9.75a3 3 0 1 1-6 0 3 3 0 0 1 6 0Z"></path></svg>`),
      this.svg(`<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" class="h-5 w-5 shrink-0"><path d="m9 11 3 3L22 4"></path><path d="M21 12v7a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h11"></path></svg>`),
      this.svg(`<svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor" class="h-5 w-5 shrink-0"><path stroke-linecap="round" stroke-linejoin="round" d="M12 6v12m-3-2.818.879.659c1.171.879 3.07.879 4.242 0 1.172-.879 1.172-2.303 0-3.182C13.536 12.219 12.768 12 12 12c-.725 0-1.45-.22-2.003-.659-1.106-.879-1.106-2.303 0-3.182s2.9-.879 4.006 0l.415.33M21 12a9 9 0 1 1-18 0 9 9 0 0 1 18 0Z"></path></svg>`),
      this.svg(`<svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor" class="h-5 w-5 shrink-0"><path stroke-linecap="round" stroke-linejoin="round" d="M6.827 6.175A2.31 2.31 0 0 1 5.186 7.23c-.38.054-.757.112-1.134.175C2.999 7.58 2.25 8.507 2.25 9.574V18a2.25 2.25 0 0 0 2.25 2.25h15A2.25 2.25 0 0 0 21.75 18V9.574c0-1.067-.75-1.994-1.802-2.169a47.865 47.865 0 0 0-1.134-.175 2.31 2.31 0 0 1-1.64-1.055l-.822-1.316a2.192 2.192 0 0 0-1.736-1.039 48.774 48.774 0 0 0-5.232 0 2.192 2.192 0 0 0-1.736 1.039l-.821 1.316Z"></path><path stroke-linecap="round" stroke-linejoin="round" d="M16.5 12.75a4.5 4.5 0 1 1-9 0 4.5 4.5 0 0 1 9 0ZM18.75 10.5h.008v.008h-.008V10.5Z"></path></svg>  `),
      this.svg(`<svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor" class="h-5 w-5 shrink-0"><path stroke-linecap="round" stroke-linejoin="round" d="m15.75 10.5 4.72-4.72a.75.75 0 0 1 1.28.53v11.38a.75.75 0 0 1-1.28.53l-4.72-4.72M4.5 18.75h9a2.25 2.25 0 0 0 2.25-2.25v-9a2.25 2.25 0 0 0-2.25-2.25h-9A2.25 2.25 0 0 0 2.25 7.5v9a2.25 2.25 0 0 0 2.25 2.25Z"></path></svg>`),
      this.svg(`<svg _ngcontent-ng-c2797985867="" xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" class="h-5 w-5 shrink-0 lucide lucide-users"><path _ngcontent-ng-c2797985867="" d="M16 21v-2a4 4 0 0 0-4-4H6a4 4 0 0 0-4 4v2"></path><circle _ngcontent-ng-c2797985867="" cx="9" cy="7" r="4"></circle><path _ngcontent-ng-c2797985867="" d="M22 21v-2a4 4 0 0 0-3-3.87"></path><path _ngcontent-ng-c2797985867="" d="M16 3.13a4 4 0 0 1 0 7.75"></path></svg>`),
      this.svg(`<svg _ngcontent-ng-c2797985867="" xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" class="h-5 w-5 shrink-0 lucide lucide-kanban"><path _ngcontent-ng-c2797985867="" d="M6 5v11"></path><path _ngcontent-ng-c2797985867="" d="M12 5v6"></path><path _ngcontent-ng-c2797985867="" d="M18 5v14"></path></svg>`),
      this.svg(`<svg _ngcontent-ng-c2797985867="" xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" class="h-5 w-5 shrink-0 lucide lucide-user-check"><path _ngcontent-ng-c2797985867="" d="M16 21v-2a4 4 0 0 0-4-4H6a4 4 0 0 0-4 4v2"></path><circle _ngcontent-ng-c2797985867="" cx="9" cy="7" r="4"></circle><polyline _ngcontent-ng-c2797985867="" points="16 11 18 13 22 9"></polyline></svg>`),
      this.svg(`<svg _ngcontent-ng-c2797985867="" xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" class="h-5 w-5 shrink-0 lucide lucide-file-text"><path _ngcontent-ng-c2797985867="" d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"></path><polyline _ngcontent-ng-c2797985867="" points="14,2 14,8 20,8"></polyline><line _ngcontent-ng-c2797985867="" x1="16" x2="8" y1="13" y2="13"></line><line _ngcontent-ng-c2797985867="" x1="16" x2="8" y1="17" y2="17"></line><polyline _ngcontent-ng-c2797985867="" points="10,9 9,9 8,9"></polyline></svg>`),
      this.svg(`<svg _ngcontent-ng-c55572515="" xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" class="h-5 w-5 shrink-0 lucide lucide-calendar"><path _ngcontent-ng-c55572515="" d="M8 2v4"></path><path _ngcontent-ng-c55572515="" d="M16 2v4"></path><rect _ngcontent-ng-c55572515="" width="18" height="18" x="3" y="4" rx="2"></rect><path _ngcontent-ng-c55572515="" d="M3 10h18"></path></svg>`),
      this.svg(`<svg _ngcontent-ng-c1679780066="" xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" class="h-5 w-5 shrink-0 lucide lucide-mail2"><rect _ngcontent-ng-c1679780066="" width="20" height="16" x="2" y="4" rx="2"></rect><path _ngcontent-ng-c1679780066="" d="m22 7-10 5L2 7"></path></svg>`),
      this.svg(`<svg _ngcontent-ng-c1679780066="" xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" class="h-5 w-5 shrink-0 lucide lucide-phone2"><path _ngcontent-ng-c1679780066="" d="M22 16.92v3a2 2 0 0 1-2.18 2 19.79 19.79 0 0 1-8.63-3.07 19.5 19.5 0 0 1-6-6 19.79 19.79 0 0 1-3.07-8.67A2 2 0 0 1 4.11 2h3a2 2 0 0 1 2 1.72 12.84 12.84 0 0 0 .7 2.81 2 2 0 0 1-.45 2.11L8.09 9.91a16 16 0 0 0 6 6l1.27-1.27a2 2 0 0 1 2.11-.45 12.84 12.84 0 0 0 2.81.7A2 2 0 0 1 22 16.92z"></path></svg>`),
      this.svg(`<svg _ngcontent-ng-c4018250204="" xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" class="h-5 w-5 shrink-0 lucide lucide-award"><circle _ngcontent-ng-c4018250204="" cx="12" cy="8" r="6"></circle><path _ngcontent-ng-c4018250204="" d="M15.477 12.89 17 22l-5-3-5 3 1.523-9.11"></path></svg>`),
    ];

    if (data?.name) {
      this.newStepName = data.name;
    }
    if (data?.icon) {
      this.selectedIcon = data.icon;
    }
  }

  svg(inner: string): SafeHtml {
    return this.sanitizer.bypassSecurityTrustHtml(`
      <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24"
        viewBox="0 0 24 24" fill="none"
        stroke="currentColor" stroke-width="1.5"
        stroke-linecap="round" stroke-linejoin="round"
        class="w-6 h-6">
        ${inner}
      </svg>
    `);
  }

  save() {
    if (this.newStepName && this.selectedIcon) {
      this.dialogRef.close({
        name: this.newStepName,
        icon: this.selectedIcon,
        isSelected: false,
        previewMode: false,
        fields: []
      });
    }
  }

  cancel() {
    this.dialogRef.close();
  }
}
