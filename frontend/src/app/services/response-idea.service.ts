import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ResponseIdeaService {
  generatedIdea: any;
  constructor() { }

  setGeneratedIdea(idea: any){
    this.generatedIdea = localStorage.setItem('idea', JSON.stringify(idea));
  }
}
