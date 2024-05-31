import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SedeComponent } from './headquarters.component';

describe('SedeComponent', () => {
  let component: SedeComponent;
  let fixture: ComponentFixture<SedeComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [SedeComponent]
    });
    fixture = TestBed.createComponent(SedeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
