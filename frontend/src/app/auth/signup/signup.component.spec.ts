import { ComponentFixture, TestBed } from '@angular/core/testing';
import { SignupComponent } from './signup.component';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { RouterTestingModule } from '@angular/router/testing';
import { of, throwError } from 'rxjs';
import { AuthService } from 'src/app/services/auth.service';
import { MapService } from 'src/app/services/map.service';
import { NgToastService } from 'ng-angular-popup';
import { Router } from '@angular/router';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { Province } from 'src/app/interfaces/provinces.interface';
import { User } from 'src/app/models/user';

describe('SignupComponent', () => {
  let component: SignupComponent;
  let fixture: ComponentFixture<SignupComponent>;
  let authService: jasmine.SpyObj<AuthService>;
  let mapService: jasmine.SpyObj<MapService>;
  let toastService: jasmine.SpyObj<NgToastService>;
  let router: jasmine.SpyObj<Router>;

  beforeEach(async () => {
    const authServiceSpy = jasmine.createSpyObj('AuthService', ['createAccount']);
    const mapServiceSpy = jasmine.createSpyObj('MapService', ['getProvinces', 'getLocalities', 'getLocalitiesFilter']);
    const toastServiceSpy = jasmine.createSpyObj('NgToastService', ['success', 'error']);
    const routerSpy = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      declarations: [SignupComponent],
      imports: [
        ReactiveFormsModule,
        FormsModule,
        RouterTestingModule,
        HttpClientTestingModule,
      ],
      providers: [
        { provide: AuthService, useValue: authServiceSpy },
        { provide: MapService, useValue: mapServiceSpy },
        { provide: NgToastService, useValue: toastServiceSpy },
        { provide: Router, useValue: routerSpy },
      ],
      schemas: [NO_ERRORS_SCHEMA]
    }).compileComponents();

    fixture = TestBed.createComponent(SignupComponent);
    component = fixture.componentInstance;
    authService = TestBed.inject(AuthService) as jasmine.SpyObj<AuthService>;
    mapService = TestBed.inject(MapService) as jasmine.SpyObj<MapService>;
    toastService = TestBed.inject(NgToastService) as jasmine.SpyObj<NgToastService>;
    router = TestBed.inject(Router) as jasmine.SpyObj<Router>;

    mapService.getProvinces.and.returnValue(of({
      cantidad: 3,
      inicio: 0,
      parametros: {},
      provincias: [
        { nombre: 'Buenos Aires' } as Province,
      ],
      total: 3
    }));
    mapService.getLocalities.and.returnValue(of({ total: '1', localidades: [{ id: 1, nombre: 'La Plata' }] }));
    mapService.getLocalitiesFilter.and.returnValue(of({ localidades: [{ id: 1, nombre: 'La Plata' }] }));

    fixture.detectChanges(); // initial binding
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize the form with empty values', () => {
    expect(component.registerForm).toBeDefined();
    expect(component.registerFormOrganizaction).toBeDefined();
    // Enable localidad control temporarily to include it in the form value
    component.registerForm.get('localidad')?.enable();
    component.registerFormOrganizaction.get('localidad')?.enable();

    expect(component.registerForm.value).toEqual({
      nombre: '',
      apellido: null,
      telefono: null,
      direccion: '',
      localidad: '', // Expect the correct structure here
      provincia: '',
      email: '',
      password: '',
    });
    expect(component.registerFormOrganizaction.value).toEqual({
      nombre: '',
      apellido: null,
      email: '',
      password: '',
      telefono: null,
      direccion: '',
      localidad: '', // Expect the correct structure here
      provincia: '',
      nombreOrg: '',
      cuit: '',
    });

    // Disable localidad control again
    component.registerForm.get('localidad')?.disable();
    component.registerFormOrganizaction.get('localidad')?.disable();
  });


  it('should toggle password visibility', () => {
    component.hideShowPass();
    expect(component.isText).toBeTrue();
    expect(component.type).toBe('text');
    expect(component.eyeIcon).toBe('fa-eye');

    component.hideShowPass();
    expect(component.isText).toBeFalse();
    expect(component.type).toBe('password');
    expect(component.eyeIcon).toBe('fa-eye-slash');
  });

  it('should validate required fields in user form', () => {
    component.selectedRole = 'usuario';
    component.onSubmit();
    expect(component.registerForm.invalid).toBeTrue();
  });

  it('should validate required fields in organization form', () => {
    component.selectedRole = 'organización';
    component.onSubmit();
    expect(component.registerFormOrganizaction.invalid).toBeTrue();
  });

  it('should submit valid user form', () => {
    component.selectedRole = 'usuario';
    component.registerForm.setValue({
      nombre: 'John',
      apellido: 'Doe',
      telefono: '12345678',
      direccion: '123 Main St',
      localidad: 'La Plata',
      provincia: 'Buenos Aires',
      email: 'john.doe@example.com',
      password: 'password',
    });

    const userResponse: User = {
      id: 1,
      nombre: 'John',
      apellido: 'Doe',
      telefono: 12345678,
      direccion: '123 Main St',
      localidad: 'La Plata',
      provincia: 'Buenos Aires',
      email: 'john.doe@example.com',
      password: 'password',
      rolId: 2,
    };

    authService.createAccount.and.returnValue(of(userResponse));
    component.onSubmit();
    expect(authService.createAccount).toHaveBeenCalled();
    expect(toastService.success).toHaveBeenCalled();
    expect(router.navigate).toHaveBeenCalledWith(['/login']);
  });

  it('should handle user form submission error', () => {
    component.selectedRole = 'usuario';
    component.registerForm.setValue({
      nombre: 'John',
      apellido: 'Doe',
      telefono: '12345678',
      direccion: '123 Main St',
      localidad: 'La Plata',
      provincia: 'Buenos Aires',
      email: 'john.doe@example.com',
      password: 'password',
    });

    authService.createAccount.and.returnValue(throwError({ error: 'Error message' }));
    component.onSubmit();
    expect(authService.createAccount).toHaveBeenCalled();
    expect(toastService.error).toHaveBeenCalledWith(jasmine.objectContaining({
      detail: 'ERROR',
      summary: 'Error message',
      duration: 5000,
      position: 'bottomRight',
    }));
  });

  it('should submit valid organization form', () => {
    component.selectedRole = 'organización';
    component.registerFormOrganizaction.setValue({
      nombre: 'Org Name',
      apellido: 'Doe',
      email: 'org@example.com',
      password: 'password',
      telefono: '12345678',
      direccion: '123 Org St',
      localidad: 'La Plata',
      provincia: 'Buenos Aires',
      nombreOrg: 'Organization',
      cuit: '12345678901',
    });

    const orgResponse: User = {
      id: 1,
      nombre: 'Org Name',
      apellido: 'Doe',
      telefono: 12345678,
      direccion: '123 Org St',
      localidad: 'La Plata',
      provincia: 'Buenos Aires',
      email: 'org@example.com',
      password: 'password',
      rolId: 3,
    };

    authService.createAccount.and.returnValue(of(orgResponse));
    component.onSubmit();
    expect(authService.createAccount).toHaveBeenCalled();
    expect(toastService.success).toHaveBeenCalled();
    expect(router.navigate).toHaveBeenCalledWith(['/login']);
  });

  it('should handle organization form submission error', () => {
    component.selectedRole = 'organización';
    component.registerFormOrganizaction.setValue({
      nombre: 'Org Name',
      apellido: 'Doe',
      email: 'org@example.com',
      password: 'password',
      telefono: '12345678',
      direccion: '123 Org St',
      localidad: 'La Plata',
      provincia: 'Buenos Aires',
      nombreOrg: 'Organization',
      cuit: '12345678901',
    });

    authService.createAccount.and.returnValue(throwError({ error: 'Error message' }));
    component.onSubmit();
    expect(authService.createAccount).toHaveBeenCalled();
    expect(toastService.error).toHaveBeenCalledWith(jasmine.objectContaining({
      detail: 'ERROR',
      summary: 'Error message',
      duration: 5000,
      position: 'bottomRight',
    }));
  });

  it('should load provinces on init', () => {
    expect(mapService.getProvinces).toHaveBeenCalled();
    expect(component.provinces).toEqual([{ nombre: 'Buenos Aires' } as Province]);
  });

  it('should load localidades on province change for user', () => {
    component.selectedRole = 'usuario';
    component.registerForm.get('provincia')?.setValue(1);
    component.onProvinceChange();

    expect(mapService.getLocalities).toHaveBeenCalledWith(1);
    expect(mapService.getLocalitiesFilter).toHaveBeenCalledWith(1, '1');
    expect(component.localidades).toEqual([{ id: 1, nombre: 'La Plata' }]);
  });

  it('should load localidades on province change for organization', () => {
    component.selectedRole = 'organización';
    component.registerFormOrganizaction.get('provincia')?.setValue(1);
    component.onProvinceChange();

    expect(mapService.getLocalities).toHaveBeenCalledWith(1);
    expect(mapService.getLocalitiesFilter).toHaveBeenCalledWith(1, '1');
    expect(component.localidades).toEqual([{ id: 1, nombre: 'La Plata' }]);
  });
});
