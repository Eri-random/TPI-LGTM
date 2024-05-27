import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DialogDonarComponent } from './dialog-donar.component';

describe('DialogDonarComponent', () => {
  let component: DialogDonarComponent;
  let fixture: ComponentFixture<DialogDonarComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [DialogDonarComponent]
    });
    fixture = TestBed.createComponent(DialogDonarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
