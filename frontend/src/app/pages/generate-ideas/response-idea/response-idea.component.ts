import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { NgToastService } from 'ng-angular-popup';
import { switchMap, tap } from 'rxjs';
import { Idea } from 'src/app/models/idea';
import { AuthService } from 'src/app/services/auth.service';
import { GenerateIdeaService } from 'src/app/services/generate-idea.service';
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
  img!: string;

  constructor(
    private responseIdeaService: ResponseIdeaService,
    private router: Router,
    private toast: NgToastService,
    private authService: AuthService,
    private userStore: UserStoreService,
    private ideaService: GenerateIdeaService
  ) {}

  ngOnInit(): void {
    this.response = this.responseIdeaService.getGeneratedIdea();
    
    this.userStore.getEmailFromStore()
    .subscribe(val =>{
      const emailFromToken = this.authService.getEmailFromToken();
      this.email = val || emailFromToken;
    });
        
    this.userStore.getUserByEmail(this.email).subscribe(resp =>{
      this.userId = resp.id;
    })
  }

  generateNewIdea() {
    localStorage.removeItem('idea');
    this.router.navigate(['/generar-ideas']);
  }

  saveIdea() {
    console.log(this.userId);
    this.responseIdeaService
      .postSaveIdea({
        titulo: this.response.idea,
        usuarioId: this.userId,
        dificultad: this.response.dificultad,
        pasos:  this.response.steps.map((paso: any, index: number) => ({
          pasoNum: index + 1,
          descripcion: paso
        }))
      })
      .subscribe({
        next: (res) => {
          this.toast.success({
            detail: 'EXITO',
            summary: 'Idea guardada exitosamente',
            duration: 3000,
            position: 'topRight',
          });

          setTimeout(() => {
            this.router.navigate(['/mis-ideas']);
          }, 3000);
        },
        error: (error:any) => {
          this.toast.error({
            detail: 'ERROR',
            summary: error.error,
            duration: 3000,
            position:'topRight',
          });
        }
      });
  }
}
