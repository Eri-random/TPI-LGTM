import { Injectable } from '@angular/core';
import { environments } from '../environments/environments';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class ResponseIdeaService {

  private baseUrl = environments.baseUrl;

  constructor(private http: HttpClient) { }

  postSaveIdea(idea: any){
    const url = `${this.baseUrl}/Idea/save`;
    return this.http.post<any>(url, {idea});
  }

  setGeneratedIdea(idea: any){
    localStorage.setItem('idea', JSON.stringify(idea));
  }

  getGeneratedIdea(){
    return JSON.parse(localStorage.getItem('idea') || '{}');
  }
}
