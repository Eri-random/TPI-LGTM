import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environments } from '../../environments/environments';

export interface Need {
  id: number;
  nombre: string;
  icono?: string;
  subcategoria?: Subcategory[];
}

export interface Subcategory {
  id: number;
  nombre: string;
}

export interface Campaign {
  id: number;
  title: string;
  startDate: string;
  endDate: string;
  organizacionId: number;
  needs: Need[];
  isActive: boolean;
  imageUrl?: string;
}

@Injectable({
  providedIn: 'root'
})
export class CampaignService {

  private baseUrl = environments.baseUrl;

  constructor(private http: HttpClient) { }

  getAllCampaigns(organizationId: string): Observable<Campaign[]> {
    const url = `${this.baseUrl}/Campaign/${organizationId}`;
    return this.http.get<Campaign[]>(url);
  }

  createCampaign(campaign: Campaign): Observable<Campaign> {
    const url = `${this.baseUrl}/Campaign`;
    return this.http.post<Campaign>(url, campaign);
  }

  updateCampaign(campaign: Campaign): Observable<Campaign> {
    const url = `${this.baseUrl}/Campaign`;
    return this.http.put<Campaign>(url, campaign);
  }

  deleteCampaign(campaignId: number): Observable<void> {
    const url = `${this.baseUrl}/Campaign/${campaignId}`;
    return this.http.delete<void>(url);
  }
}
