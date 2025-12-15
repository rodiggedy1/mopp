import { ComponentFixture, TestBed } from '@angular/core/testing';

import { YourBioComponent } from './your-bio.component';

describe('YourBioComponent', () => {
  let component: YourBioComponent;
  let fixture: ComponentFixture<YourBioComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [YourBioComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(YourBioComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
