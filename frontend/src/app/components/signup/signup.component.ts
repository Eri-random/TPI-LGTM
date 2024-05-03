import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { NgToastService } from 'ng-angular-popup';
import ValidateForm from 'src/app/helpers/validateForm';
import { User } from 'src/app/models/user';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-signup',
  templateUrl: './signup.component.html',
  styleUrls: ['./signup.component.css']
})
export class SignupComponent {
  type:string = "password";
  eyeIcon:string= "fa-eye-slash"
  isText:boolean = false;
  registerForm!: FormGroup;

  constructor(private fb:FormBuilder,
    private authService:AuthService,
    private router:Router,
    private toast: NgToastService
  ){

  }

  ngOnInit(): void {
    this.registerForm = this.fb.group({
      nombre:['',[Validators.required]],
      apellido:['',Validators.required],
      telefono:[null,[Validators.minLength(10), Validators.maxLength(10)]],
      direccion:['',Validators.required],
      localidad:['',Validators.required],
      provincia:['',Validators.required],
      email: ['',[Validators.required,Validators.email]],
      password: ['',Validators.required]
    })
  }

  hideShowPass(){
    this.isText = !this.isText;
    this.isText ? this.eyeIcon = "fa-eye" : this.eyeIcon = "fa-eye-slash";
    this.isText ? this.type = "text" : this.type = "password";
  }

  onSubmit(){
    if(this.registerForm.invalid) {
      ValidateForm.validateAllFormFileds(this.registerForm);
      return;
    } 

    const newUsuario:User = this.registerForm.value;
    newUsuario.rolId = 2;

    console.log(newUsuario);
    this.authService.crearCuenta(newUsuario)
    .subscribe({
      next:()=>{
      this.registerForm.reset();
      this.toast.success({detail:"EXITO",summary:'Usuario registrado correctamente',duration:5000});
      this.router.navigate(['/login'])
      },
      error:(error)=>{
      this.toast.error({detail:"ERROR",summary:error.error,duration:5000,position:'topCenter'});
      }
    })

  }
}
