import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { of } from 'rxjs';
import { DonationsComponent } from './donations.component';
import { OrganizationService } from 'src/app/services/organization.service';

describe('DonationsComponent', () => {
  let component: DonationsComponent;
  let fixture: ComponentFixture<DonationsComponent>;
  let organizationServiceMock: any;
  let routerMock: any;

  beforeEach(async () => {
    organizationServiceMock = {
      getPaginatedOrganizations: jasmine.createSpy('getPaginatedOrganizations').and.returnValue(of([]))
    };

    routerMock = {
      navigate: jasmine.createSpy('navigate')
    };

    await TestBed.configureTestingModule({
      declarations: [ DonationsComponent ],
      providers: [
        { provide: OrganizationService, useValue: organizationServiceMock },
        { provide: Router, useValue: routerMock }
      ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DonationsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('debería crear el componente', () => {
    expect(component).toBeTruthy();
  });

  it('debería inicializarse con los valores predeterminados', () => {
    expect(component.organizations).toEqual([]);
    expect(component.page).toBe(1);
    expect(component.pageSize).toBe(8);
    expect(component.showSeeMore).toBe(false);
  });

  it('debería llamar a uploadOrganizations en init', () => {
    spyOn(component, 'uploadOrganizations');
    component.ngOnInit();
    expect(component.uploadOrganizations).toHaveBeenCalled();
  });
  
  it('debería ocultar el botón "Ver más" si se cargan menos organizaciones', () => {
    // Simula que se devuelven menos de pageSize organizaciones
    const fewerOrganizationsMock = [
      { id: 1, name: 'Org 1' },
      { id: 2, name: 'Org 2' }
    ];
    
    organizationServiceMock.getPaginatedOrganizations.and.returnValue(of(fewerOrganizationsMock));
    
    component.organizations = []; // Nos aseguramos de que esté vacío antes de cargar
    component.uploadOrganizations();
    
    expect(component.organizations).toEqual(fewerOrganizationsMock);
    expect(component.showSeeMore).toBe(false); // Se espera que sea false cuando hay menos de pageSize organizaciones
  });
  

  it('debería cargar más organizaciones cuando se llama loadMore', () => {
    spyOn(component, 'uploadOrganizations');
    component.loadMore();
    expect(component.page).toBe(2);
    expect(component.uploadOrganizations).toHaveBeenCalled();
  });

  it('debería ir a los detalles de la organización cuando se llama a seeDetail', () => {
    const organizationMock = { id: 1 };
    component.seeDetail(organizationMock);
    expect(routerMock.navigate).toHaveBeenCalledWith(['/info-organizacion', 1]);
  });
});
