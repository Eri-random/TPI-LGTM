import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import {Router} from "@angular/router";
import { NgToastService } from 'ng-angular-popup';
import ValidateForm from 'src/app/helpers/validateForm';
import { AuthService } from 'src/app/services/auth.service';
import { GenerateIdeaService } from 'src/app/services/generate-idea.service';
import { RecognitionTelaService } from 'src/app/services/recognition-tela.service';
import { ResponseIdeaService } from 'src/app/services/response-idea.service';
import { SpinnerService } from 'src/app/services/spinner.service';
@Component({
  selector: 'app-generate-ideas',
  templateUrl: './generate-ideas.component.html',
  styleUrls: ['./generate-ideas.component.css']
})
export class GenerateIdeasComponent implements OnInit {

  imagePreviews: string[] = ['/assets/img/placeholder.png'];
  imageFiles: File[] = [];
  ideaForm!: FormGroup;
  isLogged!:boolean;
  loading: boolean = false;
  message: string = "";
  errorImage:string = "";

  constructor( private formBuilder: FormBuilder, private router:Router,
    private reconomientoTelaService: RecognitionTelaService,
    private toast: NgToastService,
    private authService: AuthService,
    private generateIdeaService: GenerateIdeaService,
    private spinnerService: SpinnerService,
    private responseIdeaService: ResponseIdeaService
  ){
  }

  ngOnInit(): void {
    this.isLogged = this.authService.isLoggedIn();
    this.ideaForm= this.formBuilder.group({
      tipoDeTela: ["", [Validators.required]],
      color:["",[Validators.required]],
      largo:["",[Validators.required]],
      ancho: ["",[Validators.required]]
    });
  }

  get fm(){
    return this.ideaForm.controls;
  }

  onFileChange(event: any, index: number) {
    const files = event.target.files;

    if (!files || files.length === 0 || !window.FileReader) return;

    const reader = new FileReader();
    reader.readAsDataURL(files[0]);

    reader.onloadend = () => {
      this.imagePreviews[index] = reader.result as string;
    };
    this.imageFiles.push(files);
    this.errorImage = '';

    this.spinnerService.showIdea();
    /*Reconocimiento momentanea de una imagen, ver de mejorar*/
    this.reconomientoTelaService.classifyImage(files[0])
      .subscribe(({tela}) =>{
        this.spinnerService.hideIdea();
        this.ideaForm.get('tipoDeTela')?.setValue(tela);
      },
      (error) => {
        this.spinnerService.hideIdea();
        this.ideaForm.get('tipoDeTela')?.setValue('');
        this.errorImage = error.error;
      })
  }

  addImage() {
    if (this.imagePreviews.length < 4) {
      this.imagePreviews.push('/assets/img/placeholder.png');
    } 
  }

  removeImage(index: number) {
    this.imagePreviews.splice(index, 1);
    this.imageFiles.splice(index, 1);
  }

  submitForm() {

    if(!this.ideaForm.valid){
      ValidateForm.validateAllFormFileds(this.ideaForm);
      console.log("SIN DATOS");
      return;
    }

    this.message = `Por favor, proporciona detalles sobre el trozo de tela que deseas reciclar:\n\n` +
    `- Tipo de tela: ${this.ideaForm.get('tipoDeTela')?.value}\n` +
    `- Color: ${this.ideaForm.get('color')?.value}\n` +
    `- Largo: ${this.ideaForm.get('largo')?.value}\n` +
    `- Ancho: ${this.ideaForm.get('ancho')?.value}\n\n` +
    `Utilizando estos detalles, generaremos una idea de producto reciclado adecuada y única. Que la respuesta tambien me de la dificultad si es Fácil, Media o Dificil.`;
    this.spinnerService.show();
    
    this.responseIdeaService.setGeneratedIdeaMessage(this.message);

    this.generateIdeaService.postGenerateIdea(this.message).subscribe(
      (response) => {
        this.spinnerService.hide();
        this.responseIdeaService.setGeneratedIdea(response);
        this.router.navigate(['/response-idea']);
      },
      (error) => {
        this.spinnerService.hide();
        console.log(error);
      }
    );
  }

}
