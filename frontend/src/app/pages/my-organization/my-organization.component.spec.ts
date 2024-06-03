import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { MyOrganizationComponent } from './my-organization.component';
import { AuthService } from 'src/app/services/auth.service';
import { OrganizationService } from 'src/app/services/organization.service';
import { NgToastService } from 'ng-angular-popup';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { EditorModule } from '@tinymce/tinymce-angular';

describe('MyOrganizationComponent', () => {
  let component: MyOrganizationComponent;
  let fixture: ComponentFixture<MyOrganizationComponent>;
  let authServiceMock: any;
  let organizationServiceMock: any;
  let toastServiceMock: any;
  let routerMock: any;
  let httpMock: HttpTestingController;

  beforeEach(async () => {
    authServiceMock = jasmine.createSpyObj('AuthService', ['getCuitFromToken']);
    organizationServiceMock = jasmine.createSpyObj('OrganizationService', [
      'getCuitFromStore',
      'getOrganizationByCuit',
      'postInfoOrganization',
      'putInfoOrganization',
    ]);
    toastServiceMock = jasmine.createSpyObj('NgToastService', ['success', 'error']);
    routerMock = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      declarations: [MyOrganizationComponent],
      imports: [HttpClientTestingModule, ReactiveFormsModule, EditorModule],
      providers: [
        { provide: AuthService, useValue: authServiceMock },
        { provide: OrganizationService, useValue: organizationServiceMock },
        { provide: NgToastService, useValue: toastServiceMock },
        { provide: Router, useValue: routerMock },
      ],
      schemas: [NO_ERRORS_SCHEMA]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MyOrganizationComponent);
    component = fixture.componentInstance;
    httpMock = TestBed.inject(HttpTestingController);

    organizationServiceMock.getCuitFromStore.and.returnValue(of('123456789'));
    authServiceMock.getCuitFromToken.and.returnValue('123456789');
    organizationServiceMock.getOrganizationByCuit.and.returnValue(of({
      nombre: 'Mi Organización',
      id: '1',
      infoOrganizacion: {
        descripcionBreve: 'Descripción breve',
        descripcionCompleta: 'Descripción completa',
        img: 'image_url',
      },
    }));

    fixture.detectChanges();
  });

  afterEach(() => {
    httpMock.verify(); // Verifica que no haya solicitudes HTTP pendientes
  });

  it('debería crear el componente', () => {
    expect(component).toBeTruthy();
  });

  it('debería inicializar el formulario con datos de la organización', () => {
    component.ngOnInit();
    fixture.detectChanges();

    expect(component.organizationForm.get('organizacion')?.value).toBe('Mi Organización');
    expect(component.organizationForm.get('organizacionId')?.value).toBe('1');
    expect(component.organizationForm.get('descripcionBreve')?.value).toBe('Descripción breve');
    expect(component.organizationForm.get('descripcionCompleta')?.value).toBe('Descripción completa');
    expect(component.imageSrc).toBe('image_url');
    expect(component.isEditMode).toBeTrue();
  });

  it('debería enviar el formulario y manejar la respuesta correctamente en modo de edición', () => {
    component.isEditMode = true;
    component.organizationForm.patchValue({
      organizacion: 'Mi Organización',
      descripcionBreve: 'Descripción breve',
      descripcionCompleta: 'Descripción completa',
      organizacionId: '1',
    });

    organizationServiceMock.putInfoOrganization.and.returnValue(of({}));

    component.onSubmit();

    expect(organizationServiceMock.putInfoOrganization).toHaveBeenCalled();
    expect(toastServiceMock.success).toHaveBeenCalledWith({
      detail: 'La información se guardó correctamente.',
      duration: 5000,
      position: 'topCenter',
    });
  });

  it('debería enviar el formulario y manejar la respuesta correctamente en modo de creación', () => {
    component.isEditMode = false;
    component.organizationForm.patchValue({
      organizacion: 'Mi Organización',
      descripcionBreve: 'Descripción breve',
      descripcionCompleta: 'Descripción completa',
      organizacionId: '1',
    });

    organizationServiceMock.postInfoOrganization.and.returnValue(of({}));

    component.onSubmit();

    expect(organizationServiceMock.postInfoOrganization).toHaveBeenCalled();
    expect(toastServiceMock.success).toHaveBeenCalledWith({
      detail: 'La información se guardó correctamente.',
      duration: 5000,
      position: 'topCenter',
    });
    expect(routerMock.navigate).toHaveBeenCalledWith(['/dashboard']);
  });

  it('debería manejar errores al enviar el formulario', () => {
    component.isEditMode = true;
    component.organizationForm.patchValue({
      organizacion: 'Mi Organización',
      descripcionBreve: 'Descripción breve',
      descripcionCompleta: 'Descripción completa',
      organizacionId: '1',
    });

    const errorResponse = new HttpErrorResponse({ status: 404, statusText: 'Not Found' });
    organizationServiceMock.putInfoOrganization.and.returnValue(throwError(() => errorResponse));

    component.onSubmit();

    expect(organizationServiceMock.putInfoOrganization).toHaveBeenCalled();
    expect(toastServiceMock.error).toHaveBeenCalledWith({
      detail: 'Ocurrió un error al intentar guardar la información. Intente nuevamente.',
      duration: 5000,
      position: 'topCenter',
    });
  });
});
