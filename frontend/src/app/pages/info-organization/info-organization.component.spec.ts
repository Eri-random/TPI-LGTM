import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MatDialog } from '@angular/material/dialog';
import { DomSanitizer } from '@angular/platform-browser';
import { ActivatedRoute } from '@angular/router';
import { of, throwError } from 'rxjs';
import { InfoOrganizationComponent } from './info-organization.component';
import { OrganizationService } from 'src/app/services/organization.service';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { DialogDonateComponent } from './dialog-donate/dialog-donate.component';

describe('InfoOrganizationComponent', () => {
  let component: InfoOrganizationComponent;
  let fixture: ComponentFixture<InfoOrganizationComponent>;
  let mockOrganizationService: any;
  let mockSanitizer: any;
  let mockDialog: any;
  let mockActivatedRoute: any;

  beforeEach(async () => {
    mockOrganizationService = jasmine.createSpyObj('OrganizationService', ['getOrganizationById']);
    mockSanitizer = jasmine.createSpyObj('DomSanitizer', ['bypassSecurityTrustHtml']);
    mockDialog = jasmine.createSpyObj('MatDialog', ['open']);
    mockActivatedRoute = {
      params: of({ id: 1 })
    };

    await TestBed.configureTestingModule({
      declarations: [InfoOrganizationComponent],
      providers: [
        { provide: OrganizationService, useValue: mockOrganizationService },
        { provide: DomSanitizer, useValue: mockSanitizer },
        { provide: MatDialog, useValue: mockDialog },
        { provide: ActivatedRoute, useValue: mockActivatedRoute },
      ],
      schemas: [NO_ERRORS_SCHEMA]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(InfoOrganizationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('debería crear el componente', () => {
    expect(component).toBeTruthy();
  });

  it('debería inicializar los datos de la organización en ngOnInit', () => {
    const mockData = {
      id: 1,
      infoOrganizacion: {
        descripcionCompleta: '<p>Descripción completa</p>'
      }
    };

    mockOrganizationService.getOrganizationById.and.returnValue(of(mockData));
    mockSanitizer.bypassSecurityTrustHtml.and.returnValue('safeHtmlContent');

    component.ngOnInit();

    expect(mockActivatedRoute.params.subscribe).toBeTruthy();
    expect(mockOrganizationService.getOrganizationById).toHaveBeenCalledWith(1);
    expect(component.safeContent).toBe('safeHtmlContent');
    expect(component.organization).toEqual(mockData);
  });

  it('debería manejar errores al cargar datos de la organización', () => {
    const errorResponse = new Error('Error al cargar la organización');

    mockOrganizationService.getOrganizationById.and.returnValue(throwError(() => errorResponse));
    spyOn(console, 'error');

    component.ngOnInit();

    expect(mockOrganizationService.getOrganizationById).toHaveBeenCalledWith(1);
    expect(console.error).toHaveBeenCalledWith(errorResponse);
  });

  it('debería sanitizar el contenido correctamente', () => {
    const unsafeContent = '<p>Descripción completa</p>';
    const safeContent = 'safeHtmlContent';

    mockSanitizer.bypassSecurityTrustHtml.and.returnValue(safeContent);

    const result = component.sanitizeContent(unsafeContent);

    expect(mockSanitizer.bypassSecurityTrustHtml).toHaveBeenCalledWith(unsafeContent);
    expect(result).toBe(safeContent);
  });

  it('debería abrir el diálogo de donación con los datos correctos', () => {
    component.organization = { id: 1 };

    component.openDialog();

    expect(mockDialog.open).toHaveBeenCalledWith(DialogDonateComponent, {
      width: 'auto',
      height: '80%',
      data: { organizacionId: 1 }
    });
  });
});
