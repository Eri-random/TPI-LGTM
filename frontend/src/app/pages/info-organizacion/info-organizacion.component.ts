import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import ValidateForm from 'src/app/helpers/validateForm';

@Component({
  selector: 'app-info-organizacion',
  templateUrl: './info-organizacion.component.html',
  styleUrls: ['./info-organizacion.component.css']
})
export class InfoOrganizacionComponent {
  donarForm!: FormGroup;

  constructor(private formBuilder: FormBuilder) {

  }

  ngOnInit(): void {
    this.donarForm = this.formBuilder.group({
      nombre: ["", [Validators.required]],
      apellido: ["", [Validators.required]],
      telefono: ["", [Validators.required]],
      email: ["",
        [Validators.required,
        Validators.pattern(/^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$/)
        ]],
      producto: ["", [Validators.required]],
      cantidad: ["", [Validators.required]]
    });
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


}
