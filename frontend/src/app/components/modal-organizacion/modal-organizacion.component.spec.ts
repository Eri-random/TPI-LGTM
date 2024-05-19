import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ModalOrganizacionComponent } from './modal-organizacion.component';

describe('ModalOrganizacionComponent', () => {
  let component: ModalOrganizacionComponent;
  let fixture: ComponentFixture<ModalOrganizacionComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [ModalOrganizacionComponent]
    });
    fixture = TestBed.createComponent(ModalOrganizacionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
