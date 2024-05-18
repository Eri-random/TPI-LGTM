/// <reference types="@types/googlemaps" />
import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { MapService } from 'src/app/services/map.service';
import { GoogleMapsLoaderService } from 'src/app/services/google-maps-loader.service';

@Component({
  selector: 'app-mapa-organizaciones',
  templateUrl: './mapa-organizaciones.component.html',
  styleUrls: ['./mapa-organizaciones.component.css'],
})
export class MapaOrganizacionesComponent implements OnInit {
  organizations: any[] = [];
  provincias: any[] = [];
  opcionSeleccionado: any | string = 'todas';

  constructor(
    private http: HttpClient,
    private mapService: MapService,
    private googleMapsLoader: GoogleMapsLoaderService
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
        position: { lat: org.lat, lng: org.lng },
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

  selectProvince(): void {
    if (this.opcionSeleccionado === 'todas') {
      this.getMarker();
    } else {
      const map = new google.maps.Map(
        document.getElementById('map') as HTMLElement,
        {
          center: {
            lat: this.opcionSeleccionado?.centroide.lat,
            lng: this.opcionSeleccionado?.centroide.lon,
          },
          zoom: 7,
          minZoom: 5,
          maxZoom: 16,
        }
      );

      this.mapService.getPoligonosProvincias().then((geojson) => {
        geojson.features.forEach((feature: any) => {
          if (feature.properties.nombre === this.opcionSeleccionado?.nombre) {
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

      this.organizations.forEach((org) => {
        if (org.provincia === this.opcionSeleccionado?.nombre) {
          if(org.localidad === 'Ciudad Autónoma de Buenos Aires'){
            org.provincia = 'Ciudad Autónoma de Buenos Aires';
          }
          const marker = new google.maps.Marker({
            position: { lat: org.lat, lng: org.lng },
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

          // o se coloca aca cuando se selecciona una provincia
          // this.mapService.getPoligonosProvincias().then((geojson) => {
          //   geojson.features.forEach((feature: any) => {
          //     if (feature.properties.nombre === this.opcionSeleccionado?.nombre) {
          //       const polygon = new google.maps.Polygon({
          //         paths: feature.geometry.coordinates[0].map((coord: any) => {
          //           return { lat: coord[1], lng: coord[0] };
          //         }),
          //         strokeColor: '#FF0000',
          //         strokeOpacity: 0.8,
          //         strokeWeight: 2,
          //         fillColor: 'rgba(255, 0, 0, 0.3)',
          //         fillOpacity: 0.35,
          //       });
          //       polygon.setMap(map);
          //     }
          //   });
          // });
        }
      });
    }
  }
}
