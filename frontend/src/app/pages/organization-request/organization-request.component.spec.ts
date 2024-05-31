import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OrganizationRequestComponent } from './organization-request.component';

describe('PedidoDeOrganizacionComponent', () => {
  let component: OrganizationRequestComponent;
  let fixture: ComponentFixture<OrganizationRequestComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [OrganizationRequestComponent]
    });
    fixture = TestBed.createComponent(OrganizationRequestComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
