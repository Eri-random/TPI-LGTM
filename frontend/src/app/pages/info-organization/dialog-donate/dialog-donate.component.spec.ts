import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { of, throwError } from 'rxjs';
import { DialogDonateComponent } from './dialog-donate.component';
import { AuthService } from 'src/app/services/auth.service';
import { OrganizationService } from 'src/app/services/organization.service';
import { HeadquartersService } from 'src/app/services/headquarters.service';
import { UserStoreService } from 'src/app/services/user-store.service';
import { DonationsService } from 'src/app/services/donations.service';

describe('DialogDonateComponent', () => {
  let component: DialogDonateComponent;
  let fixture: ComponentFixture<DialogDonateComponent>;

  // Mock servicios
  let mockAuthService: jasmine.SpyObj<AuthService>;
  let mockOrganizationService: jasmine.SpyObj<OrganizationService>;
  let mockHeadquartersService: jasmine.SpyObj<HeadquartersService>;
  let mockUserStoreService: jasmine.SpyObj<UserStoreService>;
  let mockDonationsService: jasmine.SpyObj<DonationsService>;

  beforeEach(async () => {
    mockAuthService = jasmine.createSpyObj('AuthService', ['getEmailFromToken']);
    mockOrganizationService = jasmine.createSpyObj('OrganizationService', ['getOrganizationById']);
    mockHeadquartersService = jasmine.createSpyObj('HeadquartersService', ['getHeadquartersByOrganization', 'postNearestHeadquarter', 'setDataDirection']);
    mockUserStoreService = jasmine.createSpyObj('UserStoreService', ['getEmailFromStore', 'getUserByEmail']);
    mockDonationsService = jasmine.createSpyObj('DonationsService', ['postSaveDonation']);

    await TestBed.configureTestingModule({
      declarations: [DialogDonateComponent],
      imports: [ReactiveFormsModule],
      providers: [
        FormBuilder,
        { provide: MAT_DIALOG_DATA, useValue: { organizacionId: 1 } },
        { provide: MatDialogRef, useValue: { close: jasmine.createSpy('close') } },
        { provide: AuthService, useValue: mockAuthService },
        { provide: OrganizationService, useValue: mockOrganizationService },
        { provide: HeadquartersService, useValue: mockHeadquartersService },
        { provide: UserStoreService, useValue: mockUserStoreService },
        { provide: DonationsService, useValue: mockDonationsService },
      ]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DialogDonateComponent);
    component = fixture.componentInstance;

    // Configurar comportamientos predeterminados de los mocks
    mockAuthService.getEmailFromToken.and.returnValue('test@example.com');
    mockUserStoreService.getEmailFromStore.and.returnValue(of('test@example.com'));
    mockUserStoreService.getUserByEmail.and.returnValue(of({ id: 1 }));
    mockOrganizationService.getOrganizationById.and.returnValue(of({ id: 1, cuit: '123456789' }));
    mockHeadquartersService.getHeadquartersByOrganization.and.returnValue(of([{ id: 1, location: 'Location 1' }]));
    mockHeadquartersService.postNearestHeadquarter.and.returnValue(of({ direction: 'Nearest Location' }));
    mockDonationsService.postSaveDonation.and.returnValue(of({}));

    fixture.detectChanges();
  });

  it('debe crear el componente', () => {
    expect(component).toBeTruthy();
  });

  it('debe inicializar el formulario', () => {
    expect(component.donateForm).toBeTruthy();
    expect(component.donateForm.controls['producto'].valid).toBeFalse();
    expect(component.donateForm.controls['cantidad'].valid).toBeFalse();
  });

  it('debe obtener el email desde el store y el token al inicializar', () => {
    expect(mockAuthService.getEmailFromToken).toHaveBeenCalled();
    expect(mockUserStoreService.getEmailFromStore).toHaveBeenCalled();
    expect(component.email).toBe('test@example.com');
  });

  it('debe obtener el usuario por email al inicializar', () => {
    expect(mockUserStoreService.getUserByEmail).toHaveBeenCalledWith('test@example.com');
    expect(component.user).toEqual({ id: 1 });
  });

  it('debe obtener la organización por id al inicializar', () => {
    expect(mockOrganizationService.getOrganizationById).toHaveBeenCalledWith(1);
    expect(component.cuit).toBe('123456789');
  });

  it('debe validar los campos del formulario en envío inválido', () => {
    component.donateForm.controls['producto'].setValue('');
    component.donateForm.controls['cantidad'].setValue('');
    component.donate();
    expect(component.donateForm.invalid).toBeTrue();
  });

  it('debe llamar a postSaveDonation en envío de formulario válido', () => {
    component.donateForm.controls['producto'].setValue('Producto 1');
    component.donateForm.controls['cantidad'].setValue('10');
    component.donate();

    expect(mockDonationsService.postSaveDonation).toHaveBeenCalledWith({
      producto: 'Producto 1',
      cantidad: '10',
      usuarioId: 1,
      organizacionId: 1,
      cuit: '123456789'
    });
  });

  it('debe manejar errores en postSaveDonation', () => {
    mockDonationsService.postSaveDonation.and.returnValue(throwError('Error'));
    spyOn(console, 'error');

    component.donateForm.controls['producto'].setValue('Producto 1');
    component.donateForm.controls['cantidad'].setValue('10');
    component.donate();

    expect(console.error).toHaveBeenCalledWith('Error:', 'Error');
  });

  it('debe obtener la sede más cercana y establecer dataDirection en envío de formulario válido', () => {
    component.donateForm.controls['producto'].setValue('Producto 1');
    component.donateForm.controls['cantidad'].setValue('10');
    component.donate();

    expect(mockHeadquartersService.getHeadquartersByOrganization).toHaveBeenCalledWith(1);
    expect(mockHeadquartersService.postNearestHeadquarter).toHaveBeenCalledWith({
      organizacion: { id: 1, cuit: '123456789' },
      sedes: [{ id: 1, location: 'Location 1' }],
      usuario: { id: 1 }
    });
    expect(mockHeadquartersService.setDataDirection).toHaveBeenCalledWith({ direction: 'Nearest Location' });
  });

  it('debe cerrar el diálogo', () => {
    component.close();
    expect(component.dialogRef.close).toHaveBeenCalled();
  });
});
