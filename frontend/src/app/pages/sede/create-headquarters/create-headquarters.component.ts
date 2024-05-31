import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { NgToastService } from 'ng-angular-popup';
import ValidateForm from 'src/app/helpers/validateForm';
import { AuthService } from 'src/app/services/auth.service';
import { OrganizationService } from 'src/app/services/organization.service';
import { HeadquartersService } from 'src/app/services/headquarters.service';
import { Province, Provinces } from 'src/app/interfaces/provinces.interface';
import { MapService } from 'src/app/services/map.service';

@Component({
  selector: 'app-create-headquarters',
  templateUrl: './create-headquarters.component.html',
  styleUrls: ['./create-headquarters.component.css']
})
export class CreateHeadquartersComponent implements OnInit {
  provinces: Province[] = [];
  HeadquartersForm!: FormGroup;
  organization: any;
  cuit!: string;
  orgName: any;

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private organizationService: OrganizationService,
    private headquartersService: HeadquartersService,
    private mapService:MapService,
    private authService: AuthService,
    private toast: NgToastService,
  ) {
    this.HeadquartersForm = this.fb.group({
      sedes: this.fb.array([this.createHeadquartersForm()]),
    });
  }

  ngOnInit(): void {
    this.organizationService.getOrgNameFromStore().subscribe((val) => {
      const orgNameFromToken = this.authService.getOrgNameFromToken();
      this.orgName = val || orgNameFromToken;
    });

    this.organizationService.getCuitFromStore().subscribe((val) => {
      const cuitFromToken = this.authService.getCuitFromToken();
      this.cuit = val || cuitFromToken;
    });

    this.organizationService.getOrganizationByCuit(this.cuit).subscribe(
      (data) => {
        this.organization = data;
        console.log(this.organization);
      },
      (error) => {
        console.error(error);
      }
    );

    this.mapService.getProvinces().subscribe(
      (data:Provinces) => {
        this.provinces = data.provincias
          .filter((province:Province) => 
            province.nombre.toLowerCase() !== 'ciudad autónoma de buenos aires' &&
            province.nombre.toLowerCase() !== 'tierra del fuego, antártida e islas del atlántico sur'
          )
          .sort((a:Province, b:Province) => a.nombre.localeCompare(b.nombre));
        }
      );
  }

  get headquarters(): FormArray {
    return this.HeadquartersForm.get('sedes') as FormArray;
  }

  createHeadquartersForm(): FormGroup {
    return this.fb.group({
      nombre: ['', Validators.required],
      direccion: ['', Validators.required],
      localidad: ['', Validators.required],
      provincia: ['', Validators.required],
      telefono: ['', [Validators.required, Validators.pattern(/^[0-9]*$/)]],
    });
  }

  addingOtherHeadquarters(): void {
    if(this.headquarters.length < 4){
      this.headquarters.push(this.createHeadquartersForm());
    }
  }

  deleteHeadquarters(index: number): void {
    if (this.headquarters.length > 1) {
      this.headquarters.removeAt(index);
    }
  }

  saveHeadquarters(): void {
    if (this.HeadquartersForm.invalid) {
      ValidateForm.validateAllFormFileds(this.HeadquartersForm);
      return;
    }

    const headquartersToSave = this.HeadquartersForm.value.sedes.map((sede: any) => ({
      ...sede,
      telefono: sede.telefono.toString(),
      organizacionId: this.organization.id
    }));

    this.headquartersService.postHeadquarters(headquartersToSave).subscribe(
      (data) => {
        console.log(headquartersToSave.length);
        if(headquartersToSave.length > 1){
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

        this.HeadquartersForm = this.fb.group({
          sedes: this.fb.array([this.createHeadquartersForm()]),
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
