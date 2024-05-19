import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import ValidateForm from 'src/app/helpers/validateForm';
import { OrganizacionService } from 'src/app/services/organizacion.service';
import { AuthService } from 'src/app/services/auth.service';
import { NgToastService } from 'ng-angular-popup';
import { Router } from '@angular/router';

@Component({
  selector: 'app-edit-info',
  templateUrl: './edit-info.component.html',
  styleUrls: ['./edit-info.component.css'],
})
export class EditInfoComponent implements OnInit {
  organizationForm: FormGroup;
  cuit!: string;
  imageSrc: string | ArrayBuffer | null =
    'https://media.istockphoto.com/id/1226328537/es/vector/soporte-de-posici%C3%B3n-de-imagen-con-un-icono-de-c%C3%A1mara-gris.jpg?s=612x612&w=0&k=20&c=8igCt_oe2wE-aP0qExUDfwicSNUCb4Ho9DiKCq0rSaA=';

  selectedFile: File | null = null;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private organizacionService: OrganizacionService,
    private toast: NgToastService,
    private router: Router,
  ) {
    this.organizationForm = this.fb.group({
      organizacion: ['', [Validators.required, Validators.maxLength(30)]],
      descripcionBreve: ['', [Validators.required, Validators.maxLength(150)]],
      descripcionCompleta: [
        '',
        [Validators.required, Validators.maxLength(4000)],
      ],
      file: [null, Validators.required],
      organizacionId: [null],
    });
  }

  ngOnInit() {
    this.organizacionService.getCuitFromStore().subscribe((val) => {
      const cuitFromToken = this.authService.getCuitFromToken();
      this.cuit = val || cuitFromToken;
    });

    this.organizacionService
      .getOrganizacionByCuit(this.cuit)
      .subscribe((resp) => {
        //obtengo la imagen y la seteo en el formulario
        // this.imageSrc = resp.infoOrganizacion.img;
        this.organizationForm
          .get('organizacion')
          ?.setValue(resp.infoOrganizacion.organizacion);
        this.organizationForm
          .get('descripcionBreve')
          ?.setValue(resp.infoOrganizacion.descripcionBreve);
        this.organizationForm
          .get('descripcionCompleta')
          ?.setValue(resp.infoOrganizacion.descripcionCompleta);
        this.organizationForm.get('organizacionId')?.setValue(resp.id);
      });
  }

  onFileSelected(event: any) {
    if (event.target.files.length > 0) {
      const file = event.target.files[0];

      const reader = new FileReader();
      reader.onload = (e) => {
        this.imageSrc = e.target!.result;
        this.organizationForm.patchValue({
          file: file
        });
      };
      reader.readAsDataURL(file);
    }
  }

  getCharacterCount(fieldName: string, maxLength: number): string {
    const field = this.organizationForm.get(fieldName);
    return `${field?.value.length}/${maxLength}`;
  }

  onSubmit() {
    if (this.organizationForm.invalid) {
      ValidateForm.validateAllFormFileds(this.organizationForm);
      return;
    }

    const formData: FormData = new FormData();
    formData.append(
      'Organizacion',
      this.organizationForm.get('organizacion')?.value || ''
    );
    formData.append(
      'DescripcionBreve',
      this.organizationForm.get('descripcionBreve')?.value || ''
    );
    formData.append(
      'DescripcionCompleta',
      this.organizationForm.get('descripcionCompleta')?.value || ''
    );
    const file = this.organizationForm.get('file')?.value;
    if (file instanceof File) {
      formData.append('file', file);
    }
    formData.append(
      'OrganizacionId',
      this.organizationForm.get('organizacionId')?.value || ''
    );

    this.organizacionService.putInfoOrganizacion(formData).subscribe({
      next: (res) => {
        console.log('Respuesta:', res);
        this.toast.success({
          detail: 'EXITO',
          summary: 'Información actualizada',
          duration: 5000,
          position: 'topRight',
        });

        setTimeout(() => {
          this.router.navigate(['/dashboard']);
        }, 3000);
      },
      error: ({ error }) => {
        this.toast.error({
          detail: 'ERROR',
          summary: 'Error al actualizar la información',
          duration: 5000,
          position: 'topCenter',
        });
      },
    });
  }
}
