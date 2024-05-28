import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { NgToastService } from 'ng-angular-popup';
import ValidateForm from 'src/app/helpers/validateForm';
import { AuthService } from 'src/app/services/auth.service';
import { OrganizacionService } from 'src/app/services/organizacion.service';
import { SedeService } from 'src/app/services/sede.service';

@Component({
  selector: 'app-crear-sede',
  templateUrl: './crear-sede.component.html',
  styleUrls: ['./crear-sede.component.css']
})
export class CrearSedeComponent implements OnInit {
  sedeForm!: FormGroup;
  organizacion: any;
  cuit!: string;
  orgNombre: any;

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private organizacionService: OrganizacionService,
    private sedeService: SedeService,
    private authService: AuthService,
    private toast: NgToastService,
  ) {
    this.sedeForm = this.fb.group({
      sedes: this.fb.array([this.crearSedeForm()]),
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
        console.log(this.organizacion);
      },
      (error) => {
        console.error(error);
      }
    );
  }

  get sedes(): FormArray {
    return this.sedeForm.get('sedes') as FormArray;
  }

  crearSedeForm(): FormGroup {
    return this.fb.group({
      nombre: ['', Validators.required],
      direccion: ['', Validators.required],
      localidad: ['', Validators.required],
      provincia: ['', Validators.required],
      telefono: ['', [Validators.required, Validators.pattern(/^[0-9]*$/)]],
    });
  }

  agregarOtraSede(): void {
    if(this.sedes.length < 4){
      this.sedes.push(this.crearSedeForm());
    }
  }

  eliminarSede(index: number): void {
    if (this.sedes.length > 1) {
      this.sedes.removeAt(index);
    }
  }

  guardarSedes(): void {
    if (this.sedeForm.invalid) {
      ValidateForm.validateAllFormFileds(this.sedeForm);
      return;
    }

    const sedesToSave = this.sedeForm.value.sedes.map((sede: any) => ({
      ...sede,
      telefono: sede.telefono.toString(),
      organizacionId: this.organizacion.id
    }));

    this.sedeService.postSede(sedesToSave).subscribe(
      (data) => {
        console.log(sedesToSave.length);
        if(sedesToSave.length >= 1){
          this.toast.success({
            detail: 'EXITO',
            summary: 'Sedes guardadas con éxito',
            duration: 3000,
            position: 'topRight',
          });
        }else{
          this.toast.success({
            detail: 'EXITO',
            summary: 'Sede guardada con éxito',
            duration: 3000,
            position: 'topRight',
          });
        }

        this.sedeForm = this.fb.group({
          sedes: this.fb.array([this.crearSedeForm()]),
        });
      },
      (error) => {
        this.toast.error({
          detail: 'ERROR',
          summary: 'Ocurrió un error al procesar la solicitud!',
          duration: 3000,
          position: 'topRight',
        });
      }
    );
  }
}
