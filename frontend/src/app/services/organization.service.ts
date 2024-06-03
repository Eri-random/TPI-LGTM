import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environments } from '../../environments/environments';
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
    const url = `${this.baseUrl}/Organization/${cuit}`;
    return this.http.get<any>(url);
  }

  getOrganizationById(id:number){
    const url = `${this.baseUrl}/Organization/id/${id}`;
    return this.http.get<any>(url);
  }

  getAllOrganizations(){
    const url = `${this.baseUrl}/Organization`;
    return this.http.get<any>(url);
  }

  getPaginatedOrganizations(page: number, pageSize: number): Observable<any[]> {
    const url = `${this.baseUrl}/Organization/pagination?page=${page}&pageSize=${pageSize}`;
    return this.http.get<any[]>(url);
  }

  postInfoOrganization(formData: FormData): Observable<any> {
    const url = `${this.baseUrl}/Information/Details`;
    return this.http.post<any>(url, formData);
  }

  putInfoOrganization(formData: FormData): Observable<any> {
    const url = `${this.baseUrl}/Information`;
    return this.http.put<any>(url, formData);
  }

  getAssignedSubcategories(organizationId: number) {
    const url = `${this.baseUrl}/Organization/${organizationId}/subcategories`
    return this.http.get<any[]>(url);
  }

  assignSubcategories(organizationId: number, subcategories: any[]) {
    const url = `${this.baseUrl}/Organization/${organizationId}/assign-need`
    return this.http.post(url, subcategories);
  }

  getGroupedSubcategories(organizationId:number){
    const url = `${this.baseUrl}/Organization/${organizationId}/grouped-subcategories`
    return this.http.get(url);
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
