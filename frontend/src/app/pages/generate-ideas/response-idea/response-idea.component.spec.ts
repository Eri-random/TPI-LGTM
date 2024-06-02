import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { NgToastService } from 'ng-angular-popup';
import { ResponseIdeaComponent } from './response-idea.component';
import { ResponseIdeaService } from 'src/app/services/response-idea.service';
import { AuthService } from 'src/app/services/auth.service';
import { UserStoreService } from 'src/app/services/user-store.service';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';

describe('ResponseIdeaComponent', () => {
  let component: ResponseIdeaComponent;
  let fixture: ComponentFixture<ResponseIdeaComponent>;
  let responseIdeaServiceMock: any;
  let routerMock: any;
  let toastServiceMock: any;
  let authServiceMock: any;
  let userStoreMock: any;

  beforeEach(async () => {
    responseIdeaServiceMock = {
      getGeneratedIdea: jasmine.createSpy('getGeneratedIdea').and.returnValue({
        idea: 'Idea Reciclada',
        dificultad: 'Fácil',
        steps: ['Paso 1', 'Paso 2']
      }),
      postSaveIdea: jasmine.createSpy('postSaveIdea').and.returnValue(of({}))
    };

    authServiceMock = {
      getEmailFromToken: jasmine.createSpy('getEmailFromToken').and.returnValue('test@example.com')
    };

    userStoreMock = {
      getUserByEmail: jasmine.createSpy('getUserByEmail').and.returnValue(of({ id: 1 }))
    };

    toastServiceMock = {
      success: jasmine.createSpy('success'),
      error: jasmine.createSpy('error')
    };

    routerMock = {
      navigate: jasmine.createSpy('navigate')
    };

    await TestBed.configureTestingModule({
      declarations: [ResponseIdeaComponent],
      providers: [
        { provide: ResponseIdeaService, useValue: responseIdeaServiceMock },
        { provide: AuthService, useValue: authServiceMock },
        { provide: UserStoreService, useValue: userStoreMock },
        { provide: NgToastService, useValue: toastServiceMock },
        { provide: Router, useValue: routerMock }
      ],
      schemas: [CUSTOM_ELEMENTS_SCHEMA]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ResponseIdeaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('debería crear el componente', () => {
    expect(component).toBeTruthy();
  });

  it('debería inicializarse correctamente', fakeAsync(() => {
    component.ngOnInit();
    expect(responseIdeaServiceMock.getGeneratedIdea).toHaveBeenCalled();
    expect(authServiceMock.getEmailFromToken).toHaveBeenCalled();
    expect(userStoreMock.getUserByEmail).toHaveBeenCalledWith('test@example.com');
    expect(component.response).toEqual({
      idea: 'Idea Reciclada',
      dificultad: 'Fácil',
      steps: ['Paso 1', 'Paso 2']
    });
    expect(component.email).toBe('test@example.com');
    expect(component.userId).toBe(1);
  }));

  it('debería navegar a la página de generar ideas cuando se llama a generateNewIdea', () => {
    component.generateNewIdea();
    expect(routerMock.navigate).toHaveBeenCalledWith(['/generar-ideas']);
  });

  it('debería guardar la idea y mostrar un mensaje de éxito', fakeAsync(() => {
    component.ngOnInit();
    component.saveIdea();

    expect(toastServiceMock.success).toHaveBeenCalledWith({
      detail: 'EXITO',
      summary: 'Idea guardada exitosamente',
      duration: 3000,
      position: 'topCenter',
    });

    tick(3000);
    expect(routerMock.navigate).toHaveBeenCalledWith(['/mis-ideas']);
  }));

  
});
