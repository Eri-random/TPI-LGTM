import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { FormBuilder, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { DialogDonateComponent } from './dialog-donate.component';
import { AuthService } from 'src/app/services/auth.service';
import { UserStoreService } from 'src/app/services/user-store.service';
import { HeadquartersService } from 'src/app/services/headquarters.service';
import { OrganizationService } from 'src/app/services/organization.service';
import { NgToastService } from 'ng-angular-popup';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { DonationsService } from 'src/app/services/donations.service';

describe('DialogDonateComponent', () => {
  let component: DialogDonateComponent;
  let fixture: ComponentFixture<DialogDonateComponent>;
  let mockDonationService: any;
  let mockAuthService: any;
  let mockUserStoreService: any;
  let mockHeadquartersService: any;
  let mockOrganizationService: any;
  let mockToastService: any;
  let mockRouter: any;
  let mockDialogRef: any;

  beforeEach(async () => {
    mockDonationService = jasmine.createSpyObj('DonationsService', ['postSaveDonation']);
    mockAuthService = jasmine.createSpyObj('AuthService', ['getEmailFromToken']);
    mockUserStoreService = jasmine.createSpyObj('UserStoreService', ['getEmailFromStore', 'getUserByEmail']);
    mockHeadquartersService = jasmine.createSpyObj('HeadquartersService', ['getHeadquartersByOrganization', 'postNearestHeadquarter', 'setDataDirection']);
    mockOrganizationService = jasmine.createSpyObj('OrganizationService', ['getOrganizationById']);
    mockToastService = jasmine.createSpyObj('NgToastService', ['success', 'error']);
    mockRouter = jasmine.createSpyObj('Router', ['navigate']);
    mockDialogRef = jasmine.createSpyObj('MatDialogRef', ['close']);

    await TestBed.configureTestingModule({
      declarations: [DialogDonateComponent],
      imports: [ReactiveFormsModule, FormsModule],
      providers: [
        FormBuilder,
        { provide: MAT_DIALOG_DATA, useValue: { organizacionId: 1 } },
        { provide: MatDialogRef, useValue: mockDialogRef },
        { provide: DonationsService, useValue: mockDonationService },
        { provide: AuthService, useValue: mockAuthService },
        { provide: UserStoreService, useValue: mockUserStoreService },
        { provide: HeadquartersService, useValue: mockHeadquartersService },
        { provide: OrganizationService, useValue: mockOrganizationService },
        { provide: NgToastService, useValue: mockToastService },
        { provide: Router, useValue: mockRouter },
      ],
      schemas: [NO_ERRORS_SCHEMA]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DialogDonateComponent);
    component = fixture.componentInstance;

    mockUserStoreService.getEmailFromStore.and.returnValue(of('test@ejemplo.com'));
    mockAuthService.getEmailFromToken.and.returnValue('test@ejemplo.com');
    mockUserStoreService.getUserByEmail.and.returnValue(of({ id: 1, email: 'test@ejemplo.com' }));

    fixture.detectChanges();
  });

  it('debería crear el componente', () => {
    expect(component).toBeTruthy();
  });

  it('debería inicializar el formulario y cargar los datos del usuario en ngOnInit', fakeAsync(() => {
    component.ngOnInit();

    expect(component.donateForm).toBeDefined();
    expect(component.email).toBe('test@ejemplo.com');
    expect(component.user).toEqual({ id: 1, email: 'test@ejemplo.com' });
  }));

  it('debería validar el formulario y no llamar al servicio de donación si el formulario es inválido', () => {
    component.donateForm.setValue({
      producto: '',
      cantidad: ''
    });

    component.donate();

    expect(mockDonationService.postSaveDonation).not.toHaveBeenCalled();
  });

  it('debería cerrar el diálogo al llamar a close', () => {
    component.close();

    expect(mockDialogRef.close).toHaveBeenCalled();
  });
});
