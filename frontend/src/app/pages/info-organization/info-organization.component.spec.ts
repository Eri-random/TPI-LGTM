// import { ComponentFixture, TestBed } from '@angular/core/testing';
// import { InfoOrganizationComponent } from './info-organization.component';
// import { OrganizationService } from 'src/app/services/organization.service';
// import { CampaignService, Campaign } from 'src/app/services/campaign.service';
// import { MatDialogModule } from '@angular/material/dialog';
// import { RouterTestingModule } from '@angular/router/testing';
// import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
// import { of, throwError } from 'rxjs';
// import { SafeHtml, DomSanitizer } from '@angular/platform-browser';
// import { ActivatedRoute, Router } from '@angular/router';
// import { CUSTOM_ELEMENTS_SCHEMA, ElementRef } from '@angular/core';

// describe('InfoOrganizationComponent', () => {
//   let component: InfoOrganizationComponent;
//   let fixture: ComponentFixture<InfoOrganizationComponent>;
//   let organizationServiceMock: any;
//   let campaignServiceMock: any;
//   let sanitizerMock: any;
//   let routeMock: any;
//   let routerMock: any;

//   beforeEach(async () => {
//     organizationServiceMock = jasmine.createSpyObj('OrganizationService', ['getOrganizationById']);
//     campaignServiceMock = jasmine.createSpyObj('CampaignService', ['getAllCampaigns']);
//     sanitizerMock = jasmine.createSpyObj('DomSanitizer', ['bypassSecurityTrustHtml']);
//     routeMock = { params: of({ id: '123' }) };
//     routerMock = jasmine.createSpyObj('Router', ['navigate']);

//     await TestBed.configureTestingModule({
//       declarations: [InfoOrganizationComponent],
//       imports: [
//         MatDialogModule,
//         RouterTestingModule,
//         BrowserAnimationsModule
//       ],
//       providers: [
//         { provide: OrganizationService, useValue: organizationServiceMock },
//         { provide: CampaignService, useValue: campaignServiceMock },
//         { provide: DomSanitizer, useValue: sanitizerMock },
//         { provide: ActivatedRoute, useValue: routeMock },
//         { provide: Router, useValue: routerMock }
//       ],
//       schemas: [CUSTOM_ELEMENTS_SCHEMA]
//     }).compileComponents();
//   });

//   beforeEach(() => {
//     fixture = TestBed.createComponent(InfoOrganizationComponent);
//     component = fixture.componentInstance;
//     fixture.detectChanges();
//   });

//   it('debería crear el componente', () => {
//     expect(component).toBeTruthy();
//   });

//   it('debería cargar la organización y campañas en ngOnInit', () => {
//     const organizationData = { id: '123', infoOrganizacion: { descripcionCompleta: '<p>Descripción</p>' } };
//     const campaignsData: Campaign[] = [
//       {
//         id: 1, title: 'Campaign 1', startDate: '2023-01-01', endDate: '2023-12-31', isActive: true, descripcionCompleta: 'Description 1',
//         organizacionId: 0,
//         needs: [],
//         descripcionBreve: ''
//       },
//       {
//         id: 2, title: 'Campaign 2', startDate: '2023-01-01', endDate: '2023-12-31', isActive: false, descripcionCompleta: 'Description 2',
//         organizacionId: 0,
//         needs: [],
//         descripcionBreve: ''
//       }
//     ];

//     organizationServiceMock.getOrganizationById.and.returnValue(of(organizationData));
//     campaignServiceMock.getAllCampaigns.and.returnValue(of(campaignsData));
//     sanitizerMock.bypassSecurityTrustHtml.and.returnValue('<p>Descripción</p>' as SafeHtml);

//     component.ngOnInit();
//     fixture.detectChanges();

//     expect(component.organization).toEqual(organizationData);
//     expect(component.safeContent).toEqual('<p>Descripción</p>' as SafeHtml);
//     expect(component.campaigns).toEqual([campaignsData[0]]);
//     expect(sanitizerMock.bypassSecurityTrustHtml).toHaveBeenCalledWith('<p>Descripción</p>');
//   });

//   it('debería manejar errores al cargar la organización', () => {
//     spyOn(console, 'error');
//     organizationServiceMock.getOrganizationById.and.returnValue(throwError('error'));

//     component.ngOnInit();
//     fixture.detectChanges();

//     expect(console.error).toHaveBeenCalledWith('error');
//   });

//   it('debería inicializar el swiper después de la vista', () => {
//     const swiperMock = {
//       nativeElement: {
//         initialize: jasmine.createSpy('initialize'),
//         slidesPerView: 1,
//         spaceBetween: 10,
//         pagination: { clickable: true },
//         breakpoints: {
//           640: { slidesPerView: 2, spaceBetween: 20 },
//           768: { slidesPerView: 3, spaceBetween: 40 },
//           1024: { slidesPerView: 3, spaceBetween: 50 },
//         }
//       }
//     };

//     component.swiperEl = swiperMock as any;
//     component.ngAfterViewInit();

//     expect(swiperMock.nativeElement.initialize).toHaveBeenCalled();
//   });
// });
