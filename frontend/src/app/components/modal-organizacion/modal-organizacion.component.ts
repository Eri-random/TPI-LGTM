import { Component, OnInit } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import ValidateForm from 'src/app/helpers/validateForm';
import { OrganizacionService } from 'src/app/services/organizacion.service';
import { AuthService } from 'src/app/services/auth.service';
import { catchError } from 'rxjs/operators';
import { throwError } from 'rxjs';
import { HttpErrorResponse } from '@angular/common/http';
import { NgToastService } from 'ng-angular-popup';

@Component({
  selector: 'app-modal-organizacion',
  templateUrl: './modal-organizacion.component.html',
  styleUrls: ['./modal-organizacion.component.css'],
})
export class ModalOrganizacionComponent implements OnInit {
  organizationForm: FormGroup;
  cuit!: string;
  imageSrc: string | ArrayBuffer | null =
    'https://media.istockphoto.com/id/1226328537/es/vector/soporte-de-posici%C3%B3n-de-imagen-con-un-icono-de-c%C3%A1mara-gris.jpg?s=612x612&w=0&k=20&c=8igCt_oe2wE-aP0qExUDfwicSNUCb4Ho9DiKCq0rSaA=';

  constructor(
    private fb: FormBuilder,
    public dialogRef: MatDialogRef<ModalOrganizacionComponent>,
    private authService: AuthService,
    private organizacionService: OrganizacionService,
    private toast: NgToastService
  ) {
    this.organizationForm = this.fb.group({
      organizacion: ['', [Validators.required, Validators.maxLength(30)]],
      descripcionBreve: ['', [Validators.required, Validators.maxLength(150)]],
      descripcionCompleta: [
        '',
        [Validators.required, Validators.maxLength(4000)],
      ],
      img: [null, Validators.required],
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
        this.organizationForm.get('organizacion')?.setValue(resp.nombre);
        this.organizationForm.get('organizacionId')?.setValue(resp.id);
      });
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files[0]) {
      const file = input.files[0];

      const reader = new FileReader();
      reader.onload = (e) => {
        this.imageSrc = e.target!.result;
        this.organizationForm.patchValue({ img: e.target!.result  });
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

    this.organizacionService
      .postInfoOrganizacion(this.organizationForm.value)
      .pipe(
        catchError((error: HttpErrorResponse) => {
          if (error.status === 404) {
            console.error('Error 404: Recurso no encontrado');
            this.toast.error({
              detail: 'Ocurrió un error al intentar guardar la información. Intente nuevamente.',
              duration: 5000,
              position: 'topCenter',
            });
          } else {
            console.error('Error:', error.message);
            this.toast.error({
              detail: 'Ocurrió un error al intentar guardar la información. Intente nuevamente.',
              duration: 5000,
              position: 'topCenter',
            });
          }
          return throwError(error);
        })
      )
      .subscribe((resp) => {
        console.log('Respuesta:', resp);
        this.toast.success({
          detail: 'La información se guardó correctamente.',
          duration: 5000,
          position: 'topCenter',
        });
        this.close();
      });
  }

  close(): void {
    this.dialogRef.close();
  }
}
