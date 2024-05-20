import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { switchMap, tap } from 'rxjs';
import { ResponseIdeaService } from 'src/app/services/response-idea.service';

@Component({
  selector: 'app-ver-idea',
  templateUrl: './ver-idea.component.html',
  styleUrls: ['./ver-idea.component.css']
})
export class VerIdeaComponent implements OnInit{
  idea: any;

  constructor(
    private route: ActivatedRoute,
    private responseIdeaService: ResponseIdeaService
  ) {}

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      const ideaId = params['id'];
      this.responseIdeaService.getIdea(ideaId).subscribe(
        (data) => {
          this.idea = data;
        },
        (error) => {
          console.error('Error al cargar la idea:', error);
        }
      );
    });
  }
}
