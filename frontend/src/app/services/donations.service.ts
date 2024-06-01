import { Injectable } from '@angular/core';
import { environments } from '../../environments/environments';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';

export interface Donation {
  name: string;
  telefono: string;
  email: string;
  producto: string;
  cantidad: number;
}

@Injectable({
  providedIn: 'root',
})
export class DonationsService {
  private baseUrl = environments.baseUrl;

  private donationsSubject: BehaviorSubject<Donation[]> = new BehaviorSubject<
    Donation[]
  >([]);
  public donations$: Observable<Donation[]> =
    this.donationsSubject.asObservable();

  constructor(private http: HttpClient) {}

  getDonationsByOrganizationId(id: number) {
    const url = `${this.baseUrl}/Donation/organization/${id}`;
    return this.http.get<any>(url);
  }

  addDonation(donation: Donation) {
    const donations = this.donationsSubject.getValue();
    donations.push(donation);
    this.donationsSubject.next(donations);
  }

  // Método para obtener todas las donaciones
  getDonations(): Observable<Donation[]> {
    return this.donations$;
  }
}
