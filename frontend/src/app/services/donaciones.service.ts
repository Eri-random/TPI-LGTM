import { Injectable } from '@angular/core';
import { environments } from '../environments/environments';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';

export interface Donacion {
  name: string;
  telefono: string;
  email: string;
  producto: string;
  cantidad: number;
}

@Injectable({
  providedIn: 'root',
})
export class DonacionesService {
  private baseUrl = environments.baseUrl;

  private donacionesSubject: BehaviorSubject<Donacion[]> = new BehaviorSubject<
    Donacion[]
  >([]);
  public donaciones$: Observable<Donacion[]> =
    this.donacionesSubject.asObservable();

  constructor(private http: HttpClient) {}

  getDonacionesByOrganizacionId(id: number) {
    const url = `${this.baseUrl}/Donacion/organizacion/${id}`;
    return this.http.get<any>(url);
  }

  addDonacion(donacion: Donacion) {
    const donaciones = this.donacionesSubject.getValue();
    donaciones.push(donacion);
    this.donacionesSubject.next(donaciones);
  }

  // Método para obtener todas las donaciones
  getDonaciones(): Observable<Donacion[]> {
    return this.donaciones$;
  }
}
