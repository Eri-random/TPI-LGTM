import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { NgToastService } from 'ng-angular-popup';
import ValidateForm from 'src/app/helpers/validateForm';
import { AuthService } from 'src/app/services/auth.service';
import { OrganizationService } from 'src/app/services/organization.service';
import { UserStoreService } from 'src/app/services/user-store.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
})
export class LoginComponent implements OnInit {
  type: string = 'password';
  eyeIcon: string = 'fa-eye-slash';
  isText: boolean = false;
  loginForm!: FormGroup;
  error: string | null = null;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private userStore: UserStoreService,
    private organizacionService: OrganizationService,
    private router: Router,
    private toast: NgToastService
  ) {}

  ngOnInit(): void {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required],
    });
  }

  hideShowPass() {
    this.isText = !this.isText;
    this.isText ? (this.eyeIcon = 'fa-eye') : (this.eyeIcon = 'fa-eye-slash');
    this.isText ? (this.type = 'text') : (this.type = 'password');
  }

  onSubmit() {
    if (this.loginForm.invalid) {
      ValidateForm.validateAllFormFileds(this.loginForm);
      return;
    }

    this.authService.login(this.loginForm.value).subscribe({
      next: (res) => {
        this.error = null;
        this.loginForm.reset();

        this.authService.storeToken(res.token);
        let tokenPayload = this.authService.decodedToken();
        console.log(tokenPayload);

        this.userStore.setFullNameForStore(tokenPayload.name);
        this.userStore.setRolForStore(tokenPayload.role);
        this.userStore.setEmailForStore(tokenPayload.email);
        this.authService.setIsLoggedIn(true);

        this.toast.success({
          detail: 'EXITO',
          summary: 'Login exitoso',
          duration: 5000,
          position: 'topRight',
        });
        
        tokenPayload.role === 'organizacion' 
        ? (() => {
        this.router.navigate(['/dashboard']);
        this.organizacionService.setCuitForStore(tokenPayload.cuit);
        this.organizacionService.setOrgNameForStore(tokenPayload.orgName);
        })() 
        : this.router.navigate(['/']);
      },
      error: ( error ) => {
        this.error = 'Usuario y/o contraseña erróneo';
        console.log(error)
      },
    });
  }
}
