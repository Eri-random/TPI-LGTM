import { Injectable } from '@angular/core';
import { environments } from '../environments/environments';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class DonacionesService {

  private baseUrl = environments.baseUrl;

  constructor(private http: HttpClient) { }

  
  getDonacionesByOrganizacionId(id:number){
    const url = `${this.baseUrl}/Donacion/organizacion/${id}`;
    return this.http.get<any>(url);
  }

}
