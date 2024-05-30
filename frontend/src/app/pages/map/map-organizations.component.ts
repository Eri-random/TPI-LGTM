/// <reference types="@types/googlemaps" />
import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { MapService } from 'src/app/services/map.service';
import { GoogleMapsLoaderService } from 'src/app/services/google-maps-loader.service';
import { NgToastService } from 'ng-angular-popup';
import { SedeService } from 'src/app/services/sede.service';

@Component({
  selector: 'app-mapa-organizaciones',
  templateUrl: './map-organizations.component.html',
  styleUrls: ['./map-organizations.component.css'],
})
export class MapOrganizationsComponent implements OnInit {
  organizations: any[] = [];
  provinces: any[] = [];
  provinceSelected: any = 'todas';
  selectedOrganization: any = 'todas';
  filteredOrganizations: any[] = [];
  dataDirection: any = null;

  constructor(
    private http: HttpClient,
    private mapService: MapService,
    private googleMapsLoader: GoogleMapsLoaderService,
    private toast: NgToastService,
    private sedeService: SedeService
  ) {}

  ngOnInit(): void {
    this.getProvinces();
    this.checkCloserHeadquarters();
  }

  getProvinces(): void {
    this.mapService.getPronvincias().subscribe(
      (data) => {
        this.provinces = data.provincias;
        this.provinces.sort((a, b) => a.nombre.localeCompare(b.nombre));
        this.getMarker();
      },
      (error) => {
        console.error('Error al cargar los datos de provincias:', error);
      }
    );
  }

  getMarker(): void {
    this.mapService.getOrganizations().subscribe(
      (data) => {
        this.organizations = data;
        this.googleMapsLoader.load().then(() => {
          this.applyFilters();
        });
      },
      (error) => {
        console.error('Error al cargar los datos de ubicaciones:', error);
      }
    );
  }

  applyFilters(): void {
    if (this.dataDirection) {
      let location = this.dataDirection.localidad.toLowerCase();
      if (
        location === 'caba' ||
        location === 'ciudad autónoma de buenos aires'
      ) {
        this.provinceSelected = this.provinces.find(
          (provinces) => provinces.nombre === 'Ciudad Autónoma de Buenos Aires'
        );
      } else {
        this.provinceSelected = this.provinces.find(
          (provinces) => provinces.nombre === this.dataDirection.provincia
        );
      }
      this.selectedOrganization = this.organizations.find((org: any) => {
        if (
          this.dataDirection.nombreOrganizacion !== null &&
          this.dataDirection.nombreOrganizacion !== undefined
        ) {
          return org.nombre === this.dataDirection.nombreOrganizacion;
        } else {
          return org.nombre === this.dataDirection.nombre;
        }
      });
      this.filterOrganizations();
      this.loadMap();
    } else {
      this.filterOrganizations();
      this.loadMap();
    }
  }

  loadMap(): void {
    const map = new google.maps.Map(
      document.getElementById('map') as HTMLElement,
      {
        center: { lat: -38.4161, lng: -63.6167 },
        zoom: 5,
        minZoom: 5,
        maxZoom: 16,
      }
    );

    this.filteredOrganizations.forEach((org) => {
      const marker = new google.maps.Marker({
        position: { lat: org.latitud, lng: org.longitud },
        map: map,
        title: org.nombre,
        icon: 'https://maps.gstatic.com/mapfiles/ms2/micons/red-pushpin.png',
      });

      const infoWindow = new google.maps.InfoWindow({
        content: `
        <div class="info-window">
          <h3>${org.nombre}</h3>
          <p>Dirección: ${org.direccion}</p>
          <p>Localidad: ${org.localidad}</p>
          <p>Provincia: ${org.provincia}</p>
          <p>Teléfono: ${org.telefono}</p>
        </div>
        `,
      });

      marker.addListener('click', () => {
        infoWindow.open(map, marker);
      });
    });

    if (this.dataDirection) {
      this.highlightSedeMasCercana(map);
    }
  }

