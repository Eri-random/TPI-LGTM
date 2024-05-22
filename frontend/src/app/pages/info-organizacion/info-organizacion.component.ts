import { AfterViewInit, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { NgToastService } from 'ng-angular-popup';
import ValidateForm from 'src/app/helpers/validateForm';
import { AuthService } from 'src/app/services/auth.service';
import { DonacionService } from 'src/app/services/donacion.service';
import { OrganizacionService } from 'src/app/services/organizacion.service';
import { UserStoreService } from 'src/app/services/user-store.service';

@Component({
  selector: 'app-info-organizacion',
  templateUrl: './info-organizacion.component.html',
  styleUrls: ['./info-organizacion.component.css']
})
export class InfoOrganizacionComponent implements OnInit {
  donarForm!: FormGroup;
  organizacion:any;
  usuario:any;
  safeContent!: SafeHtml;
  email!:string;

  constructor(private formBuilder: FormBuilder,
    private organizacionService:OrganizacionService,
    private authService:AuthService,
    private userStore:UserStoreService,
    private sanitizer: DomSanitizer,
    private donacionService:DonacionService,
    private toast: NgToastService,
    private route: ActivatedRoute,
  ) {
  }

  ngOnInit(): void {
    this.donarForm = this.formBuilder.group({
      nombre: [{value:"",disabled:true}, [Validators.required]],
      apellido: [{value:"",disabled:true}, [Validators.required]],
      telefono: [{value:"",disabled:true}, [Validators.required]],
      email: [{value:"",disabled:true},
        [Validators.required,
        Validators.pattern(/^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$/)
        ]],
      producto: ["", [Validators.required]],
      cantidad: ["", [Validators.required]]
    });

    this.route.params.subscribe((params) => {
      const organizacionId = params['id'];
      this.organizacionService.getOrganizacionById(organizacionId).subscribe(
        (data) => {
          this.safeContent = this.sanitizeContent(data.infoOrganizacion.descripcionCompleta);
          this.organizacion = data;
        },
        (error) => {
          console.error(error);
        }
      );
    });

    this.email = this.authService.getEmailFromToken();
        
    this.userStore.getUserByEmail(this.email).subscribe(resp =>{
      this.usuario = resp;
      this.donarForm.get('nombre')?.setValue(resp.nombre);
      this.donarForm.get('apellido')?.setValue(resp.apellido);
      this.donarForm.get('telefono')?.setValue(resp.telefono);
      this.donarForm.get('email')?.setValue(resp.email);
    })
  }

  sanitizeContent(content: string): SafeHtml {
    return this.sanitizer.bypassSecurityTrustHtml(content);
  }

  get fm() {
    return this.donarForm.controls;
  }

  enviarFormDonacion() {
    if (!this.donarForm.valid) {
      ValidateForm.validateAllFormFileds(this.donarForm);
      console.log("SIN DATOS");
      return;
    }

    console.log(this.donarForm.value);
  }

  donar(){
    if (this.donarForm.invalid) {
      ValidateForm.validateAllFormFileds(this.donarForm);
      return;
    }

    this.donacionService.postGuardarDonacion({
        producto: this.donarForm.value.producto,
        cantidad: this.donarForm.value.cantidad,
        usuarioId: this.usuario.id,
        organizacionId: this.organizacion.id
    })
      .subscribe(resp =>{
        this.toast.success({
          detail: 'EXITO',
          summary: 'Muchas Gracias por la ayuda!',
          duration: 5000,
          position: 'topRight',
        });
      },error=>{
        this.toast.error({
          detail: 'ERROR',
          summary: 'Ocurrió un error al procesar la donación!',
          duration: 5000,
          position: 'topRight',
        });
      })
  }
}
