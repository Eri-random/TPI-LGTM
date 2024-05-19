import { Component } from '@angular/core';
import { ResponseIdeaService } from 'src/app/services/response-idea.service';

@Component({
  selector: 'app-response-idea',
  templateUrl: './response-idea.component.html',
  styleUrls: ['./response-idea.component.css']
})
export class ResponseIdeaComponent {
  response: any;
  constructor(
    private responseIdeaService: ResponseIdeaService
  ) { }

  ngOnInit(): void {
    this.response = this.responseIdeaService.generatedIdea;
  }

}
