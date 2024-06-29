import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule, FormsModule, FormBuilder } from '@angular/forms';
import { MatExpansionModule } from '@angular/material/expansion';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { RouterTestingModule } from '@angular/router/testing';
import { of, throwError } from 'rxjs';
import { CampaignsComponent } from './campaigns.component';
import { CampaignService, Campaign } from './../../services/campaign.service';
import { NeedService } from 'src/app/services/need.service';
import { OrganizationService } from 'src/app/services/organization.service';
import { AuthService } from 'src/app/services/auth.service';
import { NgToastService } from 'ng-angular-popup';
import { ChangeDetectorRef, NO_ERRORS_SCHEMA, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';

describe('CampaignsComponent', () => {
  let component: CampaignsComponent;
  let fixture: ComponentFixture<CampaignsComponent>;

  let mockCampaignService: jasmine.SpyObj<CampaignService>;
  let mockNeedService: jasmine.SpyObj<NeedService>;
  let mockOrganizationService: jasmine.SpyObj<OrganizationService>;
  let mockAuthService: jasmine.SpyObj<AuthService>;
  let mockToastService: jasmine.SpyObj<NgToastService>;
  let mockChangeDetectorRef: jasmine.SpyObj<ChangeDetectorRef>;

  beforeEach(async () => {
    mockCampaignService = jasmine.createSpyObj('CampaignService', [
      'getAllCampaigns',
      'createCampaign',
      'deleteCampaign',
      'updateCampaign'
    ]);
    mockNeedService = jasmine.createSpyObj('NeedService', ['getAllNeeds']);
    mockOrganizationService = jasmine.createSpyObj('OrganizationService', [
      'getCuitFromStore',
      'getOrganizationByCuit'
    ]);
    mockAuthService = jasmine.createSpyObj('AuthService', [
      'isLoggedIn',
      'getCuitFromToken'
    ]);
    mockToastService = jasmine.createSpyObj('NgToastService', ['success', 'error']);
    mockChangeDetectorRef = jasmine.createSpyObj('ChangeDetectorRef', ['detectChanges']);

    const mockCampaign: Campaign = {
      id: 1,
      title: 'Campaña 1',
      startDate: '2023-01-01',
      endDate: '2023-12-31',
      organizacionId: 1,
      isActive: true,
      imageUrl: 'http://example.com/image.jpg',
      descripcionBreve: 'Descripción breve',
      needs: [],
      descripcionCompleta: ''
    };

    mockCampaignService.getAllCampaigns.and.returnValue(of([mockCampaign]));
    mockCampaignService.createCampaign.and.returnValue(of(mockCampaign));
    mockCampaignService.deleteCampaign.and.returnValue(of(void 0));
    mockCampaignService.updateCampaign.and.returnValue(of(mockCampaign));
    mockNeedService.getAllNeeds.and.returnValue(of([{ id: 1, nombre: 'Necesidad 1' }]));
    mockOrganizationService.getCuitFromStore.and.returnValue(of('123456789'));
    mockOrganizationService.getOrganizationByCuit.and.returnValue(of({ id: 1 }));
    mockAuthService.isLoggedIn.and.returnValue(true);
    mockAuthService.getCuitFromToken.and.returnValue('123456789');

    await TestBed.configureTestingModule({
      declarations: [CampaignsComponent],
      imports: [
        ReactiveFormsModule,
        FormsModule,
        RouterTestingModule,
        MatExpansionModule,
        BrowserAnimationsModule
      ],
      providers: [
        FormBuilder,
        { provide: CampaignService, useValue: mockCampaignService },
        { provide: NeedService, useValue: mockNeedService },
        { provide: OrganizationService, useValue: mockOrganizationService },
        { provide: AuthService, useValue: mockAuthService },
        { provide: NgToastService, useValue: mockToastService },
        { provide: ChangeDetectorRef, useValue: mockChangeDetectorRef }
      ],
      schemas: [NO_ERRORS_SCHEMA, CUSTOM_ELEMENTS_SCHEMA]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CampaignsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('debería crear el componente', () => {
    expect(component).toBeTruthy();
  });

  it('debería inicializar correctamente el formulario y cargar las necesidades', () => {
    expect(component.campaignForm).toBeTruthy();
    expect(component.needs.length).toBe(1);
    expect(component.needs[0].nombre).toBe('Necesidad 1');
  });

  it('debería cargar las campañas y necesidades al inicializar', () => {
    component.loadCampaignsAndNeeds('123456789');
    expect(mockOrganizationService.getOrganizationByCuit).toHaveBeenCalledWith('123456789');
    expect(mockCampaignService.getAllCampaigns).toHaveBeenCalledWith('1');
    expect(component.campaigns.length).toBe(1);
  });

  it('debería añadir una nueva campaña', () => {
    const campaignFormValue = {
      title: 'Nueva Campaña',
      startDate: '2023-01-01',
      endDate: '2023-12-31',
      needs: [],
      imageUrl: 'http://example.com/image.jpg',
      descripcionBreve: 'Descripción breve',
      descripcionCompleta: 'Descripción completa'
    };

    component.campaignForm.setValue(campaignFormValue);
    mockCampaignService.createCampaign.and.returnValue(of({ ...campaignFormValue, id: 2 } as unknown as Campaign));

    component.addCampaign();

    expect(mockCampaignService.createCampaign).toHaveBeenCalled();
    expect(component.campaigns.length).toBe(2);
  });

  it('debería manejar errores al añadir una nueva campaña', () => {
    const campaignFormValue = {
      title: 'Nueva Campaña',
      startDate: '2023-01-01',
      endDate: '2023-12-31',
      needs: [],
      imageUrl: 'http://example.com/image.jpg',
      descripcionBreve: 'Descripción breve',
      descripcionCompleta: 'Descripción completa'
    };

    component.campaignForm.setValue(campaignFormValue);
    mockCampaignService.createCampaign.and.returnValue(throwError('Error'));

    component.addCampaign();

    expect(mockCampaignService.createCampaign).toHaveBeenCalled();
    expect(mockToastService.error).toHaveBeenCalledWith({
      detail: 'Error',
      summary: 'Error al añadir la campaña',
      duration: 5000,
      position: 'bottomRight'
    });
  });

  it('debería eliminar una campaña', () => {
    const campaignId = 1;

    component.deleteCampaign(campaignId);

    expect(mockCampaignService.deleteCampaign).toHaveBeenCalledWith(campaignId);
    expect(mockToastService.success).toHaveBeenCalledWith({
      detail: 'Éxito',
      summary: 'Campaña eliminada correctamente',
      duration: 5000,
      position: 'bottomRight'
    });
    expect(component.campaigns.length).toBe(0);
  });

  it('debería manejar errores al eliminar una campaña', () => {
    const campaignId = 1;

    mockCampaignService.deleteCampaign.and.returnValue(throwError('Error'));

    component.deleteCampaign(campaignId);

    expect(mockCampaignService.deleteCampaign).toHaveBeenCalledWith(campaignId);
    expect(mockToastService.error).toHaveBeenCalledWith({
      detail: 'Error',
      summary: 'Error al eliminar la campaña',
      duration: 5000,
      position: 'bottomRight'
    });
  });

  it('debería actualizar el estado de una campaña', () => {
    const campaignId = 1;

    component.updateCampaignStatus(campaignId, false);

    expect(mockCampaignService.updateCampaign).toHaveBeenCalled();
    expect(mockToastService.success).toHaveBeenCalledWith({
      detail: 'Éxito',
      summary: 'Campaña desactivada correctamente',
      duration: 5000,
      position: 'bottomRight'
    });
  });

  it('debería manejar errores al actualizar el estado de una campaña', () => {
    const campaignId = 1;

    mockCampaignService.updateCampaign.and.returnValue(throwError('Error'));

    component.updateCampaignStatus(campaignId, false);

    expect(mockCampaignService.updateCampaign).toHaveBeenCalled();
    expect(mockToastService.error).toHaveBeenCalledWith({
      detail: 'Error',
      summary: 'Error al desactivar la campaña',
      duration: 5000,
      position: 'bottomRight'
    });
  });
});
