import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { ReactiveFormsModule, FormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { NgToastService } from 'ng-angular-popup';
import { EditHeadquartersComponent } from './edit-headquarters.component';
import { AuthService } from 'src/app/services/auth.service';
import { OrganizationService } from 'src/app/services/organization.service';
import { HeadquartersService } from 'src/app/services/headquarters.service';
import { MapService } from 'src/app/services/map.service';
import { Province, Provinces } from 'src/app/interfaces/provinces.interface';

describe('EditHeadquartersComponent', () => {
  let component: EditHeadquartersComponent;
  let fixture: ComponentFixture<EditHeadquartersComponent>;
  let mockOrganizationService: jasmine.SpyObj<OrganizationService>;
  let mockHeadquartersService: jasmine.SpyObj<HeadquartersService>;
  let mockMapService: jasmine.SpyObj<MapService>;
  let mockAuthService: jasmine.SpyObj<AuthService>;
  let mockToast: jasmine.SpyObj<NgToastService>;
  let mockRouter: jasmine.SpyObj<Router>;
  let mockActivatedRoute: any;
  let fb: FormBuilder;

  beforeEach(async () => {
    const organizationServiceSpy = jasmine.createSpyObj('OrganizationService', ['getOrgNameFromStore', 'getCuitFromStore', 'getOrganizationByCuit']);
    const headquartersServiceSpy = jasmine.createSpyObj('HeadquartersService', ['getHeadquarterById', 'updateHeadquarters']);
    const mapServiceSpy = jasmine.createSpyObj('MapService', ['getProvinces']);
    const authServiceSpy = jasmine.createSpyObj('AuthService', ['getOrgNameFromToken', 'getCuitFromToken']);
    const toastSpy = jasmine.createSpyObj('NgToastService', ['success', 'error']);
    const routerSpy = jasmine.createSpyObj('Router', ['navigate']);
    
    mockActivatedRoute = {
      snapshot: { params: { id: 1 } }
    };

    await TestBed.configureTestingModule({
      declarations: [ EditHeadquartersComponent ],
      imports: [ ReactiveFormsModule ],
      providers: [
        FormBuilder,
        { provide: OrganizationService, useValue: organizationServiceSpy },
        { provide: HeadquartersService, useValue: headquartersServiceSpy },
        { provide: MapService, useValue: mapServiceSpy },
        { provide: AuthService, useValue: authServiceSpy },
        { provide: NgToastService, useValue: toastSpy },
        { provide: Router, useValue: routerSpy },
        { provide: ActivatedRoute, useValue: mockActivatedRoute }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(EditHeadquartersComponent);
    component = fixture.componentInstance;
    mockOrganizationService = TestBed.inject(OrganizationService) as jasmine.SpyObj<OrganizationService>;
    mockHeadquartersService = TestBed.inject(HeadquartersService) as jasmine.SpyObj<HeadquartersService>;
    mockMapService = TestBed.inject(MapService) as jasmine.SpyObj<MapService>;
    mockAuthService = TestBed.inject(AuthService) as jasmine.SpyObj<AuthService>;
    mockToast = TestBed.inject(NgToastService) as jasmine.SpyObj<NgToastService>;
    mockRouter = TestBed.inject(Router) as jasmine.SpyObj<Router>;
    fb = TestBed.inject(FormBuilder);
  });

  it('debería crear el componente', () => {
    expect(component).toBeTruthy();
  });

  it('debería inicializar y cargar datos de la organización, sede y provincias', fakeAsync(() => {
    const mockOrgName = 'Org Prueba';
    const mockCuit = '123456789';
    const mockOrganization = { id: 1 };
    const mockHeadquarter = {
      nombre: 'Sede 1',
      direccion: 'Direccion 1',
      localidad: 'Localidad 1',
      provincia: 'Provincia 1',
      telefono: '123456789'
    };
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
    mockHeadquartersService.getHeadquarterById.and.returnValue(of(mockHeadquarter));
    mockMapService.getProvinces.and.returnValue(of(mockProvinces));

    component.ngOnInit();

    expect(mockOrganizationService.getOrgNameFromStore).toHaveBeenCalled();
    expect(mockAuthService.getOrgNameFromToken).toHaveBeenCalled();
    expect(mockOrganizationService.getCuitFromStore).toHaveBeenCalled();
    expect(mockAuthService.getCuitFromToken).toHaveBeenCalled();
    expect(mockOrganizationService.getOrganizationByCuit).toHaveBeenCalledWith(mockCuit);
    expect(mockHeadquartersService.getHeadquarterById).toHaveBeenCalledWith(1);
    expect(mockMapService.getProvinces).toHaveBeenCalled();

    expect(component.orgName).toBe(mockOrgName);
    expect(component.cuit).toBe(mockCuit);
    expect(component.organization).toEqual(mockOrganization);
    expect(component.headquartersForm.value).toEqual(mockHeadquarter);
    expect(component.provinces.length).toBe(3); // Asegura que se carguen las provincias
  }));

  it('debería actualizar la sede correctamente', () => {
    const mockOrganization = { id: 1 };
    const mockHeadquartersFormValue = {
      nombre: 'Sede 1',
      direccion: 'Direccion 1',
      localidad: 'Localidad 1',
      provincia: 'Provincia 1',
      telefono: '123456789'
    };
    const mockResponse = {};

    component.organization = mockOrganization;
    component.headquartersForm.setValue(mockHeadquartersFormValue);
    component.headquartersId = 1;

    mockHeadquartersService.updateHeadquarters.and.returnValue(of(mockResponse));

    component.submitForm();

    expect(mockHeadquartersService.updateHeadquarters).toHaveBeenCalledWith({
      ...mockHeadquartersFormValue,
      id: 1,
      organizacionId: mockOrganization.id
    });
    expect(mockToast.success).toHaveBeenCalledWith({
      detail: 'EXITO',
      summary: 'Sede 1 actualizada correctamente',
      duration: 3000,
      position: 'topRight',
    });
  });

  it('debería mostrar un mensaje de error al fallar la actualización de la sede', () => {
    const mockOrganization = { id: 1 };
    const mockHeadquartersFormValue = {
      nombre: 'Sede 1',
      direccion: 'Direccion 1',
      localidad: 'Localidad 1',
      provincia: 'Provincia 1',
      telefono: '123456789'
    };
    const mockError = new Error('Error al actualizar la sede');

    component.organization = mockOrganization;
    component.headquartersForm.setValue(mockHeadquartersFormValue);
    component.headquartersId = 1;  // Asegurarse de que headquartersId esté configurado correctamente

    mockHeadquartersService.updateHeadquarters.and.returnValue(throwError(mockError));

    component.submitForm();

    expect(mockToast.error).toHaveBeenCalledWith({
      detail: 'ERROR',
      summary: 'No se pudo actualizar la sede',
      duration: 3000,
      position: 'topRight',
    });
  });
});
