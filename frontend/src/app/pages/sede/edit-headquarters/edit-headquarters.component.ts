import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { NgToastService } from 'ng-angular-popup';
import ValidateForm from 'src/app/helpers/validateForm';
import { AuthService } from 'src/app/services/auth.service';
import { OrganizationService } from 'src/app/services/organization.service';
import { HeadquartersService } from 'src/app/services/headquarters.service';
import { Province, Provinces } from 'src/app/interfaces/provinces.interface';
import { MapService } from 'src/app/services/map.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-edit-headquarters',
  templateUrl: './edit-headquarters.component.html',
  styleUrls: ['./edit-headquarters.component.css'],
})
export class EditHeadquartersComponent implements OnInit {
  provinces: Province[] = [];
  headquartersId: number = 0;
  headquartersForm!: FormGroup;
  orgName: any;
  organization: any;
  cuit: any;
  localidades: any[] = [];

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private route: ActivatedRoute,
    private toast: NgToastService,
    private organizationService: OrganizationService,
    private mapService:MapService,
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

    this.organizationService.getCuitFromStore().subscribe((val) => {
      const cuitFromToken = this.authService.getCuitFromToken();
      this.cuit = val || cuitFromToken;
    });

    this.organizationService.getOrganizationByCuit(this.cuit).subscribe(
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
        // Primero cargar las localidades de la provincia seleccionada
        this.loadLocalidades(sede.provincia).subscribe(() => {
          // Luego asignar los valores al formulario
          this.headquartersForm.patchValue({
            nombre: sede.nombre,
            direccion: sede.direccion,
            localidad: sede.localidad,
            provincia: sede.provincia,
            telefono: sede.telefono,
          });
          console.log(this.headquartersForm.value);

        });
      },
      (error) => {
        console.error('Error:', error);
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

  loadLocalidades(provinceId: number) {
    return new Observable((observer) => {
      this.mapService.getLocalities(provinceId).subscribe(
        (response: any) => {
          const totalLocalidades = response.total;
          this.mapService.getLocalitiesFilter(provinceId, totalLocalidades).subscribe(
            (response: any) => {
              let localidades = response.localidades;
              this.localidades = localidades.sort((a: any, b: any) =>
                a.nombre.localeCompare(b.nombre)
              );
              observer.next();
              observer.complete();
            },
            (error) => {
              observer.error(error);
            }
          );
        },
        (error) => {
          observer.error(error);
        }
      );
    });
  }

  onProvinceChange(): void {
    const provinceId = this.headquartersForm.get('provincia')?.value;

    // Primera solicitud para obtener el total de localidades
    this.mapService.getLocalities(provinceId).subscribe(
      (response: any) => {
        const totalLocalidades = response.total;

        // Segunda solicitud para obtener todas las localidades utilizando el total en el parámetro max
        this.mapService.getLocalitiesFilter(provinceId, totalLocalidades).subscribe(
          (response: any) => {
            let localidades = response.localidades;

            // Filtrar "Ciudad de Buenos Aires" si la provincia seleccionada es Buenos Aires
            if (provinceId == '06') { // Asumiendo que '06' es el ID de la provincia de Buenos Aires
              localidades = localidades.filter(
                (localidad: any) =>
                  localidad.nombre.toLowerCase() !== 'ciudad de buenos aires'
              );
            }

            // Ordenar alfabéticamente las localidades
            this.localidades = localidades.sort((a: any, b: any) =>
              a.nombre.localeCompare(b.nombre)
            );
          },
          (error) => {
            console.error(error);
          }
        );
      },
      (error) => {
        console.error(error);
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
