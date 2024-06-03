import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CustomMatPaginatorIntlComponent } from './custom-mat-paginator-intl.component';

describe('CustomMatPaginatorIntlComponent', () => {
  let component: CustomMatPaginatorIntlComponent;
  let fixture: ComponentFixture<CustomMatPaginatorIntlComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [CustomMatPaginatorIntlComponent]
    });
    fixture = TestBed.createComponent(CustomMatPaginatorIntlComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
