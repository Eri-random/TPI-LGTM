import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environments } from '../environments/environments';
import { Provincias } from '../interfaces/provincias.interface';

@Injectable({
  providedIn: 'root',
})
export class MapService {
  private baseUrl = environments.baseUrl;

  constructor(private http: HttpClient) {}

  getOrganizations() {
    const url = `${this.baseUrl}/Maps`;
    return this.http.get<any[]>(url);
  }

  getPronvincias() {
    const url = 'https://apis.datos.gob.ar/georef/api/provincias';
    return this.http.get<Provincias>(url);
  }

  async getPoligonosProvincias() {
    const response = await fetch('assets/ProvinciasArgentina.geojson');
    return await response.json();
  }
}
