import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PedidoDeOrganizacionComponent } from './pedido-de-organizacion.component';

describe('PedidoDeOrganizacionComponent', () => {
  let component: PedidoDeOrganizacionComponent;
  let fixture: ComponentFixture<PedidoDeOrganizacionComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [PedidoDeOrganizacionComponent]
    });
    fixture = TestBed.createComponent(PedidoDeOrganizacionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
