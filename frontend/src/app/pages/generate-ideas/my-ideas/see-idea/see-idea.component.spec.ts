import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of, throwError } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { ResponseIdeaService } from 'src/app/services/response-idea.service';
import { SeeIdeaComponent } from './see-idea.component';
import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import * as jsPDF from 'jspdf';
import autoTable from 'jspdf-autotable';

describe('SeeIdeaComponent', () => {
  let component: SeeIdeaComponent;
  let fixture: ComponentFixture<SeeIdeaComponent>;
  let mockResponseIdeaService: jasmine.SpyObj<ResponseIdeaService>;
  let mockActivatedRoute: any;

  beforeEach(async () => {
    const responseIdeaServiceSpy = jasmine.createSpyObj('ResponseIdeaService', ['getIdea']);
    mockActivatedRoute = {
      params: of({ id: 1 })
    };

    await TestBed.configureTestingModule({
      declarations: [SeeIdeaComponent],
      providers: [
        { provide: ResponseIdeaService, useValue: responseIdeaServiceSpy },
        { provide: ActivatedRoute, useValue: mockActivatedRoute }
      ],
      schemas: [CUSTOM_ELEMENTS_SCHEMA]
    }).compileComponents();

    fixture = TestBed.createComponent(SeeIdeaComponent);
    component = fixture.componentInstance;
    mockResponseIdeaService = TestBed.inject(ResponseIdeaService) as jasmine.SpyObj<ResponseIdeaService>;
  });

  it('debería crear el componente', () => {
    expect(component).toBeTruthy();
  });

  it('debería inicializar y cargar datos de la idea', () => {
    const mockIdea = {
      titulo: 'Idea de Prueba',
      usuarioId: 1,
      pasos: [{ descripcion: 'Paso 1' }, { descripcion: 'Paso 2' }],
      dificultad: 'Media'
    };

    mockResponseIdeaService.getIdea.and.returnValue(of(mockIdea));

    component.ngOnInit();

    expect(mockResponseIdeaService.getIdea).toHaveBeenCalledWith(1);
    expect(component.idea).toEqual(mockIdea);
  });


  it('debería manejar el error al cargar datos de la idea', () => {
    const mockError = new Error('Error al cargar la idea');

    mockResponseIdeaService.getIdea.and.returnValue(throwError(mockError));
    spyOn(console, 'error'); // Aquí estamos creando un espía para console.error

    component.ngOnInit();

    expect(mockResponseIdeaService.getIdea).toHaveBeenCalledWith(1);
    expect(console.error).toHaveBeenCalledWith('Error al cargar la idea:', mockError);
  });


  it('debería cargar una imagen y convertirla a base64', (done) => {
    const imageUrl = 'assets/logos/recrea.jpg';
    const base64Image = 'data:image/jpeg;base64,...'; // Base64 de ejemplo
  
    spyOn(component, 'loadImage').and.callThrough();
  
    const imgMock = new Image();
    imgMock.crossOrigin = 'Anonymous';
  
    // Aseguramos que `onload` no es null antes de invocarlo
    imgMock.onload = () => {
      done();
    };
  
    Object.defineProperty(imgMock, 'src', {
      set: (url: string) => {
        if (url === imageUrl) {
          setTimeout(() => {
            if (imgMock.onload) {
              imgMock.onload();
            }
          }, 100);
        }
      },
      configurable: true
    });
  
    spyOn(window, 'Image').and.returnValue(imgMock);
  
    component.loadImage(imageUrl).then((base64) => {
      expect(base64).toBe(base64Image);
    });
  
    component.ngAfterViewInit();
  }, 10000); // Aumenta el tiempo de espera predeterminado a 10000ms
  


  it('debería generar un PDF con la idea', () => {
    const mockIdea = {
      titulo: 'Idea de Prueba',
      usuarioId: 1,
      pasos: [{ descripcion: 'Paso 1' }, { descripcion: 'Paso 2' }],
      dificultad: 'Media'
    };

    component.idea = mockIdea;
    component.logoBase64 = 'data:image/jpeg;base64,...'; // Base64 de ejemplo

    const doc = new jsPDF.default();
    const docSaveSpy = spyOn(doc, 'save');

    const autoTableSpy = spyOn<any>(autoTable, 'applyPlugin').and.callFake(() => { });

    component.downloadPDF();

    expect(docSaveSpy).toHaveBeenCalledWith('idea-Idea de Prueba.pdf');
    expect(autoTableSpy).toHaveBeenCalled();
  });

  it('debería mostrar un error si el logo no está cargado al intentar generar un PDF', () => {
    spyOn(console, 'error');

    component.downloadPDF();

    expect(console.error).toHaveBeenCalledWith('Logo not loaded yet');
  });
});
