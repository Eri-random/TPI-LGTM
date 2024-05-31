import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { NgToastService } from 'ng-angular-popup';
import { map, switchMap } from 'rxjs';
import ValidateForm from 'src/app/helpers/validateForm';
import { AuthService } from 'src/app/services/auth.service';
import { DonacionService } from 'src/app/services/donacion.service';
import { OrganizacionService } from 'src/app/services/organizacion.service';
import { SedeService } from 'src/app/services/sede.service';
import { UserStoreService } from 'src/app/services/user-store.service';

@Component({
  selector: 'app-dialog-donar',
  templateUrl: './dialog-donar.component.html',
  styleUrls: ['./dialog-donar.component.css'],
})
export class DialogDonarComponent implements OnInit {
  donarForm!: FormGroup;
  email!: string;
  usuario: any;
  dataDirection: any;
  sedes: any;
  isSubmitted: boolean = false;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: { organizacionId: number },
    public dialogRef: MatDialogRef<DialogDonarComponent>,
    private formBuilder: FormBuilder,
    private donacionService: DonacionService,
    private authService: AuthService,
    private userStore: UserStoreService,
    private sedeService: SedeService,
    private organizacionService: OrganizacionService,
    private toast: NgToastService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.donarForm = this.formBuilder.group({
      producto: ['', [Validators.required]],
      cantidad: ['', [Validators.required]],
    });

    this.userStore.getEmailFromStore()
    .subscribe(val =>{
      const emailFromToken = this.authService.getEmailFromToken();
      this.email = val || emailFromToken;
    });

    this.userStore.getUserByEmail(this.email).subscribe((resp) => {
      this.usuario = resp;
    });
  }

  get fm() {
    return this.donarForm.controls;
  }

  donar() {
    if (this.donarForm.invalid) {
      ValidateForm.validateAllFormFileds(this.donarForm);
      return;
    }

    this.donacionService
      .postGuardarDonacion({
        producto: this.donarForm.value.producto,
        cantidad: this.donarForm.value.cantidad,
        usuarioId: this.usuario.id,
        organizacionId: this.data.organizacionId,
      })
      .subscribe(
        (resp) => {
          // this.toast.success({
          //   detail: 'EXITO',
          //   summary: 'Muchas Gracias por la ayuda!',
          //   duration: 5000,
          //   position: 'topRight',
          // });
          // this.close();
          this.isSubmitted = true;
        },
        (error) => {
          // this.toast.error({
          //   detail: 'ERROR',
          //   summary: 'Ocurrió un error al procesar la donación!',
          //   duration: 5000,
          //   position: 'topRight',
          // });
        }
      );

      this.organizacionService
      .getOrganizacionById(this.data.organizacionId)
      .pipe(
        switchMap((org) =>
          this.sedeService.getSedesByOrganization(org.id).pipe(
            map((sedes: any) => ({ organizacion: org, sedes }))
          )
        ),
        switchMap((data) => 
          this.sedeService.postSedeMasCercana({
            ...data,
            usuario: this.usuario
          })
        )
      )
      .subscribe(
        (sedeMasCercana) => {
          this.dataDirection = sedeMasCercana;
          console.log(this.dataDirection);

          this.sedeService.setDataDirection(this.dataDirection);

        },
        (error) => {
          console.error('Error:', error);
        }
      );
  }


  close(): void {
    this.dialogRef.close();
  }
}
