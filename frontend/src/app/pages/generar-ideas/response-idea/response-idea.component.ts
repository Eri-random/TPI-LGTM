import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { ResponseIdeaService } from 'src/app/services/response-idea.service';

@Component({
  selector: 'app-response-idea',
  templateUrl: './response-idea.component.html',
  styleUrls: ['./response-idea.component.css']
})
export class ResponseIdeaComponent {

  response : any;

  constructor(
    private responseIdeaService: ResponseIdeaService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.response = this.responseIdeaService.getGeneratedIdea();
  }
  

  generateNewIdea(){
    localStorage.removeItem('idea');
    this.router.navigate(['/generar-ideas']);
  }

  saveIdea(){
    this.responseIdeaService.postSaveIdea(this.response);
    this.router.navigate(['/ideas']);
  }
}
