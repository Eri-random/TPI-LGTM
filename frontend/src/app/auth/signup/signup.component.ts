import { Component } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ValidationErrors, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { NgToastService } from 'ng-angular-popup';
import { Observable } from 'rxjs';
import ValidateForm from 'src/app/helpers/validateForm';
import { Province, Provinces } from 'src/app/interfaces/provinces.interface';
import { User } from 'src/app/models/user';
import { AuthService } from 'src/app/services/auth.service';
import { MapService } from 'src/app/services/map.service';
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

  provinces: Province[] = [];
  localidades: any[] = [];

  constructor(private fb:FormBuilder,
    private authService:AuthService,
    private router:Router,
    private mapService:MapService,
    private toast: NgToastService
  ){

  }

  ngOnInit(): void {
    this.registerForm = this.fb.group({
      nombre:['',[Validators.required]],
      apellido:[null],
      telefono:[null,[Validators.minLength(8), Validators.maxLength(10)]],
      direccion:['',Validators.required],
      localidad:['',Validators.required],
      provincia:['',Validators.required],
      cuit:[null],
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

    this.mapService.getProvinces().subscribe((data: Provinces) => {
      this.provinces = data.provincias
        .filter(
          (province: Province) =>
            province.nombre.toLowerCase() !== 'ciudad autónoma de buenos aires' &&
            province.nombre.toLowerCase() !== 'tierra del fuego, antártida e islas del atlántico sur'
        )
        .sort((a: Province, b: Province) => a.nombre.localeCompare(b.nombre));
    });
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
    this.selectedRole == 'usuario' ? newUsuario.rolId = Roles.User : newUsuario.rolId = Roles.Organization;


    this.authService.createAccount(newUsuario)
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


  onProvinceChange(): void {
    const provinceId = this.registerForm.get('provincia')?.value;
    this.loadLocalidades(provinceId).subscribe(
      () => {
        console.log('Localidades cargadas:', this.localidades);
      },
      (error) => {
        console.error('Error cargando localidades:', error);
      }
    );
  }

  loadLocalidades(provinceId: number): Observable<any> {
    return new Observable((observer) => {
      this.mapService.getLocalities(provinceId).subscribe(
        (response: any) => {
          const totalLocalidades = response.total;
          this.mapService
            .getLocalitiesFilter(provinceId, totalLocalidades)
            .subscribe(
              (response: any) => {
                this.localidades = response.localidades.sort((a: any, b: any) =>
                  a.nombre.localeCompare(b.nombre)
                );
                observer.next();
                observer.complete();
              },
              (error) => {
                observer.error(error);
              }
            );
        },
        (error) => {
          observer.error(error);
        }
      );
    });
  }

}
