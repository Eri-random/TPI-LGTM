import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { NgToastService } from 'ng-angular-popup';
import ValidateForm from 'src/app/helpers/validateForm';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit{


  type:string = "password";
  eyeIcon:string= "fa-eye-slash"
  isText:boolean = false;
  loginForm!: FormGroup;
  error:string|null = null;

  constructor(private fb:FormBuilder,
    private authService:AuthService,
    private router:Router,
    private toast: NgToastService,
  ){

  }

  ngOnInit(): void {
    this.loginForm = this.fb.group({
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
    if(this.loginForm.invalid) {
      ValidateForm.validateAllFormFileds(this.loginForm);
      return;
    } 

    this.authService.login(this.loginForm.value)
    .subscribe({
      next:()=>{
      this.loginForm.reset();
      this.toast.success({detail:"EXITO",summary:'Login exitoso',duration:5000});
      },
      error:({error})=>{
        this.error = error;
      }
    })
  }
}
