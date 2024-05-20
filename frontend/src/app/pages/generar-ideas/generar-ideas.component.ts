import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import {Router} from "@angular/router";
import { NgToastService } from 'ng-angular-popup';
import ValidateForm from 'src/app/helpers/validateForm';
import { AuthService } from 'src/app/services/auth.service';
import { ReconocimientoTelaService } from 'src/app/services/reconocimiento-tela.service';

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

  constructor( private formBuilder: FormBuilder, private router:Router,
    private reconomientoTelaService: ReconocimientoTelaService,
    private authService: AuthService,
    private toast: NgToastService
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

    /*Reconocimiento momentanea de una imagen, ver de mejorar*/
    this.reconomientoTelaService.classifyImage(files[0])
      .subscribe(({tela}) =>{
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

    console.log(this.imageFiles);
    console.log(this.ideaForm.value);
  }

}
