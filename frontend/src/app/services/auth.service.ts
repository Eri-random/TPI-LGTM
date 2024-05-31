import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { User } from '../models/user';
import { environments } from '../environments/environments';
import { LoginForm } from '../interfaces/login-form.interface';
import { JwtHelperService } from '@auth0/angular-jwt';
import { Router } from '@angular/router';
import { BehaviorSubject } from 'rxjs';


@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private userPayload:any;
  private _isLoggedIn$ = new BehaviorSubject<boolean>(false);

  private baseUrl = environments.baseUrl;

  constructor(private http:HttpClient,
  private router:Router
  ) {
    this.userPayload = this.decodedToken();
   }

   login(user:LoginForm){
    const url = `${this.baseUrl}/User/authenticate`;
    return this.http.post<LoginForm>(url,user);
   }

   createAccount(user:User){
    const url = `${this.baseUrl}/User`;
    return this.http.post<User>(url,user);
  }

  singOut(){
    localStorage.clear();
    this.router.navigate(['login'])
  }

  storeToken(tokenValue:string){
    localStorage.setItem('token',tokenValue)
  }

  getToken(){
    return localStorage.getItem('token');
  }

  isLoggedIn():boolean{
    return !!localStorage.getItem('token')
  }

   getIsLoggedIn() {
    return this._isLoggedIn$.asObservable();
  }

  setIsLoggedIn(active:boolean){
    this._isLoggedIn$.next(active);
  }

  decodedToken(){
    const jwtHelper = new JwtHelperService();
    const token = this.getToken()!;
    return jwtHelper.decodeToken(token);
  }

  getFullNameFromToken(){
    if(this.userPayload)
    return this.userPayload.name;
  }

  getRoleFromToken(){
    if(this.userPayload)
      return this.userPayload.role;
  }

  getCuitFromToken(){
    if(this.userPayload){
      return this.userPayload.cuit;
    }
  }

  getOrgNameFromToken(){
    if(this.userPayload){
      return this.userPayload.orgName;
    }
  }

  getEmailFromToken(){
    if(this.userPayload){
      return this.userPayload.email;
    }
  }
}
