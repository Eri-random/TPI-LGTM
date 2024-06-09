import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule, AbstractControl, ValidationErrors } from '@angular/forms';
import { SignupComponent } from './signup.component';
import { AuthService } from 'src/app/services/auth.service';
import { MapService } from 'src/app/services/map.service';
import { Router } from '@angular/router';
import { NgToastService } from 'ng-angular-popup';
import { of } from 'rxjs';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { CUSTOM_ELEMENTS_SCHEMA, NO_ERRORS_SCHEMA } from '@angular/core';
import { Province, Provinces } from 'src/app/interfaces/provinces.interface';

describe('SignupComponent', () => {
  let component: SignupComponent;
  let fixture: ComponentFixture<SignupComponent>;
  let mapServiceMock: any;
  let routerMock: any;
  let toastServiceMock: any;

  const mockProvinces: Provinces = {
    provincias: [
      { nombre: 'Buenos Aires' } as Province,
      { nombre: 'Cordoba' } as unknown as Province,
      { nombre: 'Ciudad Autónoma de Buenos Aires' } as Province,
      { nombre: 'Tierra del Fuego, Antártida e Islas del Atlántico Sur' } as Province
    ],
    cantidad: 3,
    inicio: 0,
    parametros: {},
    total: 3
  }; 

  beforeEach(async () => {
    mapServiceMock = jasmine.createSpyObj('MapService', ['getProvinces']);
    routerMock = jasmine.createSpyObj('Router', ['navigate']);
    toastServiceMock = jasmine.createSpyObj('NgToastService', ['success', 'error']);

    await TestBed.configureTestingModule({
      declarations: [SignupComponent],
      imports: [HttpClientTestingModule, ReactiveFormsModule],
      providers: [
        { provide: MapService, useValue: mapServiceMock },
        { provide: Router, useValue: routerMock },
        { provide: NgToastService, useValue: toastServiceMock },
        AuthService
      ],
      schemas: [CUSTOM_ELEMENTS_SCHEMA, NO_ERRORS_SCHEMA]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SignupComponent);
    component = fixture.componentInstance;
    mapServiceMock.getProvinces.and.returnValue(of(mockProvinces));
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize the form in ngOnInit', () => {
    mapServiceMock.getProvinces.and.returnValue(of(mockProvinces));
    component.ngOnInit();
    expect(component.registerForm).toBeDefined();
    expect(component.registerForm.controls['nombre'].valid).toBeFalse();
    expect(component.registerForm.controls['apellido'].valid).toBeTrue();
    expect(component.registerForm.controls['telefono'].valid).toBeTrue();
    expect(component.registerForm.controls['direccion'].valid).toBeFalse();
    expect(component.registerForm.controls['localidad'].valid).toBeFalse();
    expect(component.registerForm.controls['provincia'].valid).toBeFalse();
    expect(component.registerForm.controls['cuit'].valid).toBeTrue();
    expect(component.registerForm.controls['email'].valid).toBeFalse();
    expect(component.registerForm.controls['password'].valid).toBeFalse();
  });

  it('should set validators for apellido based on selectedRole', () => {
    component.selectedRole = 'usuario';
    component.ngOnInit();
    const control = component.registerForm.get('apellido') as AbstractControl;
    expect(control.errors).toBeNull();
    control.setValue('');
    expect(control.errors).toEqual({ required: true });

    component.selectedRole = 'organización';
    component.ngOnInit();
    const cuitControl = component.registerForm.get('cuit') as AbstractControl;
    expect(cuitControl.errors).toBeNull();
    cuitControl.setValue('');
    expect(cuitControl.errors).toEqual({ required: true });
  });

  it('should load provinces on initialization', () => {
    component.ngOnInit();
    expect(mapServiceMock.getProvinces).toHaveBeenCalled();
    expect(component.provinces.length).toBe(2);
    expect(component.provinces[0].nombre).toBe('Buenos Aires');
    expect(component.provinces[1].nombre).toBe('Cordoba');
  });
});