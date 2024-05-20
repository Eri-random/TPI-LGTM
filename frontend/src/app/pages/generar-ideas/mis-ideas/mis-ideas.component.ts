import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { switchMap, tap } from 'rxjs';
import { AuthService } from 'src/app/services/auth.service';
import { ResponseIdeaService } from 'src/app/services/response-idea.service';
import { UserStoreService } from 'src/app/services/user-store.service';

@Component({
  selector: 'app-mis-ideas',
  templateUrl: './mis-ideas.component.html',
  styleUrls: ['./mis-ideas.component.css'],
})
export class MisIdeasComponent implements OnInit {
  email: string = '';
  userId: number = 0;
  data: any[] = [];

  constructor(
    private responseIdeaService: ResponseIdeaService,
    private router: Router,
    private userStore: UserStoreService,
    private authService:AuthService
  ) {}

  ngOnInit(): void {
    this.email = this.authService.getEmailFromToken();
    
    this.userStore.getUserByEmail(this.email)
    .subscribe({
        next: (res) => {
          this.userId = res.id;
          this.responseIdeaService.getIdeasByUser(this.userId).subscribe({
            next: (res) => {
              console.log('Ideas retrieved:', res); // Verificar las ideas recibidas
              this.data = res;
            },
          });
        },
      });
  }

  verDetalle(idea: any) {
    console.log('Ver Detalle:', idea);
    this.router.navigate(['/mis-ideas', idea.id]);
  }

  deleteIdea(idea: any) {
    console.log('Delete Idea:', idea);
  }
}
