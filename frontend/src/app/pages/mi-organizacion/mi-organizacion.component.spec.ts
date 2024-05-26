import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MiOrganizacionComponent } from './mi-organizacion.component';

describe('MiOrganizacionComponent', () => {
  let component: MiOrganizacionComponent;
  let fixture: ComponentFixture<MiOrganizacionComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [MiOrganizacionComponent]
    });
    fixture = TestBed.createComponent(MiOrganizacionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
