import { Injectable } from '@angular/core';
import { environments } from '../../environments/environments';
import { HttpClient } from '@angular/common/http';
import { Idea } from '../models/idea';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ResponseIdeaService {

  private baseUrl = environments.baseUrl;

  constructor(private http: HttpClient) { }

  postSaveIdea(idea: Idea){
    const url = `${this.baseUrl}/Idea/save`;
    return this.http.post<any>(url, idea);
  }

  getIdeasByUser(userId: number){
    const url = `${this.baseUrl}/Idea/user/${userId}`;
    return this.http.get<any>(url);
  }

  getIdea(ideaId: number): Observable<any> {
    const url = `${this.baseUrl}/Idea/see-detail/${ideaId}`;
    return this.http.get<any>(url);
  }

  deleteIdea(ideaId: number){
    const url = `${this.baseUrl}/Idea/delete/${ideaId}`;
    return this.http.delete<any>(url);
  }

  setGeneratedIdeaMessage(message:any){
    localStorage.setItem('message',JSON.stringify(message));
  }

  getGeneratedIdeaMessage(){
    return JSON.parse(localStorage.getItem('message') || '{}');
  }

  setGeneratedIdea(idea: any){
    localStorage.setItem('idea', JSON.stringify(idea));
  }

  getGeneratedIdea(){
    return JSON.parse(localStorage.getItem('idea') || '{}');
  }
}
