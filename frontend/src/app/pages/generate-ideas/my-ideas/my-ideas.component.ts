import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NgToastService } from 'ng-angular-popup';
import { switchMap, tap } from 'rxjs';
import { AuthService } from 'src/app/services/auth.service';
import { ResponseIdeaService } from 'src/app/services/response-idea.service';
import { UserStoreService } from 'src/app/services/user-store.service';

@Component({
  selector: 'app-my-ideas',
  templateUrl: './my-ideas.component.html',
  styleUrls: ['./my-ideas.component.css'],
})
export class MyIdeasComponent implements OnInit {
  email: string = '';
  userId: number = 0;
  data: any[] = [];
  loading: boolean = true;

  constructor(
    private responseIdeaService: ResponseIdeaService,
    private router: Router,
    private userStore: UserStoreService,
    private toast: NgToastService,
    private authService:AuthService
  ) {}

  ngOnInit(): void {
    this.userStore.getEmailFromStore()
    .subscribe(val =>{
      const emailFromToken =  this.authService.getEmailFromToken();
      this.email = val || emailFromToken;
    });
    
    this.userStore.getUserByEmail(this.email)
    .subscribe({
        next: (res) => {
          this.userId = res.id;
          this.responseIdeaService.getIdeasByUser(this.userId).subscribe({
            next: (res) => {
              console.log('Ideas retrieved:', res); // Verificar las ideas recibidas
              this.data = res;

              setTimeout(() => {
                this.loading = false;
              }, 1000);
            },
          });
        },
      });
  }

  seeDetail(idea: any) {
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
          position:'topRight',
        });
      },
      error: ({ error }) => {
        this.toast.error({
          detail: 'Error al eliminar la idea',
          duration: 5000,
          position: 'topRight',
        });
      },
    });
  }
}
