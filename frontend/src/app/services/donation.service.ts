import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environments } from '../../environments/environments';

@Injectable({
  providedIn: 'root'
})
export class DonationService {
  
  private baseUrl = environments.baseUrl;

  constructor(private http: HttpClient) { }

  postSaveDonation(data: any){
    const url = `${this.baseUrl}/Donation`;
    return this.http.post<any>(url, data);
  }

  getAllDonationsByUserId(userId: number){
    const url = `${this.baseUrl}/Donation/user/${userId}`;
    return this.http.get<any>(url);
  }
}
