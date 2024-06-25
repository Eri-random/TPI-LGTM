import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { NgToastService } from 'ng-angular-popup';
import { ResponseIdeaComponent } from './response-idea.component';
import { ResponseIdeaService } from 'src/app/services/response-idea.service';
import { AuthService } from 'src/app/services/auth.service';
import { UserStoreService } from 'src/app/services/user-store.service';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { GenerateIdeaService } from 'src/app/services/generate-idea.service';


describe('ResponseIdeaComponent', () => {
  let component: ResponseIdeaComponent;
  let fixture: ComponentFixture<ResponseIdeaComponent>;
  let responseIdeaServiceMock: any;
  let generateIdeaServiceMock: any;
  let routerMock: any;
  let toastServiceMock: any;
  let authServiceMock: any;
  let userStoreMock: any;
  let spinnerServiceMock: any;

  beforeEach(async () => {
    responseIdeaServiceMock = {
      getGeneratedIdea: jasmine.createSpy('getGeneratedIdea').and.returnValue({
        idea: 'Idea Reciclada',
        dificultad: 'Fácil',
        steps: ['Paso 1', 'Paso 2']
      }),
      getGeneratedIdeaMessage: jasmine.createSpy('getGeneratedIdeaMessage').and.returnValue(
        {
          "message": "string"
        }),
      postSaveIdea: jasmine.createSpy('postSaveIdea').and.returnValue(of({}))
    };

    authServiceMock = {
      getEmailFromToken: jasmine.createSpy('getEmailFromToken').and.returnValue('test@example.com')
    };

    userStoreMock = {
      getUserByEmail: jasmine.createSpy('getUserByEmail').and.returnValue(of({ id: 1 })),
      getEmailFromStore: jasmine.createSpy('getEmailFromStore').and.returnValue(of())
    };

    toastServiceMock = {
      success: jasmine.createSpy('success'),
      error: jasmine.createSpy('error')
    };

    routerMock = {
      navigate: jasmine.createSpy('navigate')
    };

    const dialogSpy = jasmine.createSpyObj('MatDialog', ['open', 'closeAll']);

    spinnerServiceMock = {
      show: jasmine.createSpy('show'),
      hide: jasmine.createSpy('hide'),
      showIdea: jasmine.createSpy('showIdea'),
      hideIdea: jasmine.createSpy('hideIdea')
    };

    generateIdeaServiceMock = {
      postGenerateIdea: jasmine.createSpy('postGenerateIdea').and.returnValue(of({
        titulo: 'Idea Reciclada',
        usuarioId: 1,
        dificultad: 'Fácil',
        pasos: [{ id: 1, pasoNum: 1, descripcion: 'Paso 1', ideaId: 1 }]
      }))
    };

    await TestBed.configureTestingModule({
      declarations: [ResponseIdeaComponent],
      providers: [
        { provide: ResponseIdeaService, useValue: responseIdeaServiceMock },
        { provide: AuthService, useValue: authServiceMock },
        { provide: UserStoreService, useValue: userStoreMock },
        { provide: NgToastService, useValue: toastServiceMock },
        { provide: Router, useValue: routerMock },
        { provide: MatDialog, useValue: dialogSpy },
        { provide: GenerateIdeaService, useValue: generateIdeaServiceMock },
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
    userStoreMock.getEmailFromStore.and.returnValue(of('test@example.com'));

    userStoreMock.getUserByEmail.and.returnValue(of({ id: 1 }));

    component.ngOnInit();

    // Simulate the passage of time for asynchronous operations
    tick();

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
      position: 'topRight',
    });

    tick(3000);
    expect(routerMock.navigate).toHaveBeenCalledWith(['/mis-ideas']);
  }));

  it('debería manejar errores al guardar la idea', fakeAsync(() => {
    responseIdeaServiceMock.postSaveIdea.and.returnValue(throwError(() => ({ error: 'Error al guardar la idea' })));
    component.ngOnInit();
    component.saveIdea();
    tick(); 
  
    expect(responseIdeaServiceMock.postSaveIdea).toHaveBeenCalled();
    expect(toastServiceMock.error).toHaveBeenCalledWith({
      detail: 'ERROR',
      summary: 'Error al guardar la idea',
      duration: 3000,
      position: 'topRight',
    });
    expect(routerMock.navigate).not.toHaveBeenCalledWith(['/mis-ideas']);
  }));
  
});
