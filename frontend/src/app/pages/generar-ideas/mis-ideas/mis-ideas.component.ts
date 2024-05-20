import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NgToastService } from 'ng-angular-popup';
import { switchMap, tap } from 'rxjs';
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
    private toast: NgToastService
  ) {}

  ngOnInit(): void {
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
    this.router.navigate(['/mis-ideas', idea.id]);
  }

  deleteIdea(id: number) {
    this.responseIdeaService.deleteIdea(id).subscribe({
      next: (res) => {
        this.data = this.data.filter((idea) => idea.id !== id);

        this.toast.success({
          detail: 'EXITO',
          summary: 'Idea eliminada exitosamente',
          duration: 5000,
          position: 'topCenter',
        });
      },
      error: ({ error }) => {
        this.toast.error({
          detail: 'Error al eliminar la idea',
          duration: 5000,
          position: 'topCenter',
        });
      },
    });
  }
}
