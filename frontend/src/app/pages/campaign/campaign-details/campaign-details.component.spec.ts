import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ActivatedRoute } from '@angular/router';
import { CampaignDetailsComponent } from './campaign-details.component';
import { CampaignService, Campaign } from 'src/app/services/campaign.service';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { RouterTestingModule } from '@angular/router/testing';
import { of, throwError } from 'rxjs';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { DialogDonateComponent } from '../../info-organization/dialog-donate/dialog-donate.component';

describe('CampaignDetailsComponent', () => {
  let component: CampaignDetailsComponent;
  let fixture: ComponentFixture<CampaignDetailsComponent>;
  let mockCampaignService: jasmine.SpyObj<CampaignService>;
  let mockDialog: jasmine.SpyObj<MatDialog>;
  let routeMock: any;

  beforeEach(async () => {
    mockCampaignService = jasmine.createSpyObj('CampaignService', ['getIdCampaign']);
    mockDialog = jasmine.createSpyObj('MatDialog', ['open']);

    routeMock = {
      snapshot: {
        params: { id: '1' }
      }
    };

    await TestBed.configureTestingModule({
      declarations: [CampaignDetailsComponent],
      imports: [
        MatDialogModule,
        RouterTestingModule
      ],
      providers: [
        { provide: CampaignService, useValue: mockCampaignService },
        { provide: MatDialog, useValue: mockDialog },
        { provide: ActivatedRoute, useValue: routeMock }
      ],
      schemas: [CUSTOM_ELEMENTS_SCHEMA]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CampaignDetailsComponent);
    component = fixture.componentInstance;
    component.campaignId = 1; // Asegurar que campaignId está definido

    const campaignData: Campaign = {
      id: 1,
      title: 'Campaña 1',
      startDate: '2023-01-01',
      endDate: '2023-12-31',
      organizacionId: 123,
      needs: [{ id: 1, nombre: 'Comida' }, { id: 2, nombre: 'Ropa' }],
      isActive: true,
      descripcionBreve: 'Descripción breve de la campaña',
      descripcionCompleta: 'Descripción completa de la campaña',
      imageUrl: 'https://example.com/image.jpg' // Opcional
    };

    mockCampaignService.getIdCampaign.and.returnValue(of(campaignData));

    fixture.detectChanges();
  });

  it('debería crear el componente', () => {
    expect(component).toBeTruthy();
  });

  it('debería obtener los detalles de la campaña al inicializar', () => {
    const campaignData: Campaign = {
      id: 1,
      title: 'Campaña 1',
      startDate: '2023-01-01',
      endDate: '2023-12-31',
      organizacionId: 123,
      needs: [{ id: 1, nombre: 'Comida' }, { id: 2, nombre: 'Ropa' }],
      isActive: true,
      descripcionBreve: 'Descripción breve de la campaña',
      descripcionCompleta: 'Descripción completa de la campaña',
      imageUrl: 'https://example.com/image.jpg'
    };

    mockCampaignService.getIdCampaign.and.returnValue(of(campaignData));

    component.ngOnInit();
    expect(component.campaign).toEqual(campaignData);
    expect(component.organizationId).toEqual(123);
  });

  it('debería manejar errores al obtener los detalles de la campaña', () => {
    spyOn(console, 'error');
    mockCampaignService.getIdCampaign.and.returnValue(throwError('error'));

    component.campaignId = 1; // Asegurar que campaignId está definido
    component.getCampaignDetails();

    expect(mockCampaignService.getIdCampaign).toHaveBeenCalledWith(1);
    expect(console.error).toHaveBeenCalledWith('error');
  });

  it('debería navegar de vuelta a la página de información de la organización', () => {
    const routerSpy = spyOn(component['router'], 'navigate');
    component.organizationId = 123;

    component.goBack();

    expect(routerSpy).toHaveBeenCalledWith(['/info-organizacion/123']);
  });

  it('debería abrir el diálogo para donar', () => {
    component.organizationId = 123;
    component.openDialog();

    expect(mockDialog.open).toHaveBeenCalledWith(DialogDonateComponent, {
      width: 'auto',
      height: '75%',
      data: { organizacionId: 123 }
    });
  });
});
