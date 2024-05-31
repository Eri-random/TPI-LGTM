import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environments } from '../environments/environments';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class HeadquartersService {

  private baseUrl = environments.baseUrl;
  private dataDirection$ = new BehaviorSubject<any>(null);

  constructor(private http: HttpClient) { }

  getHeadquartersByOrganization(organizationId: number){
    const url = `${this.baseUrl}/Headquarters/${organizationId}`;
    return this.http.get<any>(url);
  }

  postHeadquarters(headquarters : any[]): Observable<any> {
    const url = `${this.baseUrl}/Headquarters`;
    return this.http.post<any>(url, headquarters);
  }

  updateHeadquarters(headquarter: any): Observable<any> {
    const url = `${this.baseUrl}/Headquarters`;
    return this.http.put<any>(url, headquarter);
  }

  deleteHeadquarters(headquartersId: number): Observable<any> {
    const url = `${this.baseUrl}/Headquarters/${headquartersId}`;
    return this.http.delete<any>(url);
  }

  getHeadquarterById(headquartersId: number): Observable<any> {
    const url = `${this.baseUrl}/Headquarters/Details/${headquartersId}`;
    return this.http.get<any>(url);
  }

  postNearestHeadquarter(data: any): Observable<any> {
    const url = `${this.baseUrl}/Headquarters/Closest`;
    return this.http.post<any>(url, data);
  }


  public setDataDirection(data: any): void {
    this.dataDirection$.next(data);
  }

  public getDataDirection(): any {
    return this.dataDirection$.asObservable();
  }

}
