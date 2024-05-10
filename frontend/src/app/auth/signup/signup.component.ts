import { Component } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ValidationErrors, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { NgToastService } from 'ng-angular-popup';
import ValidateForm from 'src/app/helpers/validateForm';
import { User } from 'src/app/models/user';
import { AuthService } from 'src/app/services/auth.service';
import { Roles } from 'src/app/utils/roles.enum';

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
  selectedRole: string | null = null;

  visibilityForm:string="d-none";
  visibilityRol:string="d-block";

  constructor(private fb:FormBuilder,
    private authService:AuthService,
    private router:Router,
    private toast: NgToastService
  ){

  }

  ngOnInit(): void {
    this.registerForm = this.fb.group({
      nombre:['',[Validators.required]],
      apellido:[''],
      telefono:[null,[Validators.minLength(8), Validators.maxLength(10)]],
      direccion:['',Validators.required],
      localidad:['',Validators.required],
      provincia:['',Validators.required],
      cuit:[''],
      email: ['',[Validators.required,Validators.email]],
      password: ['',[Validators.required,Validators.minLength(6)]]
    })

    this.registerForm.get('apellido')?.setValidators(
      (control: AbstractControl): ValidationErrors | null => {
        if (this.selectedRole == 'usuario') {
          return Validators.required(control);
        }
        return null;
      }
    );

    this.registerForm.get('cuit')?.setValidators(
      (control: AbstractControl): ValidationErrors | null => {
        if (this.selectedRole == 'organización') {
          return Validators.required(control);
        }
        return null;
      }
    );
  }

  hideShowPass(){
    this.isText = !this.isText;
    this.isText ? this.eyeIcon = "fa-eye" : this.eyeIcon = "fa-eye-slash";
    this.isText ? this.type = "text" : this.type = "password";
  }

  onInput(event: Event) {
    const inputValue = (event.target as HTMLInputElement).value.trim();
    this.registerForm.patchValue({ telefono: inputValue || null });
  }

  onSubmit(){
    if(this.registerForm.invalid) {
      ValidateForm.validateAllFormFileds(this.registerForm);
      return;
    } 

    const newUsuario:User = this.registerForm.value;
    this.selectedRole == 'usuario' ? newUsuario.rolId = Roles.Usuario : newUsuario.rolId = Roles.Organizacion;


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

  seleccionar(rol: string) {
    this.selectedRole = rol;
  }

  enableForm(){
    this.visibilityForm = "d-block"
    this.visibilityRol = "d-none"
  }

}
