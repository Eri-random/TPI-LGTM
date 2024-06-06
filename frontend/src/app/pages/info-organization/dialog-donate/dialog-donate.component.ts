import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { NgToastService } from 'ng-angular-popup';
import { map, switchMap } from 'rxjs';
import ValidateForm from 'src/app/helpers/validateForm';
import { AuthService } from 'src/app/services/auth.service';
import { OrganizationService } from 'src/app/services/organization.service';
import { HeadquartersService } from 'src/app/services/headquarters.service';
import { UserStoreService } from 'src/app/services/user-store.service';
import { DonationsService } from 'src/app/services/donations.service';

@Component({
  selector: 'app-dialog-donate',
  templateUrl: './dialog-donate.component.html',
  styleUrls: ['./dialog-donate.component.css'],
})
export class DialogDonateComponent implements OnInit {
  donateForm!: FormGroup;
  email!: string;
  user: any;
  dataDirection: any;
  headquarters: any;
  isSubmitted: boolean = false;
  cuit !: string;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: { organizacionId: number },
    public dialogRef: MatDialogRef<DialogDonateComponent>,
    private formBuilder: FormBuilder,
    private donateService: DonationsService,
    private authService: AuthService,
    private userStore: UserStoreService,
    private headquarterService: HeadquartersService,
    private organizationService: OrganizationService,
    private toast: NgToastService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.donateForm = this.formBuilder.group({
      producto: ['', [Validators.required]],
      cantidad: ['', [Validators.required]],
    });

    this.userStore.getEmailFromStore().subscribe((val) => {
      const emailFromToken = this.authService.getEmailFromToken();
      this.email = val || emailFromToken;
    });

    this.userStore.getUserByEmail(this.email).subscribe((resp) => {
      this.user = resp;
    });

    this.organizationService.getOrganizationById(this.data.organizacionId).subscribe((resp) => {
      this.cuit = resp.cuit;
    });
  }

  get fm() {
    return this.donateForm.controls;
  }

  donate() {
    if (this.donateForm.invalid) {
      ValidateForm.validateAllFormFileds(this.donateForm);
      return;
    }

    this.donateService
      .postSaveDonation({
        producto: this.donateForm.value.producto,
        cantidad: this.donateForm.value.cantidad,
        usuarioId: this.user.id,
        organizacionId: this.data.organizacionId,
        cuit: this.cuit,
      })
      .subscribe(
        (resp) => {
          this.isSubmitted = true;
        },
        (error) => {
          console.error('Error:', error);
        }
      );

    this.organizationService
      .getOrganizationById(this.data.organizacionId)
      .pipe(
        switchMap((org) =>
          this.headquarterService
            .getHeadquartersByOrganization(org.id)
            .pipe(map((sedes: any) => ({ organizacion: org, sedes })))
        ),
        switchMap((data) =>
          this.headquarterService.postNearestHeadquarter({
            ...data,
            usuario: this.user,
          })
        )
      )
      .subscribe(
        (nearestLocation) => {
          this.dataDirection = nearestLocation;
          console.log(this.dataDirection);

          this.headquarterService.setDataDirection(this.dataDirection);
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
