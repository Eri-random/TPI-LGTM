import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import {Router} from "@angular/router";
import { NgToastService } from 'ng-angular-popup';
import ValidateForm from 'src/app/helpers/validateForm';
import { AuthService } from 'src/app/services/auth.service';
import { GenerarIdeaService } from 'src/app/services/generar-idea.service';
import { ReconocimientoTelaService } from 'src/app/services/reconocimiento-tela.service';
import { ResponseIdeaService } from 'src/app/services/response-idea.service';
import { SpinnerService } from 'src/app/services/spinner.service';
@Component({
  selector: 'app-generar-ideas',
  templateUrl: './generar-ideas.component.html',
  styleUrls: ['./generar-ideas.component.css']
})
export class GenerarIdeasComponent implements OnInit {

  imagePreviews: string[] = ['https://media.istockphoto.com/id/1226328537/es/vector/soporte-de-posici%C3%B3n-de-imagen-con-un-icono-de-c%C3%A1mara-gris.jpg?s=612x612&w=0&k=20&c=8igCt_oe2wE-aP0qExUDfwicSNUCb4Ho9DiKCq0rSaA='];
  imageFiles: File[] = [];
  ideaForm!: FormGroup;
  isLogged!:boolean;
  loading: boolean = false;
  message: string = "";

  constructor( private formBuilder: FormBuilder, private router:Router,
    private reconomientoTelaService: ReconocimientoTelaService,
    private toast: NgToastService,
    private authService: AuthService,
    private generarIdeaService: GenerarIdeaService,
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

    this.spinnerService.showIdea();
    /*Reconocimiento momentanea de una imagen, ver de mejorar*/
    this.reconomientoTelaService.classifyImage(files[0])
      .subscribe(({tela}) =>{
        this.spinnerService.hideIdea();
        this.ideaForm.get('tipoDeTela')?.setValue(tela);
      })
  }

  addImage() {
    if (this.imagePreviews.length < 4) {
      this.imagePreviews.push('https://media.istockphoto.com/id/1226328537/es/vector/soporte-de-posici%C3%B3n-de-imagen-con-un-icono-de-c%C3%A1mara-gris.jpg?s=612x612&w=0&k=20&c=8igCt_oe2wE-aP0qExUDfwicSNUCb4Ho9DiKCq0rSaA=');
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
    `Utilizando estos detalles, generaremos una idea de producto reciclado adecuada.`;
    this.spinnerService.show();
    
    this.generarIdeaService.postGenerateIdea(this.message).subscribe(
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
