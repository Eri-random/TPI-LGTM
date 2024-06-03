import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { OrganizationRequestComponent } from './organization-request.component';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { NeedService } from 'src/app/services/need.service';
import { OrganizationService } from 'src/app/services/organization.service';
import { AuthService } from 'src/app/services/auth.service';
import { NgToastService } from 'ng-angular-popup';
import { of } from 'rxjs';

describe('OrganizationRequestComponent', () => {
  let component: OrganizationRequestComponent;
  let fixture: ComponentFixture<OrganizationRequestComponent>;
  let needsServiceMock: any;
  let organizationServiceMock: any;
  let authServiceMock: any;
  let toastServiceMock: any;

  beforeEach(async () => {
    needsServiceMock = {
      getAllNeeds: jasmine.createSpy('getAllNeeds').and.returnValue(of([]))
    };

    organizationServiceMock = {
      getCuitFromStore: jasmine.createSpy('getCuitFromStore').and.returnValue(of('')),
      getOrganizationByCuit: jasmine.createSpy('getOrganizationByCuit').and.returnValue(of({ id: 1 })),
      getAssignedSubcategories: jasmine.createSpy('getAssignedSubcategories').and.returnValue(of([])),
      assignSubcategories: jasmine.createSpy('assignSubcategories').and.returnValue(of({ message: 'Asignación exitosa' }))
    };

    authServiceMock = {
      getCuitFromToken: jasmine.createSpy('getCuitFromToken').and.returnValue('123456789')
    };

    toastServiceMock = {
      success: jasmine.createSpy('success')
    };

    await TestBed.configureTestingModule({
      declarations: [ OrganizationRequestComponent ],
      imports: [ ReactiveFormsModule ],
      providers: [
        FormBuilder,
        { provide: NeedService, useValue: needsServiceMock },
        { provide: OrganizationService, useValue: organizationServiceMock },
        { provide: AuthService, useValue: authServiceMock },
        { provide: NgToastService, useValue: toastServiceMock }
      ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(OrganizationRequestComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('debería crear el componente', () => {
    expect(component).toBeTruthy();
  });

  it('debería inicializarse correctamente', fakeAsync(() => {
    component.ngOnInit();
    expect(organizationServiceMock.getCuitFromStore).toHaveBeenCalled();
    expect(authServiceMock.getCuitFromToken).toHaveBeenCalled();
    expect(organizationServiceMock.getOrganizationByCuit).toHaveBeenCalledWith('123456789');
    expect(needsServiceMock.getAllNeeds).toHaveBeenCalled();
    tick(1000); // Simula el tiempo de espera en setTimeout
    expect(component.loading).toBe(false); // Verifica que la carga esté completa
  }));

  it('debería cargar las necesidades', fakeAsync(() => {
    const needsMock = [
      { nombre: 'Necesidad 1', subcategoria: [{ nombre: 'Sub 1', id: 1 }] },
      { nombre: 'Necesidad 2', subcategoria: [{ nombre: 'Sub 2', id: 2 }] }
    ];
    needsServiceMock.getAllNeeds.and.returnValue(of(needsMock));
    
    component.loadForm();
    tick(1000); // Simula el tiempo de espera en setTimeout
    expect(component.needs).toEqual(needsMock);
    expect(component.formGroups['Necesidad 1']).toBeDefined();
    expect(component.formGroups['Necesidad 2']).toBeDefined();
    expect(component.loading).toBe(false); // Verifica que la carga esté completa
  }));

  // it('debería cargar las subcategorías asignadas', fakeAsync(() => {
  //   const assignedMock = [{ id: 1 }];
  //   const needsMock = [
  //     { nombre: 'Necesidad 1', subcategoria: [{ nombre: 'Sub 1', id: 1 }] }
  //   ];
  //   component.needs = needsMock;
  //   component.formGroups['Necesidad 1'] = new FormBuilder().group({
  //     'Sub 1': [false]
  //   });

  //   organizationServiceMock.getAssignedSubcategories.and.returnValue(of(assignedMock));
    
  //   component.loadNeeds();
    
  //   expect(component.formGroups['Necesidad 1'].get('Sub 1')?.value).toBe(true);
  // }));

  it('debería obtener las subcategorías seleccionadas', () => {
    const needsMock = [
      { id: 1, nombre: 'Necesidad 1', subcategoria: [{ id: 1, nombre: 'Sub 1' }] }
    ];
    component.needs = needsMock;
    component.formGroups['Necesidad 1'] = new FormBuilder().group({
      'Sub 1': [true]
    });

    const selectedSubcategories = component.GetSelectedSubcategories();
    expect(selectedSubcategories).toEqual([{ id: 1, nombre: 'Sub 1', necesidadId: 1 }]);
  });

  it('debería guardar las necesidades seleccionadas', fakeAsync(() => {
    const selectedSubcategories = [{ id: 1, nombre: 'Sub 1', necesidadId: 1 }];
    spyOn(component, 'GetSelectedSubcategories').and.returnValue(selectedSubcategories);

    component.saveNeeds();

    expect(organizationServiceMock.assignSubcategories).toHaveBeenCalledWith(1, selectedSubcategories);
    expect(toastServiceMock.success).toHaveBeenCalledWith(jasmine.objectContaining({
      detail: 'EXITO',
      summary: 'Asignación exitosa'
    }));
  }));
});
