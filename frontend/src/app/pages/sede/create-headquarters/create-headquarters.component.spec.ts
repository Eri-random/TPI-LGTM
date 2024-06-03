import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { ReactiveFormsModule, FormBuilder } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { of, throwError } from 'rxjs';
import { NgToastService } from 'ng-angular-popup';
import { CreateHeadquartersComponent } from './create-headquarters.component';
import { AuthService } from 'src/app/services/auth.service';
import { OrganizationService } from 'src/app/services/organization.service';
import { HeadquartersService } from 'src/app/services/headquarters.service';
import { MapService } from 'src/app/services/map.service';
import { Provinces, Province } from 'src/app/interfaces/provinces.interface';

describe('CreateHeadquartersComponent', () => {
  let component: CreateHeadquartersComponent;
  let fixture: ComponentFixture<CreateHeadquartersComponent>;
  let mockOrganizationService: jasmine.SpyObj<OrganizationService>;
  let mockHeadquartersService: jasmine.SpyObj<HeadquartersService>;
  let mockMapService: jasmine.SpyObj<MapService>;
  let mockAuthService: jasmine.SpyObj<AuthService>;
  let mockToast: jasmine.SpyObj<NgToastService>;
  let fb: FormBuilder;

  beforeEach(async () => {
    const organizationServiceSpy = jasmine.createSpyObj('OrganizationService', ['getOrgNameFromStore', 'getCuitFromStore', 'getOrganizationByCuit']);
    const headquartersServiceSpy = jasmine.createSpyObj('HeadquartersService', ['postHeadquarters']);
    const mapServiceSpy = jasmine.createSpyObj('MapService', ['getProvinces']);
    const authServiceSpy = jasmine.createSpyObj('AuthService', ['getOrgNameFromToken', 'getCuitFromToken']);
    const toastSpy = jasmine.createSpyObj('NgToastService', ['success', 'error']);

    await TestBed.configureTestingModule({
      declarations: [CreateHeadquartersComponent],
      imports: [ReactiveFormsModule],
      providers: [
        FormBuilder,
        { provide: OrganizationService, useValue: organizationServiceSpy },
        { provide: HeadquartersService, useValue: headquartersServiceSpy },
        { provide: MapService, useValue: mapServiceSpy },
        { provide: AuthService, useValue: authServiceSpy },
        { provide: NgToastService, useValue: toastSpy },
        { provide: ActivatedRoute, useValue: { snapshot: { params: {} } } }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(CreateHeadquartersComponent);
    component = fixture.componentInstance;
    mockOrganizationService = TestBed.inject(OrganizationService) as jasmine.SpyObj<OrganizationService>;
    mockHeadquartersService = TestBed.inject(HeadquartersService) as jasmine.SpyObj<HeadquartersService>;
    mockMapService = TestBed.inject(MapService) as jasmine.SpyObj<MapService>;
    mockAuthService = TestBed.inject(AuthService) as jasmine.SpyObj<AuthService>;
    mockToast = TestBed.inject(NgToastService) as jasmine.SpyObj<NgToastService>;
    fb = TestBed.inject(FormBuilder);
  });

  it('debería crear el componente', () => {
    expect(component).toBeTruthy();
  });

  it('debería inicializar y cargar datos de la organización y las provincias', fakeAsync(() => {
    const mockOrgName = 'Org Prueba';
    const mockCuit = '123456789';
    const mockOrganization = { id: 1 };
    const mockProvinces: Provinces = {
      provincias: [
        { nombre: 'Buenos Aires' } as Province,
        { nombre: 'Neuquen' } as Province,
        { nombre: 'Chaco' } as Province
      ],
      cantidad: 3,
      inicio: 0,
      parametros: {},
      total: 3
    };

    mockOrganizationService.getOrgNameFromStore.and.returnValue(of(mockOrgName));
    mockAuthService.getOrgNameFromToken.and.returnValue(mockOrgName);
    mockOrganizationService.getCuitFromStore.and.returnValue(of(mockCuit));
    mockAuthService.getCuitFromToken.and.returnValue(mockCuit);
    mockOrganizationService.getOrganizationByCuit.and.returnValue(of(mockOrganization));
    mockMapService.getProvinces.and.returnValue(of(mockProvinces));

    component.ngOnInit();

    expect(mockOrganizationService.getOrgNameFromStore).toHaveBeenCalled();
    expect(mockAuthService.getOrgNameFromToken).toHaveBeenCalled();
    expect(mockOrganizationService.getCuitFromStore).toHaveBeenCalled();
    expect(mockAuthService.getCuitFromToken).toHaveBeenCalled();
    expect(mockOrganizationService.getOrganizationByCuit).toHaveBeenCalledWith(mockCuit);
    expect(mockMapService.getProvinces).toHaveBeenCalled();

    expect(component.orgName).toBe(mockOrgName);
    expect(component.cuit).toBe(mockCuit);
    expect(component.organization).toEqual(mockOrganization);
    expect(component.provinces.length).toBe(3); // Asegura que se carguen las provincias
  }));

  it('debería agregar una nueva sede', () => {
    component.addingOtherHeadquarters();
    expect(component.headquarters.length).toBe(2);
  });

  it('debería eliminar una sede existente', () => {
    component.addingOtherHeadquarters();
    component.deleteHeadquarters(1);
    expect(component.headquarters.length).toBe(1);
  });

  it('debería guardar las sedes correctamente', () => {
    const mockOrganization = { id: 1 };
    const mockHeadquartersFormValue = {
      sedes: [
        {
          nombre: 'Sede 1',
          direccion: 'Direccion 1',
          localidad: 'Localidad 1',
          provincia: 'Provincia 1',
          telefono: '123456789'
        }
      ]
    };
    const mockResponse = {};

    component.organization = mockOrganization;
    component.HeadquartersForm.setValue(mockHeadquartersFormValue);

    mockHeadquartersService.postHeadquarters.and.returnValue(of(mockResponse));

    component.saveHeadquarters();

    expect(mockHeadquartersService.postHeadquarters).toHaveBeenCalledWith([
      {
        ...mockHeadquartersFormValue.sedes[0],
        telefono: '123456789',
        organizacionId: mockOrganization.id
      }
    ]);
    expect(mockToast.success).toHaveBeenCalledWith({
      detail: 'EXITO',
      summary: 'Sede guardada con éxito',
      duration: 3000,
      position: 'topRight',
    });
  });

  it('debería mostrar un mensaje de error al fallar el guardado de las sedes', () => {
    const mockOrganization = { id: 1 };
    const mockHeadquartersFormValue = {
      sedes: [
        {
          nombre: 'Sede 1',
          direccion: 'Direccion 1',
          localidad: 'Localidad 1',
          provincia: 'Provincia 1',
          telefono: '123456789'
        }
      ]
    };
    const mockError = new Error('Error al guardar las sedes');

    component.organization = mockOrganization;
    component.HeadquartersForm.setValue(mockHeadquartersFormValue);

    mockHeadquartersService.postHeadquarters.and.returnValue(throwError(mockError));

    component.saveHeadquarters();

    expect(mockToast.error).toHaveBeenCalledWith({
      detail: 'ERROR',
      summary: 'Ocurrió un error al procesar la solicitud!',
      duration: 3000,
      position: 'topRight',
    });
  });
});
