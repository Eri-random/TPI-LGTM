import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environments } from '../environments/environments';

@Injectable({
  providedIn: 'root'
})
export class DonacionService {
  
  private baseUrl = environments.baseUrl;

  constructor(private http: HttpClient) { }

  postGuardarDonacion(data: any){
    const url = `${this.baseUrl}/Donacion`;
    return this.http.post<any>(url, data);
  }
}
