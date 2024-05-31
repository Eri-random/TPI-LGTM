import { Injectable } from '@angular/core';
import { environments } from '../environments/environments';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class GenerateIdeaService {

  private baseUrl = environments.baseUrl;

  constructor(private http: HttpClient) { }

  postGenerateIdea(message: string){
    const url = `${this.baseUrl}/Idea/generate`;
    return this.http.post<any>(url, {message});
  }
}
