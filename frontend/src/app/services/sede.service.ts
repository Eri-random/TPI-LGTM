import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environments } from '../environments/environments';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SedeService {

  private baseUrl = environments.baseUrl;
  private dataDirection$ = new BehaviorSubject<any>(null);

  constructor(private http: HttpClient) { }

  getSedesByOrganization(organizationId: number){
    const url = `${this.baseUrl}/Sede/${organizationId}`;
    return this.http.get<any>(url);
  }

  postSede(sedes: any[]): Observable<any> {
    const url = `${this.baseUrl}/Sede`;
    return this.http.post<any>(url, sedes);
  }

  updateSede(sede: any): Observable<any> {
    const url = `${this.baseUrl}/Sede`;
    return this.http.put<any>(url, sede);
  }

  deleteSede(sedeId: number): Observable<any> {
    const url = `${this.baseUrl}/Sede/${sedeId}`;
    return this.http.delete<any>(url);
  }

  getSedeById(sedeId: number): Observable<any> {
    const url = `${this.baseUrl}/Sede/Detalle/${sedeId}`;
    return this.http.get<any>(url);
  }

  postSedeMasCercana(data: any): Observable<any> {
    const url = `${this.baseUrl}/Sede/MasCercana`;
    return this.http.post<any>(url, data);
  }


  public setDataDirection(data: any): void {
    this.dataDirection$.next(data);
  }

  public getDataDirection(): any {
    return this.dataDirection$.asObservable();
  }

}
