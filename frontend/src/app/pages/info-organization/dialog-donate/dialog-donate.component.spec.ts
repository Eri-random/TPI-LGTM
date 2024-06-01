import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DialogDonateComponent } from './dialog-donate.component';

describe('DialogDonarComponent', () => {
  let component: DialogDonateComponent;
  let fixture: ComponentFixture<DialogDonateComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [DialogDonateComponent]
    });
    fixture = TestBed.createComponent(DialogDonateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
