import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { NgToastService } from 'ng-angular-popup';
import { of, throwError } from 'rxjs';
import { SignupComponent } from './signup.component';
import { AuthService } from 'src/app/services/auth.service';
import { User } from 'src/app/models/user';
import { Roles } from 'src/app/utils/roles.enum';
import ValidateForm from 'src/app/helpers/validateForm';

describe('SignupComponent', () => {
  let component: SignupComponent;
  let fixture: ComponentFixture<SignupComponent>;
  let authServiceMock: any;
  let routerMock: any;
  let toastServiceMock: any;

  beforeEach(async () => {
    authServiceMock = jasmine.createSpyObj('AuthService', ['createAccount']);
    routerMock = jasmine.createSpyObj('Router', ['navigate']);
    toastServiceMock = jasmine.createSpyObj('NgToastService', ['success', 'error']);

    await TestBed.configureTestingModule({
      declarations: [SignupComponent],
      imports: [ReactiveFormsModule],
      providers: [
        { provide: AuthService, useValue: authServiceMock },
        { provide: Router, useValue: routerMock },
        { provide: NgToastService, useValue: toastServiceMock }
      ]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SignupComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('debería crear el componente', () => {
    expect(component).toBeTruthy();
  });

  it('debería inicializar el formulario en ngOnInit', () => {
    component.ngOnInit();
    expect(component.registerForm).toBeDefined();
    expect(component.registerForm.controls['nombre'].valid).toBeFalse();
    expect(component.registerForm.controls['apellido'].valid).toBeTrue();
    expect(component.registerForm.controls['cuit'].valid).toBeTrue();
  });

  it('debería alternar la visibilidad de la contraseña', () => {
    component.hideShowPass();
    expect(component.isText).toBeTrue();
    expect(component.eyeIcon).toBe('fa-eye');
    expect(component.type).toBe('text');
    
    component.hideShowPass();
    expect(component.isText).toBeFalse();
    expect(component.eyeIcon).toBe('fa-eye-slash');
    expect(component.type).toBe('password');
  });

  it('debería establecer selectedRole y actualizar validadores', () => {
    component.selectedRole = 'usuario';
    component.ngOnInit();
    component.registerForm.controls['apellido'].setValue('');
    expect(component.registerForm.controls['apellido'].valid).toBeFalse();
    
    component.selectedRole = 'organización';
    component.ngOnInit();
    component.registerForm.controls['cuit'].setValue('');
    expect(component.registerForm.controls['cuit'].valid).toBeFalse();
  });

  it('debería manejar el registro exitoso de un usuario normal', () => {
    component.ngOnInit();
    component.selectedRole = 'usuario';
    component.registerForm.setValue({
      nombre: 'Juan',
      apellido: 'Perez',
      telefono: '12345678',
      direccion: 'Calle Falsa 123',
      localidad: 'Ciudad',
      provincia: 'Provincia',
      cuit: null,
      email: 'juan.perez@ejemplo.com',
      password: 'password123'
    });
  
    const nuevoUsuario: User = {
      ...component.registerForm.value,
      rolId: Roles.User
    };
  
    authServiceMock.createAccount.and.returnValue(of(nuevoUsuario));
  
    component.onSubmit();
  
    expect(authServiceMock.createAccount).toHaveBeenCalledWith(nuevoUsuario);
    expect(toastServiceMock.success).toHaveBeenCalledWith({
      detail: 'EXITO',
      summary: 'Usuario registrado correctamente',
      duration: 5000
    });
    expect(routerMock.navigate).toHaveBeenCalledWith(['/login']);
  });

  it('debería manejar el registro exitoso de una organización', () => {
    component.ngOnInit();
    component.selectedRole = 'organización';
    component.registerForm.setValue({
      nombre: 'Organización XYZ',
      apellido: null,
      telefono: '87654321',
      direccion: 'Avenida Siempre Viva 742',
      localidad: 'San Justo',
      provincia: 'Buenos Aires',
      cuit: '20-12345678-9',
      email: 'contacto@organizacionxyz.com',
      password: 'password456'
    });
  
    const nuevaOrganizacion: User = {
      ...component.registerForm.value,
      rolId: Roles.Organization
    };
  
    authServiceMock.createAccount.and.returnValue(of(nuevaOrganizacion));
  
    component.onSubmit();
  
    expect(authServiceMock.createAccount).toHaveBeenCalledWith(nuevaOrganizacion);
    expect(toastServiceMock.success).toHaveBeenCalledWith({
      detail: 'EXITO',
      summary: 'Usuario registrado correctamente',
      duration: 5000
    });
    expect(routerMock.navigate).toHaveBeenCalledWith(['/login']);
  });  

  it('debería manejar la presentación del formulario con error', () => {
    component.ngOnInit();
    component.selectedRole = 'usuario';
    component.registerForm.setValue({
      nombre: 'Peter',
      apellido: 'Parker',
      telefono: '12345678',
      direccion: 'Calle 123',
      localidad: 'San Justo',
      provincia: 'Buenos Aires',
      cuit: null,
      email: 'spiderman@ejemplo.com',
      password: 'password123'
    });

    authServiceMock.createAccount.and.returnValue(throwError({ error: 'Some error' }));

    component.onSubmit();

    expect(authServiceMock.createAccount).toHaveBeenCalled();
    expect(toastServiceMock.error).toHaveBeenCalledWith({
      detail: 'ERROR',
      summary: 'Some error',
      duration: 5000,
      position: 'topCenter'
    });
  });

  it('debería validar los campos del formulario en la presentación si el formulario es inválido', () => {
    spyOn(ValidateForm, 'validateAllFormFileds');
    component.ngOnInit();
    component.registerForm.controls['nombre'].setValue('');

    component.onSubmit();

    expect(ValidateForm.validateAllFormFileds).toHaveBeenCalledWith(component.registerForm);
  });

  it('debería actualizar el valor de telefono en el formulario al recibir input', () => {
    const event = { target: { value: '12345678' } } as any;
    component.onInput(event);

    expect(component.registerForm.value.telefono).toBe('12345678');
  });

  it('debería establecer selectedRole y cambiar la visibilidad', () => {
    component.seleccionar('usuario');
    expect(component.selectedRole).toBe('usuario');

    component.enableForm();
    expect(component.visibilityForm).toBe('d-block');
    expect(component.visibilityRol).toBe('d-none');
  });
});
