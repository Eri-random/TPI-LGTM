import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { FormsModule } from '@angular/forms';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { Router } from '@angular/router';
import { of } from 'rxjs';
import { NgToastService } from 'ng-angular-popup';

import { MapOrganizationsComponent } from './map-organizations.component';
import { MapService } from 'src/app/services/map.service';
import { GoogleMapsLoaderService } from 'src/app/services/google-maps-loader.service';
import { HeadquartersService } from 'src/app/services/headquarters.service';
import { Provinces } from 'src/app/interfaces/provinces.interface';

describe('MapOrganizationsComponent', () => {
  let component: MapOrganizationsComponent;
  let fixture: ComponentFixture<MapOrganizationsComponent>;
  let mapServiceMock: any;
  let googleMapsLoaderMock: any;
  let toastServiceMock: any;
  let headquartersServiceMock: any;
  let routerMock: any;

  beforeEach(async () => {
    mapServiceMock = {
      getProvinces: jasmine.createSpy('getProvinces').and.returnValue(of({ provincias: [] })),
      getOrganizations: jasmine.createSpy('getOrganizations').and.returnValue(of([])),
      getPolygonosProvinces: jasmine.createSpy('getPolygonosProvinces').and.returnValue(Promise.resolve({ features: [] })),
      getOrganizationHeadquarters: jasmine.createSpy('getOrganizationHeadquarters').and.returnValue(Promise.resolve([])) // Agregado aquí
    };

    googleMapsLoaderMock = {
      load: jasmine.createSpy('load').and.returnValue(Promise.resolve())
    };

    toastServiceMock = {
      error: jasmine.createSpy('error')
    };

    headquartersServiceMock = {
      getDataDirection: jasmine.createSpy('getDataDirection').and.returnValue(of(null))
    };

    routerMock = {
      navigate: jasmine.createSpy('navigate')
    };

    await TestBed.configureTestingModule({
      declarations: [MapOrganizationsComponent],
      imports: [HttpClientTestingModule, FormsModule],
      providers: [
        { provide: MapService, useValue: mapServiceMock },
        { provide: GoogleMapsLoaderService, useValue: googleMapsLoaderMock },
        { provide: NgToastService, useValue: toastServiceMock },
        { provide: HeadquartersService, useValue: headquartersServiceMock },
        { provide: Router, useValue: routerMock }
      ]
    }).compileComponents();
  });

  beforeEach(() => {
    (window as any).google = {
      maps: {
        Map: function (element: HTMLElement, options?: google.maps.MapOptions) {
          return {
            setCenter: jasmine.createSpy('setCenter'),
            setZoom: jasmine.createSpy('setZoom'),
            panTo: jasmine.createSpy('panTo'),
            addListener: jasmine.createSpy('addListener')
          };
        },
        Marker: function (options: google.maps.MarkerOptions) {
          return {
            setMap: jasmine.createSpy('setMap'),
            addListener: jasmine.createSpy('addListener')
          };
        },
        InfoWindow: function (options?: google.maps.InfoWindowOptions) {
          return {
            open: jasmine.createSpy('open'),
            close: jasmine.createSpy('close'),
            addListener: jasmine.createSpy('addListener')
          };
        },
        Polygon: function (options: google.maps.PolygonOptions) {
          return {
            setMap: jasmine.createSpy('setMap')
          };
        },
        event: {
          addListenerOnce: jasmine.createSpy('addListenerOnce')
        }
      }
    };

    fixture = TestBed.createComponent(MapOrganizationsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('debería crear el componente', () => {
    expect(component).toBeTruthy();
  });

  it('debería inicializarse correctamente', fakeAsync(() => {
    component.ngOnInit();
    expect(mapServiceMock.getProvinces).toHaveBeenCalled();
    expect(headquartersServiceMock.getDataDirection).toHaveBeenCalled();
  }));

  it('debería cargar provincias', fakeAsync(() => {
    const provinciasMock: Provinces = {
      cantidad: 2,
      inicio: 0,
      parametros: {},
      provincias: [
        { centroide: { lat: -34.6037, lon: -58.3816 }, id: '1', nombre: 'Buenos Aires' },
        { centroide: { lat: -31.4201, lon: -64.1888 }, id: '2', nombre: 'Córdoba' }
      ],
      total: 2
    };
    mapServiceMock.getProvinces.and.returnValue(of(provinciasMock));

    component.getProvinces();
    expect(component.provinces).toEqual(provinciasMock.provincias);
  }));

  it('debería cargar organizaciones', fakeAsync(() => {
    const organizationsMock = [{ id: 1, nombre: 'Org 1', latitud: -34.6037, longitud: -58.3816 }];
    mapServiceMock.getOrganizations.and.returnValue(of(organizationsMock));

    component.getMarker();
    expect(component.organizations).toEqual(organizationsMock);
    expect(googleMapsLoaderMock.load).toHaveBeenCalled();
  }));

  it('debería aplicar filtros correctamente', () => {
    component.organizations = [
      { nombre: 'Org 1', provincia: 'Buenos Aires' },
      { nombre: 'Org 2', provincia: 'Córdoba' }
    ];
    component.provinceSelected = { nombre: 'Buenos Aires' };

    component.applyFilters();
    expect(component.filteredOrganizations).toEqual([{ nombre: 'Org 1', provincia: 'Buenos Aires' }]);
  });

  it('debería verificar la sede más cercana', fakeAsync(() => {
    const dataDirectionMock = {
      latitud: -34.6037,
      longitud: -58.3816,
      nombre: 'Sede 1',
      direccion: 'Calle Falsa 123',
      localidad: 'Buenos Aires',
      provincia: 'Buenos Aires',
      telefono: '123456789'
    };
    headquartersServiceMock.getDataDirection.and.returnValue(of(dataDirectionMock));

    component.checkCloserHeadquarters();
    expect(component.dataDirection).toEqual(dataDirectionMock);
  }));

  it('debería seleccionar la organización correcta', () => {
    component.organizations = [
      { id: 1, nombre: 'Org 1', provincia: 'Buenos Aires', localidad: 'Buenos Aires', latitud: -34.6037, longitud: -58.3816, direccion: 'Calle Falsa 123', telefono: '123456789' },
      { id: 2, nombre: 'Org 2', provincia: 'Córdoba', localidad: 'Córdoba', latitud: -31.4201, longitud: -64.1888, direccion: 'Calle Falsa 456', telefono: '987654321' }
    ];
    component.provinceSelected = { nombre: 'Buenos Aires' };
    component.selectedOrganization = { id: 1, nombre: 'Org 1', provincia: 'Buenos Aires', localidad: 'Buenos Aires', latitud: -34.6037, longitud: -58.3816, direccion: 'Calle Falsa 123', telefono: '123456789' };

    component.selectOrganization();
    expect(component.filteredOrganizations).toEqual([{ id: 1, nombre: 'Org 1', provincia: 'Buenos Aires', localidad: 'Buenos Aires', latitud: -34.6037, longitud: -58.3816, direccion: 'Calle Falsa 123', telefono: '123456789' }]);
  });

  it('debería mostrar error si la organización seleccionada no está en la provincia seleccionada', () => {
    component.provinceSelected = { nombre: 'Córdoba' };
    component.selectedOrganization = { id: 1, nombre: 'Org 1', provincia: 'Buenos Aires', localidad: 'Buenos Aires', latitud: -34.6037, longitud: -58.3816, direccion: 'Calle Falsa 123', telefono: '123456789' };

    component.selectOrganization();
    expect(toastServiceMock.error).toHaveBeenCalledWith({
      detail: 'Error',
      summary: 'La organización seleccionada no se encuentra en esta provincia.',
      position: 'topRight',
      duration: 5000,
    });
  });
});
