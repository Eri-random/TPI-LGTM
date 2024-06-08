import { ComponentFixture, TestBed } from '@angular/core/testing';
import { CustomMatPaginatorIntl } from './custom-mat-paginator-intl.component';


describe('CustomMatPaginatorIntlComponent', () => {
  let component: CustomMatPaginatorIntl;
  let fixture: ComponentFixture<CustomMatPaginatorIntl>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [CustomMatPaginatorIntl]
    });
    fixture = TestBed.createComponent(CustomMatPaginatorIntl);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
