import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MyOrganizationComponent } from './my-organization.component';

describe('MiOrganizacionComponent', () => {
  let component: MyOrganizationComponent;
  let fixture: ComponentFixture<MyOrganizationComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [MyOrganizationComponent]
    });
    fixture = TestBed.createComponent(MyOrganizationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
