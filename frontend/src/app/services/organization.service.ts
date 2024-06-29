import { HttpClient, HttpParams } from '@angular/common/http';
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

  getOrganizationByCuit(cuit: string) {
    const url = `${this.baseUrl}/Organization/cuit/${cuit}`;
    return this.http.get<any>(url);
  }

  getOrganizationById(id: number) {
    const url = `${this.baseUrl}/Organization/${id}`;
    return this.http.get<any>(url);
  }

  getAllOrganizations() {
    const url = `${this.baseUrl}/Organization`;
    return this.http.get<any>(url);
  }

  getPaginatedOrganizations(params: any): Observable<any[]> {
    const url = `${this.baseUrl}/Organization/pagination`;
    return this.http.get<any[]>(url, { params });
  }
  
  postInfoOrganization(formData: FormData): Observable<any> {
    const url = `${this.baseUrl}/Information`;
    return this.http.post<any>(url, formData);
  }

  putInfoOrganization(formData: FormData): Observable<any> {
    const url = `${this.baseUrl}/Information`;
    return this.http.put<any>(url, formData);
  }

  putOrganization(organization: any): Observable<any> {
    const url = `${this.baseUrl}/Organization`;
    return this.http.put<any>(url, organization);
  }

  getAssignedSubcategories(organizationId: number) {
    const url = `${this.baseUrl}/Organization/${organizationId}/subcategories`
    return this.http.get<any[]>(url);
  }

  assignSubcategories(organizationId: number, subcategories: any[]) {
    const url = `${this.baseUrl}/Organization/${organizationId}/need`
    return this.http.post(url, subcategories);
  }

  getGroupedSubcategories(organizationId: number) {
    const url = `${this.baseUrl}/Organization/${organizationId}/grouped-subcategories`
    return this.http.get(url);
  }

  public getCuitFromStore() {
    return this.cuit$.asObservable();
  }

  public setCuitForStore(cuit: string) {
    this.cuit$.next(cuit);
  }

  public getOrgNameFromStore() {
    return this.orgName$.asObservable();
  }

  public setOrgNameForStore(orgName: string) {
    this.orgName$.next(orgName);
  }
  
}
