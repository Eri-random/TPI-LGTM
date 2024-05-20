import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { environments } from '../environments/environments';

@Injectable({
  providedIn: 'root'
})
export class UserStoreService {

  private baseUrl = environments.baseUrl;

  private fullName$ = new BehaviorSubject<string>("");
  private role$ = new BehaviorSubject<string>("");

  constructor(private http: HttpClient) { }


  getUserByEmail(email:string){
    const url = `${this.baseUrl}/Usuarios/${email}`;
    return this.http.get<any>(url);
  }


  public getRolFromStore(){
    return this.role$.asObservable();
  }

  public setRolForStore(role:string){
    this.role$.next(role);
  }

  public getFullNameFromStore(){
    return this.fullName$.asObservable();
  }

  public setFullNameForStore(fullname:string){
    this.fullName$.next(fullname);
  }

}
