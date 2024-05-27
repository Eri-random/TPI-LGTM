/// <reference types="@types/googlemaps" />
import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { MapService } from 'src/app/services/map.service';
import { GoogleMapsLoaderService } from 'src/app/services/google-maps-loader.service';
import { NgToastService } from 'ng-angular-popup';

@Component({
  selector: 'app-mapa-organizaciones',
  templateUrl: './mapa-organizaciones.component.html',
  styleUrls: ['./mapa-organizaciones.component.css'],
})
export class MapaOrganizacionesComponent implements OnInit {
  organizations: any[] = [];
  provincias: any[] = [];
  provinciaSeleccionada: any = 'todas';
  organizacionSeleccionada: any = 'todas';
  filteredOrganizations: any[] = [];

  constructor(
    private http: HttpClient,
    private mapService: MapService,
    private googleMapsLoader: GoogleMapsLoaderService,
    private toast: NgToastService
  ) {}

  ngOnInit(): void {
    this.getMarker();
    this.getProvinces();
  }

  getMarker(): void {
    this.mapService.getOrganizations().subscribe(
      (data) => {
        this.organizations = data;
        this.googleMapsLoader.load().then(() => {
          this.loadMap();
        });
      },
      (error) => {
        console.error('Error al cargar los datos de ubicaciones:', error);
      }
    );
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

    this.organizations.forEach((org) => {
      const marker = new google.maps.Marker({
        position: { lat: org.latitud, lng: org.longitud },
        map: map,
        title: org.nombre,
      });

      const infoWindow = new google.maps.InfoWindow({
        content: `
        <h3>${org.nombre}</h3><p>${org.direccion}</p>
        <p>${org.localidad}, ${org.provincia}</p>
        <p>Teléfono de contacto: ${org.telefono}</p>
        `,
      });

      marker.addListener('click', () => {
        infoWindow.open(map, marker);
      });
    });
  }

  getProvinces(): void {
    this.mapService.getPronvincias().subscribe(
      (data) => {
        this.provincias = data.provincias;
        this.provincias.sort((a, b) => {
          if (a.nombre > b.nombre) {
            return 1;
          }
          if (a.nombre < b.nombre) {
            return -1;
          }
          return 0;
        });
      },
      (error) => {
        console.error('Error al cargar los datos de provincias:', error);
      }
    );
  }

  onProvinciaChange(): void {
    this.organizacionSeleccionada = 'todas';
    this.filterOrganizations();
    this.updateMap();
  }
  
  onOrganizacionChange(): void {
    if (this.organizacionSeleccionada === 'todas') {
      this.filterOrganizations();
      this.updateMap();
    } else {
      this.selectOrganization();
    }
  }
  
  filterOrganizations(): void {
    if (this.provinciaSeleccionada === 'todas') {
      this.filteredOrganizations = this.organizations;
    } else {
      this.filteredOrganizations = this.organizations.filter(
        (org) => org.provincia === this.provinciaSeleccionada.nombre
      );
    }
  }

  updateMap(): void {
    const provincia = this.provinciaSeleccionada;
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
        zoom: provincia === 'todas' ? 5 : 7, // Ajusta el zoom según la selección de provincia
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
          <h3>${org.nombre}</h3><p>${org.direccion}</p>
          <p>${org.localidad}, ${org.provincia}</p>
          <p>Teléfono de contacto: ${org.telefono}</p>
          `,
      });
  
      marker.addListener('click', () => {
        infoWindow.open(map, marker);
      });
    });
  }
  selectOrganization(): void {
    const org = this.organizacionSeleccionada;

    if (
      this.provinciaSeleccionada !== 'todas' &&
      org.provincia !== this.provinciaSeleccionada.nombre
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

    if (this.provinciaSeleccionada !== 'todas') {
      this.mapService.getPoligonosProvincias().then((geojson) => {
        geojson.features.forEach((feature: any) => {
          if (feature.properties.nombre === this.provinciaSeleccionada.nombre) {
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
      icon: 'https://maps.gstatic.com/mapfiles/ms2/micons/red-pushpin.png', // Ícono para la organización seleccionada
    });

    const infoWindow = new google.maps.InfoWindow({
      content: `
      <h3>${org.nombre}</h3><p>${org.direccion}</p>
      <p>${org.localidad}, ${org.provincia}</p>
      <p>Teléfono de contacto: ${org.telefono}</p>
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
            icon: 'https://maps.gstatic.com/mapfiles/ms2/micons/blue-pushpin.png', // Ícono para las sedes
          });


          const sedeInfoWindow = new google.maps.InfoWindow({
            content: `
          <h3>${sede.nombre}</h3><p>${sede.direccion}</p>
          <p>${sede.localidad}, ${sede.provincia}</p>
          <p>Teléfono de contacto: ${sede.telefono}</p>
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
