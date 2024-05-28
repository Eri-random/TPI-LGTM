import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { NgToastService } from 'ng-angular-popup';
import ValidateForm from 'src/app/helpers/validateForm';
import { AuthService } from 'src/app/services/auth.service';
import { OrganizacionService } from 'src/app/services/organizacion.service';
import { SedeService } from 'src/app/services/sede.service';

@Component({
  selector: 'app-editar-sede',
  templateUrl: './editar-sede.component.html',
  styleUrls: ['./editar-sede.component.css'],
})
export class EditarSedeComponent implements OnInit {
  sedeId: number = 0;
  sedeForm!: FormGroup;
  orgNombre: any;
  organizacion: any;

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private route: ActivatedRoute,
    private toast: NgToastService,
    private organizacionService: OrganizacionService,
    private authService: AuthService,
    private sedeService: SedeService
  ) {
    this.sedeForm = this.fb.group({
      nombre: ['', Validators.required],
      direccion: ['', Validators.required],
      localidad: ['', Validators.required],
      provincia: ['', Validators.required],
      telefono: ['', Validators.required],
    });
  }

  ngOnInit(): void {
    this.organizacionService.getOrgNameFromStore().subscribe((val) => {
      const orgNameFromToken = this.authService.getOrgNameFromToken();
      this.orgNombre = val || orgNameFromToken;
    });

    const cuitFromToken = this.authService.getCuitFromToken();

    this.organizacionService.getOrganizacionByCuit(cuitFromToken).subscribe(
      (data) => {
        this.organizacion = data;
      },
      (error) => {
        console.error(error);
      }
    );

    this.sedeId = this.route.snapshot.params['id'];

    this.sedeService.getSedeById(this.sedeId).subscribe(
      (sede) => {
        this.sedeForm.patchValue(sede);
      },
      (error) => {
        console.error('Error:', error);
      }
    );
  }

  submitForm() {
    if (this.sedeForm.invalid) {
      ValidateForm.validateAllFormFileds(this.sedeForm);
      return;
    }

    this.sedeId = Number(this.sedeId);
    this.sedeForm.value.id = this.sedeId;
    this.sedeForm.value.organizacionId = this.organizacion.id;

    this.sedeService.updateSede(this.sedeForm.value).subscribe((response) => {
      console.log(response);
      console.log(this.sedeForm.value);
      this.toast.success({
        detail: 'EXITO',
        summary: `${this.sedeForm.value.nombre} actualizada correctamente`,
        duration: 3000,
        position: 'topRight',
      });

      // this.router.navigate(['/sedes']);
    }, error => {
      this.toast.error({
        detail: 'ERROR',
        summary: 'No se pudo actualizar la sede',
        duration: 3000,
        position: 'topRight',
      });
    });
  }
}
