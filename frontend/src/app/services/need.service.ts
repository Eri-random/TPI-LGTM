import { Injectable } from '@angular/core';
import { environments } from '../environments/environments';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class NeedService {

  private baseUrl = environments.baseUrl;

  constructor(private http: HttpClient) { }

  getAllNeeds(){
    const url = `${this.baseUrl}/Necesidad`;
    return this.http.get<any>(url);
  }

}
