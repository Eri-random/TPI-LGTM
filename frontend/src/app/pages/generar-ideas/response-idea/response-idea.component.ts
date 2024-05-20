import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { switchMap, tap } from 'rxjs';
import { Idea } from 'src/app/models/idea';
import { ResponseIdeaService } from 'src/app/services/response-idea.service';
import { UserStoreService } from 'src/app/services/user-store.service';

@Component({
  selector: 'app-response-idea',
  templateUrl: './response-idea.component.html',
  styleUrls: ['./response-idea.component.css'],
})
export class ResponseIdeaComponent {
  email: string = '';
  userId: number = 0;
  response: any;

  constructor(
    private responseIdeaService: ResponseIdeaService,
    private router: Router,
    private userStore: UserStoreService
  ) {}

  ngOnInit(): void {
    this.response = this.responseIdeaService.getGeneratedIdea();
    this.userStore
      .getEmailFromStore()
      .pipe(
        tap((email) => {
          this.email = email;
        }),
        switchMap((email) =>
          this.userStore.getUserByEmail(email).pipe(
            tap((user) => {
              console.log('User retrieved:', user); // Verificar el usuario recibido
            })
          )
        )
      )
      .subscribe({
        next: (res) => {
          this.userId = res.id;
        },
      });
  }

  generateNewIdea() {
    localStorage.removeItem('idea');
    this.router.navigate(['/generar-ideas']);
  }

  saveIdea() {
    this.responseIdeaService
      .postSaveIdea({
        titulo: this.response.idea,
        usuarioId: this.userId,
        pasos:  this.response.steps.map((paso: any, index: number) => ({
          pasoNum: index + 1,
          descripcion: paso
        }))
      })
      .subscribe({
        next: (res) => {
          // this.router.navigate(['/generar-ideas']);
        },
      });
  }
}