  filterOrganizations(): void {
    if (this.provinceSelected === 'todas') {
      this.filteredOrganizations = this.organizations;
    } else {
      this.filteredOrganizations = this.organizations.filter(
        (org) => org.provincia === this.provinceSelected.nombre
      );
    }

    if (this.selectedOrganization !== 'todas') {
      this.filteredOrganizations = this.filteredOrganizations.filter(
        (org) => org.nombre === this.selectedOrganization.nombreOrganizacion
      );
    }
  }

  checkCloserHeadquarters(): void {
    this.sedeService.getDataDirection().subscribe((data: any) => {
      if (data) {
        this.dataDirection = data;
      }
    });
  }

  highlightSedeMasCercana(map: google.maps.Map): void {
    const {
      latitud,
      longitud,
      nombre,
      direccion,
      localidad,
      provincia,
      telefono,
    } = this.dataDirection;

    console.log('Sede más cercana:', this.dataDirection);
    const marker = new google.maps.Marker({
      position: { lat: latitud, lng: longitud },
      map: map,
      title: nombre || 'Sede más cercana',
      icon: 'https://maps.gstatic.com/mapfiles/ms2/micons/red-pushpin.png', // Ícono especial para la sede más cercana
    });

    const infoWindow = new google.maps.InfoWindow({
      content: `
      <div class="info-window">
          <h3>${nombre || 'Sede más cercana'}</h3>
          <p>Dirección: ${direccion || ''}</p>
          <p>Localidad: ${localidad || ''}</p>
          <p>Provincia: ${provincia || ''}</p>
          <p>Teléfono: ${telefono || ''}</p>
      </div>
      `,
    });

    marker.addListener('click', () => {
      infoWindow.open(map, marker);
    });

    // Abrir el popup al cargar el mapa
    infoWindow.open(map, marker);

    infoWindow.addListener('closeclick', () => {
      console.log('Cerrando infoWindow');
      this.provinceSelected = 'todas';
      this.selectedOrganization = 'todas';
      this.dataDirection = null;
      this.filterOrganizations();
      this.loadMap();
    });

    map.setCenter({ lat: latitud, lng: longitud });
    map.setZoom(10);
  }

  onProvinceChange(): void {
    this.selectedOrganization = 'todas';
    this.filterOrganizations();
    this.updateMap();
  }

  onOrganizationChange(): void {
    if (this.selectedOrganization === 'todas') {
      this.filterOrganizations();
      this.updateMap();
    } else {
      this.selectOrganization();
    }
  }

  updateMap(): void {
    const provincia = this.provinceSelected;
    const map = new google.maps.Map(
      document.getElementById('map') as HTMLElement,
      {
        center:
          provincia === 'todas'
            ? { lat: -38.4161, lng: -63.6167 }
            : {
                lat: provincia.centroide.lat,
                lng: provincia.centroide.lon,
              },
        zoom: provincia === 'todas' ? 5 : 7,
        minZoom: 5,
        maxZoom: 16,
      }
    );

    if (provincia !== 'todas') {
      this.mapService.getPoligonosProvincias().then((geojson) => {
        geojson.features.forEach((feature: any) => {
          if (feature.properties.nombre === provincia.nombre) {
            const polygon = new google.maps.Polygon({
              paths: feature.geometry.coordinates[0].map((coord: any) => {
                return { lat: coord[1], lng: coord[0] };
              }),
              strokeColor: '#FF0000',
              strokeOpacity: 0.8,
              strokeWeight: 2,
              fillColor: 'rgba(255, 0, 0, 0.3)',
              fillOpacity: 0.35,
            });
            polygon.setMap(map);
          }
        });
      });
    }

    this.filteredOrganizations.forEach((org) => {
      const marker = new google.maps.Marker({
        position: { lat: org.latitud, lng: org.longitud },
        map: map,
        title: org.nombre,
        icon: 'https://maps.gstatic.com/mapfiles/ms2/micons/red-pushpin.png',
      });

      const infoWindow = new google.maps.InfoWindow({
        content: `
          <div class="info-window">
            <h3>${org.nombre}</h3>
            <p>Dirección: ${org.direccion}</p>
            <p>Localidad: ${org.localidad}</p>
            <p>Provincia: ${org.provincia}</p>
            <p>Teléfono: ${org.telefono}</p>
          </div>
          `,
      });

      marker.addListener('click', () => {
        infoWindow.open(map, marker);
      });
    });

    if (this.dataDirection) {
      this.highlightSedeMasCercana(map);
    }
  }

