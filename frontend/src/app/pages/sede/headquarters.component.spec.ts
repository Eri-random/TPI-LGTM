import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { of, throwError } from 'rxjs';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { NgToastService } from 'ng-angular-popup';
import { AuthService } from 'src/app/services/auth.service';
import { OrganizationService } from 'src/app/services/organization.service';
import { HeadquartersService } from 'src/app/services/headquarters.service';
import { SedeComponent } from './headquarters.component';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';

describe('SedeComponent', () => {
  let component: SedeComponent;
  let fixture: ComponentFixture<SedeComponent>;
  let mockDialog: jasmine.SpyObj<MatDialog>;
  let mockRouter: jasmine.SpyObj<Router>;
  let mockOrganizationService: jasmine.SpyObj<OrganizationService>;
  let mockHeadquartersService: jasmine.SpyObj<HeadquartersService>;
  let mockToast: jasmine.SpyObj<NgToastService>;
  let mockAuthService: jasmine.SpyObj<AuthService>;
  let mockDialogRef: jasmine.SpyObj<MatDialogRef<any>>;

  beforeEach(async () => {
    const dialogSpy = jasmine.createSpyObj('MatDialog', ['open', 'closeAll']);
    const routerSpy = jasmine.createSpyObj('Router', ['navigate']);
    const organizationServiceSpy = jasmine.createSpyObj('OrganizationService', ['getOrgNameFromStore', 'getCuitFromStore', 'getOrganizationByCuit']);
    const headquartersServiceSpy = jasmine.createSpyObj('HeadquartersService', ['getHeadquartersByOrganization', 'deleteHeadquarters']);
    const toastSpy = jasmine.createSpyObj('NgToastService', ['success', 'error']);
    const authServiceSpy = jasmine.createSpyObj('AuthService', ['getOrgNameFromToken', 'getCuitFromToken']);
    const dialogRefSpy = jasmine.createSpyObj('MatDialogRef', ['afterClosed']);

    await TestBed.configureTestingModule({
      declarations: [ SedeComponent ],
      providers: [
        { provide: MatDialog, useValue: dialogSpy },
        { provide: Router, useValue: routerSpy },
        { provide: OrganizationService, useValue: organizationServiceSpy },
        { provide: HeadquartersService, useValue: headquartersServiceSpy },
        { provide: NgToastService, useValue: toastSpy },
        { provide: AuthService, useValue: authServiceSpy }
      ],
      schemas: [CUSTOM_ELEMENTS_SCHEMA]
    }).compileComponents();

    fixture = TestBed.createComponent(SedeComponent);
    component = fixture.componentInstance;
    mockDialog = TestBed.inject(MatDialog) as jasmine.SpyObj<MatDialog>;
    mockRouter = TestBed.inject(Router) as jasmine.SpyObj<Router>;
    mockOrganizationService = TestBed.inject(OrganizationService) as jasmine.SpyObj<OrganizationService>;
    mockHeadquartersService = TestBed.inject(HeadquartersService) as jasmine.SpyObj<HeadquartersService>;
    mockToast = TestBed.inject(NgToastService) as jasmine.SpyObj<NgToastService>;
    mockAuthService = TestBed.inject(AuthService) as jasmine.SpyObj<AuthService>;
    mockDialogRef = dialogRefSpy;
  });

  it('debería crear el componente', () => {
    expect(component).toBeTruthy();
  });

  it('debería inicializar y cargar datos de la organización y las sedes', fakeAsync(() => {
    const mockOrgName = 'Org Prueba';
    const mockCuit = '123456789';
    const mockOrganization = { id: 1 };
    const mockHeadquarters = [{ id: 1, nombre: 'Sede 1' }, { id: 2, nombre: 'Sede 2' }];

    mockOrganizationService.getOrgNameFromStore.and.returnValue(of(mockOrgName));
    mockAuthService.getOrgNameFromToken.and.returnValue(mockOrgName);
    mockOrganizationService.getCuitFromStore.and.returnValue(of(mockCuit));
    mockAuthService.getCuitFromToken.and.returnValue(mockCuit);
    mockOrganizationService.getOrganizationByCuit.and.returnValue(of(mockOrganization));
    mockHeadquartersService.getHeadquartersByOrganization.and.returnValue(of(mockHeadquarters));

    component.ngOnInit();

    tick(1000); // Simula el paso del tiempo para la función setTimeout

    expect(mockOrganizationService.getOrgNameFromStore).toHaveBeenCalled();
    expect(mockAuthService.getOrgNameFromToken).toHaveBeenCalled();
    expect(mockOrganizationService.getCuitFromStore).toHaveBeenCalled();
    expect(mockAuthService.getCuitFromToken).toHaveBeenCalled();
    expect(mockOrganizationService.getOrganizationByCuit).toHaveBeenCalledWith(mockCuit);
    expect(mockHeadquartersService.getHeadquartersByOrganization).toHaveBeenCalledWith(mockOrganization.id);

    expect(component.orgName).toBe(mockOrgName);
    expect(component.cuit).toBe(mockCuit);
    expect(component.organization).toEqual(mockOrganization);
    expect(component.headquarters).toEqual(mockHeadquarters);
    expect(component.loading).toBeFalse(); // Asegura que loading sea false después de cargar los datos
  }));

  it('debería ir a crear una nueva sede', () => {
    component.addingHeadquarters();
    expect(mockRouter.navigate).toHaveBeenCalledWith(['/crear-sede']);
  });

  it('debería ir a editar una sede existente', () => {
    const mockHeadquarter = { id: 1, nombre: 'Sede 1' };
    component.editHeadquarters(mockHeadquarter);
    expect(mockRouter.navigate).toHaveBeenCalledWith(['/editar-sede', mockHeadquarter.id]);
  });

  it('debería abrir el diálogo de confirmación', () => {
    const mockHeadquarter = { id: 1, nombre: 'Sede 1' };
    mockDialog.open.and.returnValue(mockDialogRef);
    mockDialogRef.afterClosed.and.returnValue(of(true));

    component.openConfirmDialog(mockHeadquarter);
    
    expect(component.selectedSede).toBe(mockHeadquarter);
    expect(mockDialog.open).toHaveBeenCalledWith(component.confirmDialog, {
      width: '250px',
      data: { nombre: mockHeadquarter.nombre }
    });
    expect(mockDialogRef.afterClosed).toHaveBeenCalled();
  });

  it('debería eliminar una sede y mostrar un mensaje de éxito', () => {
    const mockHeadquarterId = 1;
    const mockHeadquarters = [{ id: 1, nombre: 'Sede 1' }, { id: 2, nombre: 'Sede 2' }];
    component.headquarters = mockHeadquarters;

    mockHeadquartersService.deleteHeadquarters.and.returnValue(of({}));

    component.deleteHeadquarters(mockHeadquarterId);

    expect(mockHeadquartersService.deleteHeadquarters).toHaveBeenCalledWith(mockHeadquarterId);
    expect(component.headquarters.length).toBe(1);
    expect(mockToast.success).toHaveBeenCalledWith({
      detail: 'EXITO',
      summary: 'Sede eliminada correctamente',
      duration: 3000,
      position: 'topRight',
    });
  });

  it('debería mostrar un mensaje de error al fallar la eliminación de una sede', () => {
    const mockHeadquarterId = 1;
    const mockError = new Error('Error al eliminar la sede');

    mockHeadquartersService.deleteHeadquarters.and.returnValue(throwError(mockError));
    spyOn(console, 'error');

    component.deleteHeadquarters(mockHeadquarterId);

    expect(mockHeadquartersService.deleteHeadquarters).toHaveBeenCalledWith(mockHeadquarterId);
    expect(mockToast.error).toHaveBeenCalledWith({
      detail: 'ERROR',
      summary: 'Error al eliminar la sede',
      duration: 3000,
      position: 'topRight',
    });
  });

  it('debería cerrar todos los diálogos al cancelar', () => {
    component.onCancel();
    expect(mockDialog.closeAll).toHaveBeenCalled();
  });

  it('debería cerrar todos los diálogos y eliminar la sede al confirmar', () => {
    const mockHeadquarter = { id: 1, nombre: 'Sede 1' };
    component.selectedSede = mockHeadquarter;

    spyOn(component, 'deleteHeadquarters');

    component.onConfirm();

    expect(mockDialog.closeAll).toHaveBeenCalled();
    expect(component.deleteHeadquarters).toHaveBeenCalledWith(mockHeadquarter.id);
  });
});
