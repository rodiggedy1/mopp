import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UnsubscribedAlertDialogComponent } from './unsubscribed-alert-dialog.component';

describe('UnsubscribedAlertDialogComponent', () => {
  let component: UnsubscribedAlertDialogComponent;
  let fixture: ComponentFixture<UnsubscribedAlertDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UnsubscribedAlertDialogComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UnsubscribedAlertDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
