import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { NgToastService } from 'ng-angular-popup';
import { of, throwError } from 'rxjs';
import { LoginComponent } from './login.component';
import { AuthService } from 'src/app/services/auth.service';
import { OrganizationService } from 'src/app/services/organization.service';
import { UserStoreService } from 'src/app/services/user-store.service';
import ValidateForm from 'src/app/helpers/validateForm';

describe('LoginComponent', () => {
  let component: LoginComponent;
  let fixture: ComponentFixture<LoginComponent>;
  let authServiceMock: any;
  let userStoreMock: any;
  let organizationServiceMock: any;
  let routerMock: any;
  let toastServiceMock: any;

  beforeEach(async () => {
    authServiceMock = jasmine.createSpyObj('AuthService', ['login', 'storeToken', 'decodedToken', 'setIsLoggedIn']);
    userStoreMock = jasmine.createSpyObj('UserStoreService', ['setFullNameForStore', 'setRolForStore', 'setEmailForStore']);
    organizationServiceMock = jasmine.createSpyObj('OrganizationService', ['setCuitForStore', 'setOrgNameForStore']);
    routerMock = jasmine.createSpyObj('Router', ['navigate']);
    toastServiceMock = jasmine.createSpyObj('NgToastService', ['success', 'error']);

    await TestBed.configureTestingModule({
      declarations: [LoginComponent],
      imports: [ReactiveFormsModule],
      providers: [
        { provide: AuthService, useValue: authServiceMock },
        { provide: UserStoreService, useValue: userStoreMock },
        { provide: OrganizationService, useValue: organizationServiceMock },
        { provide: Router, useValue: routerMock },
        { provide: NgToastService, useValue: toastServiceMock }
      ]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(LoginComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('debería crear el componente', () => {
    expect(component).toBeTruthy();
  });

  it('debería inicializar el formulario en ngOnInit', () => {
    
    component.ngOnInit();
    expect(component.loginForm).toBeDefined();
    expect(component.loginForm.controls['email'].valid).toBeFalse();
    expect(component.loginForm.controls['password'].valid).toBeFalse();
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

  it('debería manejar exitosamente el inicio de sesión para un usuario normal', () => {
    const tokenPayload = {
      name: 'Juan',
      role: 'usuario',
      email: 'juan.perez@ejemplo.com'
    };

    component.ngOnInit();
    component.loginForm.setValue({
      email: 'juan.perez@ejemplo.com',
      password: 'password123'
    });

    authServiceMock.login.and.returnValue(of({ token: 'fake-token' }));
    authServiceMock.decodedToken.and.returnValue(tokenPayload);

    component.onSubmit();

    expect(authServiceMock.login).toHaveBeenCalledWith({
      email: 'juan.perez@ejemplo.com',
      password: 'password123'
    });
    expect(authServiceMock.storeToken).toHaveBeenCalledWith('fake-token');
    expect(userStoreMock.setFullNameForStore).toHaveBeenCalledWith(tokenPayload.name);
    expect(userStoreMock.setRolForStore).toHaveBeenCalledWith(tokenPayload.role);
    expect(userStoreMock.setEmailForStore).toHaveBeenCalledWith(tokenPayload.email);
    expect(authServiceMock.setIsLoggedIn).toHaveBeenCalledWith(true);
    expect(toastServiceMock.success).toHaveBeenCalledWith({
      detail: 'EXITO',
      summary: 'Login exitoso',
      duration: 5000,
      position: 'topRight'
    });
    expect(routerMock.navigate).toHaveBeenCalledWith(['/']);
  });

  it('debería manejar exitosamente el inicio de sesión para una organización', () => {
    const tokenPayload = {
      name: 'Organización XYZ',
      role: 'organizacion',
      email: 'contacto@organizacionxyz.com',
      cuit: '20-12345678-9',
      orgName: 'Organización XYZ'
    };

    component.ngOnInit();
    component.loginForm.setValue({
      email: 'contacto@organizacionxyz.com',
      password: 'password456'
    });

    authServiceMock.login.and.returnValue(of({ token: 'fake-token' }));
    authServiceMock.decodedToken.and.returnValue(tokenPayload);

    component.onSubmit();

    expect(authServiceMock.login).toHaveBeenCalledWith({
      email: 'contacto@organizacionxyz.com',
      password: 'password456'
    });
    expect(authServiceMock.storeToken).toHaveBeenCalledWith('fake-token');
    expect(userStoreMock.setFullNameForStore).toHaveBeenCalledWith(tokenPayload.name);
    expect(userStoreMock.setRolForStore).toHaveBeenCalledWith(tokenPayload.role);
    expect(userStoreMock.setEmailForStore).toHaveBeenCalledWith(tokenPayload.email);
    expect(authServiceMock.setIsLoggedIn).toHaveBeenCalledWith(true);
    expect(organizationServiceMock.setCuitForStore).toHaveBeenCalledWith(tokenPayload.cuit);
    expect(organizationServiceMock.setOrgNameForStore).toHaveBeenCalledWith(tokenPayload.orgName);
    expect(toastServiceMock.success).toHaveBeenCalledWith({
      detail: 'EXITO',
      summary: 'Login exitoso',
      duration: 5000,
      position: 'topRight'
    });
    expect(routerMock.navigate).toHaveBeenCalledWith(['/dashboard']);
  });

  it('debería manejar el error de inicio de sesión', () => {
    component.ngOnInit();
    component.loginForm.setValue({
      email: 'usuario@ejemplo.com',
      password: 'password123'
    });

    authServiceMock.login.and.returnValue(throwError({ error: 'Credenciales inválidas' }));

    component.onSubmit();

    expect(authServiceMock.login).toHaveBeenCalledWith({
      email: 'usuario@ejemplo.com',
      password: 'password123'
    });
    expect(component.error).toBe('Credenciales inválidas');
  });

  it('debería validar los campos del formulario en la presentación si el formulario es inválido', () => {
    spyOn(ValidateForm, 'validateAllFormFileds');
    component.ngOnInit();
    component.loginForm.controls['email'].setValue('');

    component.onSubmit();

    expect(ValidateForm.validateAllFormFileds).toHaveBeenCalledWith(component.loginForm);
  });
});
