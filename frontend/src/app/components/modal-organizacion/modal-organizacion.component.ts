import { Component, OnInit } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import ValidateForm from 'src/app/helpers/validateForm';

@Component({
  selector: 'app-modal-organizacion',
  templateUrl: './modal-organizacion.component.html',
  styleUrls: ['./modal-organizacion.component.css']
})
export class ModalOrganizacionComponent implements OnInit{
  organizationForm: FormGroup;
  imageSrc: string | ArrayBuffer | null = "https://media.istockphoto.com/id/1226328537/es/vector/soporte-de-posici%C3%B3n-de-imagen-con-un-icono-de-c%C3%A1mara-gris.jpg?s=612x612&w=0&k=20&c=8igCt_oe2wE-aP0qExUDfwicSNUCb4Ho9DiKCq0rSaA=";

  constructor(private fb: FormBuilder, public dialogRef: MatDialogRef<ModalOrganizacionComponent>) {
    this.organizationForm = this.fb.group({
      organizacion: [{ value: '', disabled: true }, [Validators.required, Validators.maxLength(30)]],
      descripcionBreve: ['', [Validators.required, Validators.maxLength(150)]],
      descripcionCompleta: ['', [Validators.required, Validators.maxLength(4000)]],
      imagen: [null, Validators.required]
    });
  }

  ngOnInit(){
    this.organizationForm.get('organizacion')?.setValue('Organización');
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files[0]) {
      const file = input.files[0];
      
      const reader = new FileReader();
      reader.onload = (e) => {
        this.imageSrc = e.target!.result;
        this.organizationForm.patchValue({ imagen: file });
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
    console.log(this.organizationForm.value);
  }

  close(): void {
    this.dialogRef.close();
  }

}
