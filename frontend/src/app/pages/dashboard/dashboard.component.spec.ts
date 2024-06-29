import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { of, Subject, throwError } from 'rxjs';
import { DashboardComponent, UserData } from './dashboard.component';
import { OrganizationService } from 'src/app/services/organization.service';
import { AuthService } from 'src/app/services/auth.service';
import { DonationsService } from 'src/app/services/donations.service';
import { WebsocketService } from 'src/app/services/websocket.service';
import { NgToastService } from 'ng-angular-popup';
import { ChangeDetectorRef } from '@angular/core';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

describe('DashboardComponent', () => {
  let component: DashboardComponent;
  let fixture: ComponentFixture<DashboardComponent>;
  let organizationServiceMock: any;
  let authServiceMock: any;
  let donationsServiceMock: any;
  let webSocketServiceMock: any;
  let toastServiceMock: any;
  let cdrMock: any;

  beforeEach(async () => {
    organizationServiceMock = jasmine.createSpyObj('OrganizationService', [
      'getOrgNameFromStore',
      'getCuitFromStore',
      'getOrganizationByCuit',
    ]);
    authServiceMock = jasmine.createSpyObj('AuthService', [
      'getOrgNameFromToken',
      'getCuitFromToken',
    ]);
    donationsServiceMock = jasmine.createSpyObj('DonationsService', ['getDonationsByOrganizationId']);
    webSocketServiceMock = {
      messages: new Subject(),
    };
    toastServiceMock = jasmine.createSpyObj('NgToastService', ['success', 'error']);
    cdrMock = jasmine.createSpyObj('ChangeDetectorRef', ['detectChanges']);

    await TestBed.configureTestingModule({
      declarations: [DashboardComponent],
      imports: [
        MatPaginatorModule,
        MatSortModule,
        BrowserAnimationsModule
      ],
      providers: [
        { provide: OrganizationService, useValue: organizationServiceMock },
        { provide: AuthService, useValue: authServiceMock },
        { provide: DonationsService, useValue: donationsServiceMock },
        { provide: WebsocketService, useValue: webSocketServiceMock },
        { provide: NgToastService, useValue: toastServiceMock },
        { provide: ChangeDetectorRef, useValue: cdrMock },
        { provide: MatDialog, useValue: {} },
      ],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DashboardComponent);
    component = fixture.componentInstance;
    component.paginator = jasmine.createSpyObj('MatPaginator', ['firstPage']);
    component.sort = jasmine.createSpyObj('MatSort', ['sort']);

    organizationServiceMock.getOrgNameFromStore.and.returnValue(of('Mi Organización'));
    organizationServiceMock.getCuitFromStore.and.returnValue(of('123456789'));
    authServiceMock.getOrgNameFromToken.and.returnValue('Mi Organización');
    authServiceMock.getCuitFromToken.and.returnValue('123456789');

    organizationServiceMock.getOrganizationByCuit.and.returnValue(of({ id: 1 }));

    fixture.detectChanges();
  });

  it('debería crear el componente', () => {
    expect(component).toBeTruthy();
  });

  it('debería inicializar los datos de la organización y cargar las donaciones en ngOnInit', () => {
    const donationsResponse = of([
      { usuario: { nombre: 'Juan', telefono: '12345678', email: 'juan@ejemplo.com' }, producto: 'Producto A', cantidad: 10 },
    ]);

    donationsServiceMock.getDonationsByOrganizationId.and.returnValue(donationsResponse);

    component.ngOnInit();
    fixture.detectChanges();

    expect(component.orgName).toBe('Mi Organización');
    expect(component.cuit).toBe('123456789');
    expect(component.existDonations).toBe(true);
    expect(component.dataSource.data.length).toBe(1);
    expect(component.totalDonations).toBe(10);
    expect(component.totalDonationsCount).toBe(1);
    expect(component.averageDonations).toBe(10);
    expect(component.productMostDonate).toEqual({ product: 'producto a', amount: 10 });
  });

  // it('debería manejar nuevas donaciones a través de WebSocket', () => {
  //   component.dataSource = new MatTableDataSource<UserData>([]);
  //   const newDonation = {
  //     newDonation: { Producto: 'Producto B', Cantidad: 5 },
  //     user: { Nombre: 'Maria', Telefono: '87654321', Email: 'maria@ejemplo.com' }, // Proporciona datos válidos para el usuario
  //   };
    

  //   component.handleNewDonation(newDonation);
    
  //   expect(component.totalDonations).toBe(5);
  //   expect(component.totalDonationsCount).toBe(1);
  //   expect(component.averageDonations).toBe(5);
  //   expect(component.dataSource.data.length).toBe(1);
  //   expect(component.dataSource.data[0].name).toBe('Maria'); // Verifica que el nombre sea 'Maria' correctamente
  //   expect(component.dataSource.data[0].producto).toBe('Producto B');
  //   expect(toastServiceMock.success).toHaveBeenCalledWith({
  //     detail: 'EXITO',
  //     summary: 'Nueva donación recibida',
  //     duration: 5000,
  //     position: 'topRight',
  //   });
  // });

  it('debería aplicar filtro a la tabla', () => {
    const event = { target: { value: 'juan' } } as any;
    component.dataSource.data = [
      { id:1, name: 'Juan', telefono: '12345678', email: 'juan@ejemplo.com', producto: 'Producto A', cantidad: 10, estado: 'Pendiente' },
      { id:2, name: 'Maria', telefono: '87654321', email: 'maria@ejemplo.com', producto: 'Producto B', cantidad: 5, estado: 'Pendiente' },
    ];

    component.applyFilter(event);

    expect(component.dataSource.filter).toBe('juan');
    expect(component.dataSource.filteredData.length).toBe(1);
    expect(component.dataSource.filteredData[0].name).toBe('Juan');
  });

  it('should handle error while loading donations', () => {
    const error = 'Error loading donations';
    
    organizationServiceMock.getOrganizationByCuit.and.returnValue(of({ id: 1 }));
    donationsServiceMock.getDonationsByOrganizationId.and.returnValue(throwError(error));

    spyOn(console, 'error');

    component.loadDonations();

    expect(console.error).toHaveBeenCalledWith('Error:', error);
  });
});
