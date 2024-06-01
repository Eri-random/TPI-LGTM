import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MapOrganizationsComponent } from './map-organizations.component';

describe('MapaOrganizacionesComponent', () => {
  let component: MapOrganizationsComponent;
  let fixture: ComponentFixture<MapOrganizationsComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [MapOrganizationsComponent]
    });
    fixture = TestBed.createComponent(MapOrganizationsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
