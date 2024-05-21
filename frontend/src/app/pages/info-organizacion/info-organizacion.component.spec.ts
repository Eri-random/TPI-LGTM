import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InfoOrganizacionComponent } from './info-organizacion.component';

describe('InfoOrganizacionComponent', () => {
  let component: InfoOrganizacionComponent;
  let fixture: ComponentFixture<InfoOrganizacionComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [InfoOrganizacionComponent]
    });
    fixture = TestBed.createComponent(InfoOrganizacionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
