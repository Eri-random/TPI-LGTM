import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { NgToastService } from 'ng-angular-popup';
import { catchError, throwError } from 'rxjs';
import ValidateForm from 'src/app/helpers/validateForm';
import { AuthService } from 'src/app/services/auth.service';
import { OrganizationService } from 'src/app/services/organization.service';
import { v4 as uuidv4 } from 'uuid';

@Component({
  selector: 'app-my-organization',
  templateUrl: './my-organization.component.html',
  styleUrls: ['./my-organization.component.css']
})
export class MyOrganizationComponent implements OnInit {
  organizationForm: FormGroup;
  cuit!: string;
  imageSrc: string = '/assets/img/logo-placeholder.png';
  isEditMode: boolean = false;
  placeholderImage: string = '/assets/img/logo-placeholder.png';
  uniqueId: string = uuidv4();
  
  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private organizacionService: OrganizationService,
    private toast: NgToastService,
    private router: Router
  ) {
    this.organizationForm = this.fb.group({
      organizacion: ['', [Validators.required, Validators.maxLength(30)]],
      descripcionBreve: ['', [Validators.required, Validators.maxLength(150)]],
      descripcionCompleta: ['', [Validators.required, Validators.maxLength(4000)]],
      imageUrl: ['', Validators.required],
      organizacionId: [null],
    });
  }

  ngOnInit() {
    this.organizacionService.getCuitFromStore().subscribe((val) => {
      const cuitFromToken = this.authService.getCuitFromToken();
      this.cuit = val || cuitFromToken;
    });

    this.organizacionService.getOrganizationByCuit(this.cuit).subscribe((resp) => {
      this.organizationForm.get('organizacion')?.setValue(resp.nombre);
      this.organizationForm.get('organizacionId')?.setValue(resp.id);

      if (resp.infoOrganizacion != null) {
        this.isEditMode = true;
        this.organizationForm.get('descripcionBreve')?.setValue(resp.infoOrganizacion.descripcionBreve);
        this.organizationForm.get('descripcionCompleta')?.setValue(resp.infoOrganizacion.descripcionCompleta);
        if (resp.infoOrganizacion.img) {
          this.imageSrc = resp.infoOrganizacion.img;
          this.organizationForm.get('imageUrl')?.setValue(resp.infoOrganizacion.img);
        } else {
          this.imageSrc = this.placeholderImage;
        }
      }
    });
  }

  onImageUrlInput(): void {
    const imageUrl = this.organizationForm.get('imageUrl')?.value;
    this.imageSrc = imageUrl ? imageUrl : this.placeholderImage;
  }

  onError(): void {
    this.imageSrc = this.placeholderImage;
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

    const formData: any = {
      organizacion: this.organizationForm.get('organizacion')?.value || '',
      descripcionBreve: this.organizationForm.get('descripcionBreve')?.value || '',
      descripcionCompleta: this.organizationForm.get('descripcionCompleta')?.value || '',
      imageUrl: this.organizationForm.get('imageUrl')?.value || '',
      organizacionId: this.organizationForm.get('organizacionId')?.value || ''
    };

    if (this.isEditMode) {
      this.organizacionService.putInfoOrganization(formData).pipe(
        catchError((error: HttpErrorResponse) => {
          this.handleError(error);
          return throwError(error);
        })
      ).subscribe((resp) => {
        this.handleSuccess();
      });
    } else {
      this.organizacionService.postInfoOrganization(formData).pipe(
        catchError((error: HttpErrorResponse) => {
          this.handleError(error);
          return throwError(error);
        })
      ).subscribe((resp) => {
        this.handleSuccess();
        this.router.navigate(['/dashboard']);
      });
    }
  }

  private handleError(error: HttpErrorResponse) {
    if (error.status === 404) {
      console.error('Error 404: Recurso no encontrado');
      this.toast.error({
        detail: 'Ocurrió un error al intentar guardar la información. Intente nuevamente.',
        duration: 5000,
        position: 'topRight',
      });
    } else {
      console.error('Error:', error.message);
      this.toast.error({
        detail: 'Ocurrió un error al intentar guardar la información. Intente nuevamente.',
        duration: 5000,
        position: 'topRight',
      });
    }
  }

  private handleSuccess() {
    this.toast.success({
      detail: 'La información se guardó correctamente.',
      duration: 5000,
      position: 'topRight',
    });
  }
}