  selectOrganization(): void {
    const org = this.selectedOrganization;

    if (
      this.provinceSelected !== 'todas' &&
      org.provincia !== this.provinceSelected.nombre
    ) {
      this.toast.error({
        detail: 'Error',
        summary:
          'La organización seleccionada no se encuentra en la provincia seleccionada.',
        position: 'topRight',
        duration: 5000,
      });
      return;
    }

    this.filteredOrganizations = [org];

    const map = new google.maps.Map(
      document.getElementById('map') as HTMLElement,
      {
        center: {
          lat: org.latitud,
          lng: org.longitud,
        },
        zoom: 7,
        minZoom: 5,
        maxZoom: 16,
      }
    );

    if (this.provinceSelected !== 'todas') {
      this.mapService.getPoligonosProvincias().then((geojson) => {
        geojson.features.forEach((feature: any) => {
          if (feature.properties.nombre === this.provinceSelected.nombre) {
            const polygon = new google.maps.Polygon({
              paths: feature.geometry.coordinates[0].map((coord: any) => {
                return { lat: coord[1], lng: coord[0] };
              }),
              strokeColor: '#FF0000',
              strokeOpacity: 0.8,
              strokeWeight: 2,
              fillColor: 'rgba(255, 0, 0, 0.3)',
              fillOpacity: 0.35,
            });
            polygon.setMap(map);
          }
        });
      });
    }

    const marker = new google.maps.Marker({
      position: { lat: org.latitud, lng: org.longitud },
      map: map,
      title: org.nombre,
      icon: 'https://maps.gstatic.com/mapfiles/ms2/micons/red-pushpin.png',
    });

    const infoWindow = new google.maps.InfoWindow({
      content: `
        <div class="info-window">
          <h3>${org.nombre}</h3>
          <p>Dirección: ${org.direccion}</p>
          <p>Localidad: ${org.localidad}</p>
          <p>Provincia: ${org.provincia}</p>
          <p>Teléfono: ${org.telefono}</p>
        </div>
      `,
    });

    marker.addListener('click', () => {
      infoWindow.open(map, marker);
    });

    this.mapService
      .getOrganizationSedes(org.id)
      .then((sedes) => {
        sedes?.forEach((sede) => {
          const sedeMarker = new google.maps.Marker({
            position: { lat: sede.latitud, lng: sede.longitud },
            map: map,
            title: sede.nombre,
            icon: 'https://maps.gstatic.com/mapfiles/ms2/micons/blue-pushpin.png',
          });

          const sedeInfoWindow = new google.maps.InfoWindow({
            content: `
              <div class="info-window">
                <h3>${sede.nombre}</h3>
                <p>Dirección: ${sede.direccion}</p>
                <p>Localidad: ${sede.localidad}</p>
                <p>Provincia: ${sede.provincia}</p>
                <p>Teléfono: ${sede.telefono}</p>
              </div>
          `,
          });

          sedeMarker.addListener('click', () => {
            sedeInfoWindow.open(map, sedeMarker);
          });
        });
      })
      .catch((error) => {
        console.error('Error al obtener las sedes:', error);
      });

    google.maps.event.addListenerOnce(map, 'idle', () => {
      map.panTo({
        lat: org.latitud,
        lng: org.longitud,
      });
      map.setZoom(8);
    });
  }
}
