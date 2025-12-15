import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CancelSubscribeDialogComponent } from './cancel-subscribe-dialog.component';

describe('CancelSubscribeDialogComponent', () => {
  let component: CancelSubscribeDialogComponent;
  let fixture: ComponentFixture<CancelSubscribeDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CancelSubscribeDialogComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CancelSubscribeDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
