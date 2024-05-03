import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { User } from '../models/user';
import { environments } from '../environments/environments';


@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private baseUrl = environments.baseUrl;

  constructor(private http:HttpClient) {

   }

   crearCuenta(user:User){
    const url = `${this.baseUrl}/Usuarios`;
    return this.http.post<User>(url,user);
  }
}
