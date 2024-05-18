import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environments } from '../environments/environments';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class OrganizacionService {

  private cuit$ = new BehaviorSubject<string>("");

  private baseUrl = environments.baseUrl;

  constructor(private http: HttpClient) { }

  getOrganizacionByCuit(cuit:string){
    const url = `${this.baseUrl}/Organizacion/${cuit}`;
    return this.http.get<any>(url);
  }

  getAllOrganizaciones(){
    const url = `${this.baseUrl}/Organizacion`;
    return this.http.get<any>(url);
  }

  postInfoOrganizacion(data:any){
    const url = `${this.baseUrl}/Informacion/Detalles`;
    return this.http.post<any>(url,data);
  }

  public getCuitFromStore(){
    return this.cuit$.asObservable();
  }

  public setCuitForStore(cuit:string){
    this.cuit$.next(cuit);
  }
}
