import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MapaOrganizacionesComponent } from './mapa-organizaciones.component';

describe('MapaOrganizacionesComponent', () => {
  let component: MapaOrganizacionesComponent;
  let fixture: ComponentFixture<MapaOrganizacionesComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [MapaOrganizacionesComponent]
    });
    fixture = TestBed.createComponent(MapaOrganizacionesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
