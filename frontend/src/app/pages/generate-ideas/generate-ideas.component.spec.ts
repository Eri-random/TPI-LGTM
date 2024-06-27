import { ComponentFixture, TestBed, fakeAsync } from '@angular/core/testing';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { NgToastService } from 'ng-angular-popup';

import { GenerateIdeasComponent } from './generate-ideas.component';
import { RecognitionTelaService } from 'src/app/services/recognition-tela.service';
import { AuthService } from 'src/app/services/auth.service';
import { GenerateIdeaService } from 'src/app/services/generate-idea.service';
import { SpinnerService } from 'src/app/services/spinner.service';
import { ResponseIdeaService } from 'src/app/services/response-idea.service';

describe('GenerateIdeasComponent', () => {
  let component: GenerateIdeasComponent;
  let fixture: ComponentFixture<GenerateIdeasComponent>;
  let recognitionTelaServiceMock: any;
  let authServiceMock: any;
  let generateIdeaServiceMock: any;
  let spinnerServiceMock: any;
  let responseIdeaServiceMock: any;
  let toastServiceMock: any;
  let routerMock: any;

  beforeEach(async () => {
    recognitionTelaServiceMock = {
      classifyImage: jasmine.createSpy('classifyImage').and.returnValue(of({ tela: 'Algodón' }))
    };

    authServiceMock = {
      isLoggedIn: jasmine.createSpy('isLoggedIn').and.returnValue(true)
    };

    generateIdeaServiceMock = {
      postGenerateIdea: jasmine.createSpy('postGenerateIdea').and.returnValue(of({
        titulo: 'Idea Reciclada',
        usuarioId: 1,
        dificultad: 'Fácil',
        pasos: [{ id: 1, pasoNum: 1, descripcion: 'Paso 1', ideaId: 1 }]
      }))
    };

    spinnerServiceMock = {
      show: jasmine.createSpy('show'),
      hide: jasmine.createSpy('hide'),
      showIdea: jasmine.createSpy('showIdea'),
      hideIdea: jasmine.createSpy('hideIdea')
    };

    responseIdeaServiceMock = {
      setGeneratedIdea: jasmine.createSpy('setGeneratedIdea'),
      setGeneratedIdeaMessage: jasmine.createSpy('setGeneratedIdeaMessage')
    };

    toastServiceMock = {
      error: jasmine.createSpy('error')
    };

    routerMock = {
      navigate: jasmine.createSpy('navigate')
    };

    await TestBed.configureTestingModule({
      declarations: [GenerateIdeasComponent],
      imports: [HttpClientTestingModule, FormsModule, ReactiveFormsModule],
      providers: [
        { provide: RecognitionTelaService, useValue: recognitionTelaServiceMock },
        { provide: AuthService, useValue: authServiceMock },
        { provide: GenerateIdeaService, useValue: generateIdeaServiceMock },
        { provide: SpinnerService, useValue: spinnerServiceMock },
        { provide: ResponseIdeaService, useValue: responseIdeaServiceMock },
        { provide: NgToastService, useValue: toastServiceMock },
        { provide: Router, useValue: routerMock }
      ]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(GenerateIdeasComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('debería crear el componente', () => {
    expect(component).toBeTruthy();
  });

  it('debería inicializarse correctamente', () => {
    component.ngOnInit();
    expect(authServiceMock.isLoggedIn).toHaveBeenCalled();
    expect(component.isLogged).toBeTrue();
  });

  it('debería agregar una imagen correctamente', () => {
    const initialLength = component.imagePreviews.length;
    component.addImage();
    expect(component.imagePreviews.length).toBe(initialLength + 1);
  });

  it('debería remover una imagen correctamente', () => {
    component.imagePreviews.push('dummy-url');
    const initialLength = component.imagePreviews.length;
    component.removeImage(initialLength - 1);
    expect(component.imagePreviews.length).toBe(initialLength - 1);
  });

  it('debería manejar el cambio de archivo correctamente', fakeAsync(() => {
    const file = new File([''], 'test.png', { type: 'image/png' });
    const event = { target: { files: [file] } };
    const readerSpy = spyOn(window as any, 'FileReader').and.callThrough();

    component.onFileChange(event, 0);

    expect(readerSpy).toHaveBeenCalled();
    expect(recognitionTelaServiceMock.classifyImage).toHaveBeenCalledWith(file);
    expect(spinnerServiceMock.showIdea).toHaveBeenCalled();
  }));

  it('debería manejar la presentación del formulario correctamente', fakeAsync(() => {
    component.ideaForm.setValue({
      tipoDeTela: 'Algodón',
      color: 'Azul',
      largo: '2',
      ancho: '1'
    });

    component.submitForm();

    expect(spinnerServiceMock.show).toHaveBeenCalled();
    expect(responseIdeaServiceMock.setGeneratedIdeaMessage).toHaveBeenCalled();
    expect(generateIdeaServiceMock.postGenerateIdea).toHaveBeenCalled();
    expect(spinnerServiceMock.hide).toHaveBeenCalled();
    expect(responseIdeaServiceMock.setGeneratedIdea).toHaveBeenCalled();
    expect(routerMock.navigate).toHaveBeenCalledWith(['/response-idea']);
  }));

  it('debería mostrar error si el reconocimiento de tela falla', fakeAsync(() => {
    recognitionTelaServiceMock.classifyImage.and.returnValue(throwError({ error: 'Error en el reconocimiento de la imagen' }));

    const file = new File([''], 'test.png', { type: 'image/png' });
    const event = { target: { files: [file] } };
    component.onFileChange(event, 0);

    expect(spinnerServiceMock.hideIdea).toHaveBeenCalled();
    expect(component.errorImage).toBe('Error en el reconocimiento de la imagen');
  }));

  it('debería validar el formulario correctamente', () => {
    component.submitForm();
    expect(component.ideaForm.valid).toBeFalse();
  });

});
