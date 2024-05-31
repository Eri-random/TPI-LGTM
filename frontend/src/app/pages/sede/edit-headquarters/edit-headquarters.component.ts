import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { NgToastService } from 'ng-angular-popup';
import ValidateForm from 'src/app/helpers/validateForm';
import { AuthService } from 'src/app/services/auth.service';
import { OrganizationService } from 'src/app/services/organization.service';
import { HeadquartersService } from 'src/app/services/headquarters.service';

@Component({
  selector: 'app-edit-headquarters',
  templateUrl: './edit-headquarters.component.html',
  styleUrls: ['./edit-headquarters.component.css'],
})
export class EditHeadquartersComponent implements OnInit {
  headquartersId: number = 0;
  headquartersForm!: FormGroup;
  orgName: any;
  organization: any;

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private route: ActivatedRoute,
    private toast: NgToastService,
    private organizationService: OrganizationService,
    private authService: AuthService,
    private headquartersService: HeadquartersService
  ) {
    this.headquartersForm = this.fb.group({
      nombre: ['', Validators.required],
      direccion: ['', Validators.required],
      localidad: ['', Validators.required],
      provincia: ['', Validators.required],
      telefono: ['', Validators.required],
    });
  }

  ngOnInit(): void {
    this.organizationService.getOrgNameFromStore().subscribe((val) => {
      const orgNameFromToken = this.authService.getOrgNameFromToken();
      this.orgName = val || orgNameFromToken;
    });

    const cuitFromToken = this.authService.getCuitFromToken();

    this.organizationService.getOrganizationByCuit(cuitFromToken).subscribe(
      (data) => {
        this.organization = data;
      },
      (error) => {
        console.error(error);
      }
    );

    this.headquartersId = this.route.snapshot.params['id'];

    this.headquartersService.getHeadquarterById(this.headquartersId).subscribe(
      (sede) => {
        this.headquartersForm.patchValue(sede);
      },
      (error) => {
        console.error('Error:', error);
      }
    );
  }

  submitForm() {
    if (this.headquartersForm.invalid) {
      ValidateForm.validateAllFormFileds(this.headquartersForm);
      return;
    }

    this.headquartersId = Number(this.headquartersId);
    this.headquartersForm.value.id = this.headquartersId;
    this.headquartersForm.value.organizacionId = this.organization.id;

    this.headquartersService.updateHeadquarters(this.headquartersForm.value).subscribe((response) => {
      console.log(response);
      console.log(this.headquartersForm.value);
      this.toast.success({
        detail: 'EXITO',
        summary: `${this.headquartersForm.value.nombre} actualizada correctamente`,
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
