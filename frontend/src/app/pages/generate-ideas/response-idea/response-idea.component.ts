import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { NgToastService } from 'ng-angular-popup';
import { AuthService } from 'src/app/services/auth.service';
import { ResponseIdeaService } from 'src/app/services/response-idea.service';
import { UserStoreService } from 'src/app/services/user-store.service';

interface Step {
  text: string;
  imageUrl?: string;
}
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
  formattedSteps: Step[] = [];

  constructor(
    private responseIdeaService: ResponseIdeaService,
    private router: Router,
    private toast: NgToastService,
    private authService: AuthService,
    private userStore: UserStoreService,
  ) {}

  ngOnInit(): void {
    this.response = this.responseIdeaService.getGeneratedIdea();
    console.log(this.response);
    this.formatSteps();

    this.userStore.getEmailFromStore()
    .subscribe(val =>{
      const emailFromToken = this.authService.getEmailFromToken();
      this.email = val || emailFromToken;
    });
        
    this.userStore.getUserByEmail(this.email).subscribe(resp =>{
      this.userId = resp.id;
    })
  }

  formatSteps() {
    if (this.response && this.response.steps) {
      this.formattedSteps = this.response.steps.map((step: string) => {
        const regex = /(.*)(ImageURL:\s*(https?:\/\/[^\s]+))/;
        const matches = step.match(regex);
        if (matches) {
          return {
            text: matches[1].trim(),
            imageUrl: matches[3]
          };
        } else {
          return {
            text: step,
            imageUrl: null
          };
        }
      });
    }
  }

  generateNewIdea() {
    localStorage.removeItem('idea');
    this.router.navigate(['/generar-ideas']);
  }

  saveIdea() {
    this.response.steps = this.response.steps.map((step: string) => {
      const regex = /(.*)(ImageURL:\s*(https?:\/\/[^\s]+))/;
      const matches = step.match(regex);
      if (matches) {
        return matches[1].trim();
      } else {
        return step;
      }
    });

    this.responseIdeaService
      .postSaveIdea({
        titulo: this.response.idea,
        usuarioId: this.userId,
        dificultad: this.response.dificultad,
        pasos:  this.response.steps.map((paso: any, index: number) => ({
          pasoNum: index + 1,
          descripcion: paso,
          imagenUrl: this.formattedSteps[index].imageUrl
        })),
        imageUrl:this.response.imageUrl
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
