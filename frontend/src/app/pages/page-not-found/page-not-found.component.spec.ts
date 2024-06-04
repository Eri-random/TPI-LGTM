import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/services/auth.service';
import { PageNotFoundComponent } from './page-not-found.component';
import { of } from 'rxjs';

describe('PageNotFoundComponent', () => {
  let component: PageNotFoundComponent;
  let fixture: ComponentFixture<PageNotFoundComponent>;
  let authServiceMock: any;
  let routerMock: any;

  beforeEach(async () => {
    authServiceMock = jasmine.createSpyObj('AuthService', ['getRoleFromToken']);
    routerMock = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      declarations: [PageNotFoundComponent],
      providers: [
        { provide: AuthService, useValue: authServiceMock },
        { provide: Router, useValue: routerMock }
      ]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PageNotFoundComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('debería crear el componente', () => {
    expect(component).toBeTruthy();
  });

  it('debería obtener el rol del token en ngOnInit', () => {
    const rol = 'organizacion';
    authServiceMock.getRoleFromToken.and.returnValue(rol);

    component.ngOnInit();
    expect(component.rol).toBe(rol);
    expect(authServiceMock.getRoleFromToken).toHaveBeenCalled();
  });

  it('debería ir a "/dashboard" si el rol es "organización"', () => {
    const rol = 'organizacion';
    component.rol = rol;
    component.goBack();

    expect(routerMock.navigate).toHaveBeenCalledWith(['/dashboard']);
  });

  it('debería ir a la ruta por defecto si el rol no está', () => {
    const rol = 'desconocido';
    component.rol = rol;
    component.goBack();

    expect(routerMock.navigate).toHaveBeenCalledWith(['']);
  });

  it('debería ir a "/generar-ideas" si el rol es "usuario"', () => {
    const rol = 'usuario';
    component.rol = rol;
    component.goBack();

    expect(routerMock.navigate).toHaveBeenCalledWith(['/generar-ideas']);
  });
});
