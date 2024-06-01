import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environments } from '../../environments/environments';
import { Provinces } from '../interfaces/provinces.interface';

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

  getOrganizationHeadquarters(organizacionId: number) {
    const url = `${this.baseUrl}/Maps/${organizacionId}`;
    return this.http.get<any[]>(url).toPromise();
  }

  getProvinces() {
    const url = 'https://apis.datos.gob.ar/georef/api/provincias';
    return this.http.get<Provinces>(url);
  }

  async getPolygonosProvinces() {
    const response = await fetch('assets/ProvinciasArgentina.geojson');
    return await response.json();
  }
}
