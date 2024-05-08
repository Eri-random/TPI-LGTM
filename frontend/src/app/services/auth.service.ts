import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { User } from '../models/user';
import { environments } from '../environments/environments';
import { LoginForm } from '../interfaces/login-form.interface';


@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private baseUrl = environments.baseUrl;

  constructor(private http:HttpClient) {

   }

   login(user:LoginForm){
    const url = `${this.baseUrl}/Usuarios/authenticate`;
    return this.http.post<LoginForm>(url,user);
   }

   crearCuenta(user:User){
    const url = `${this.baseUrl}/Usuarios`;
    return this.http.post<User>(url,user);
  }
}
