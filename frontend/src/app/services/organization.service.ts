import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environments } from '../environments/environments';
import { BehaviorSubject, Observable, tap } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class OrganizationService {

  private cuit$ = new BehaviorSubject<string>("");
  private orgName$ = new BehaviorSubject<string>("");


  private baseUrl = environments.baseUrl;

  constructor(private http: HttpClient) { }

  getOrganizationByCuit(cuit:string){
    const url = `${this.baseUrl}/Organizacion/${cuit}`;
    return this.http.get<any>(url);
  }

  getOrganizationById(id:number){
    const url = `${this.baseUrl}/Organizacion/id/${id}`;
    return this.http.get<any>(url);
  }

  getAllOrganizations(){
    const url = `${this.baseUrl}/Organizacion`;
    return this.http.get<any>(url);
  }

  getPaginatedOrganizations(page: number, pageSize: number): Observable<any[]> {
    const url = `${this.baseUrl}/Organizacion/paginacion?page=${page}&pageSize=${pageSize}`;
    return this.http.get<any[]>(url);
  }

  postInfoOrganization(formData: FormData): Observable<any> {
    const url = `${this.baseUrl}/Informacion/Detalles`;
    return this.http.post<any>(url, formData);
  }

  putInfoOrganization(formData: FormData): Observable<any> {
    const url = `${this.baseUrl}/Informacion`;
    return this.http.put<any>(url, formData);
  }

  getAssignedSubcategories(organizacionId: number) {
    const url = `${this.baseUrl}/Organizacion/${organizacionId}/subcategorias`
    return this.http.get<any[]>(url);
  }

  assignSubcategories(organizacionId: number, subcategorias: any[]) {
    const url = `${this.baseUrl}/Organizacion/${organizacionId}/asignar-necesidad`
    return this.http.post(url, subcategorias);
  }

  public getCuitFromStore(){
    return this.cuit$.asObservable();
  }

  public setCuitForStore(cuit:string){
    this.cuit$.next(cuit);
  }

  public getOrgNameFromStore(){
    return this.orgName$.asObservable();
  }

  public setOrgNameForStore(orgName:string){
    this.orgName$.next(orgName);
  }
}
