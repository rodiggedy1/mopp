import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ScreeningBoardComponent } from './screening-board.component';

describe('ScreeningBoardComponent', () => {
  let component: ScreeningBoardComponent;
  let fixture: ComponentFixture<ScreeningBoardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ScreeningBoardComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ScreeningBoardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
