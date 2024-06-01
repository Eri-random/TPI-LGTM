import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InfoOrganizationComponent } from './info-organization.component';

describe('InfoOrganizacionComponent', () => {
  let component: InfoOrganizationComponent;
  let fixture: ComponentFixture<InfoOrganizationComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [InfoOrganizationComponent]
    });
    fixture = TestBed.createComponent(InfoOrganizationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
