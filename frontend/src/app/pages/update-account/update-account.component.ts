import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgToastService } from 'ng-angular-popup';
import { Observable, catchError, switchMap, tap, throwError } from 'rxjs';
import ValidateForm from 'src/app/helpers/validateForm';
import { Province, Provinces } from 'src/app/interfaces/provinces.interface';
import { AuthService } from 'src/app/services/auth.service';
import { DonationsService } from 'src/app/services/donations.service';
import { HeadquartersService } from 'src/app/services/headquarters.service';
import { MapService } from 'src/app/services/map.service';
import { OrganizationService } from 'src/app/services/organization.service';
import { UserStoreService } from 'src/app/services/user-store.service';

@Component({
  selector: 'app-update-account',
  templateUrl: './update-account.component.html',
  styleUrls: ['./update-account.component.css'],
})
export class UpdateAccountComponent implements OnInit {
  email = '';
  accountForm!: FormGroup;
  accountOrgForm!: FormGroup;
  provinces: Province[] = [];
  isEditMode = false;
  rolId = 0;
  idUser = 0;
  idOrg = 0;
  donations: any[] = [];
  mostDonatedProductType = '';
  mostDonatedProductQuantity = 0;
  topOrganizationId = '';
  topOrganizationDonation = 0;
  loading: boolean = true;
  role = '';
  orgName = '';
  cuit = '';
  initialAccountFormValues: any;
  initialAccountOrgFormValues: any;
  organization: any;
  headquarters: any[] = [];
  needs: any;
  totalNeeds: any;
  localidades: any[] = [];


  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private userStore: UserStoreService,
    private mapService: MapService,
    private toast: NgToastService,
    private organizationService: OrganizationService,
    private headquartersService: HeadquartersService,
    private donationService: DonationsService
  ) {
    this.accountForm = this.fb.group({
      nombre: [{ value: '', disabled: true }, Validators.required],
      apellido: [{ value: '', disabled: true }, Validators.required],
      email: [
        { value: '', disabled: true },
        [Validators.required, Validators.email],
      ],
      telefono: [{ value: '', disabled: true }, Validators.required],
      direccion: [{ value: '', disabled: true }, Validators.required],
      localidad: [{ value: '', disabled: true }, Validators.required],
      provincia: [{ value: '', disabled: true }, Validators.required],
    });

    this.accountOrgForm = this.fb.group({
      nombre: [{ value: '', disabled: true }, Validators.required],
      cuit: [{ value: '', disabled: true }, Validators.required],
      telefono: [{ value: '', disabled: true }, Validators.required],
      direccion: [{ value: '', disabled: true }, Validators.required],
      localidad: [{ value: '', disabled: true }, Validators.required],
      provincia: [{ value: '', disabled: true }, Validators.required],
    });
  }

  ngOnInit(): void {
    this.mapService.getProvinces().subscribe((data: Provinces) => {
      this.provinces = data.provincias
        .filter(
          (province: Province) =>
            province.nombre.toLowerCase() !==
              'ciudad autónoma de buenos aires' &&
            province.nombre.toLowerCase() !==
              'tierra del fuego, antártida e islas del atlántico sur'
        )
        .sort((a: Province, b: Province) => a.nombre.localeCompare(b.nombre));
    });

    this.organizationService.getOrgNameFromStore().subscribe((val) => {
      const orgNameFromToken = this.authService.getOrgNameFromToken();
      this.orgName = val || orgNameFromToken;
    });

    this.organizationService.getCuitFromStore().subscribe((val) => {
      const cuitFromToken = this.authService.getCuitFromToken();
      this.cuit = val || cuitFromToken;
    });

    this.userStore.getRolFromStore()
    .subscribe(val =>{
      const roleFromToken = this.authService.getRoleFromToken();
      this.role = val || roleFromToken;
      console.log(this.role)
    });

    if(this.role == 'organizacion'){
    this.organizationService
      .getOrganizationByCuit(this.cuit)
      .pipe(
        tap((org) => {
          this.organization = org;
        }),
        switchMap(({ id }) =>
          this.headquartersService.getHeadquartersByOrganization(id)
        )
      )
      .subscribe(
        (sedes) => {
          this.headquarters = sedes;
          setTimeout(() => {
            this.loading = false;
          }, 1000);
        },
        (error) => {
          console.error('Error:', error);
          setTimeout(() => {
            this.loading = false;
          }, 1000);
        }
      );
    }

    this.userStore.getEmailFromStore().subscribe((val) => {
      const emailFromToken = this.authService.getEmailFromToken();
      this.email = val || emailFromToken;
      if (this.role === 'usuario') {
        this.loadUserData().subscribe(() => {
          this.initialAccountFormValues = this.accountForm.getRawValue();
          this.loadDonations();
          setTimeout(() => {
            this.loading = false;
          }, 1000);
        });
      } else {
        this.loadUserData().subscribe(() => {
          this.initialAccountOrgFormValues = this.accountOrgForm.getRawValue();
          this.loadNeeds();
          setTimeout(() => {
            this.loading = false;
          }, 1000);
        });
      }
    });
  }

  onProvinceChange(): void {
    let provinceId = 0;
    if (this.role === 'usuario') {
      provinceId = this.accountForm.get('provincia')?.value;
    } else {
      provinceId = this.accountOrgForm.get('provincia')?.value;
    }
    this.loadLocalidades(provinceId).subscribe(
      () => {
        console.log('Localidades cargadas:', this.localidades);
      },
      (error) => {
        console.error('Error cargando localidades:', error);
      }
    );
  }

  loadLocalidades(provinceId: number): Observable<any> {
    return new Observable((observer) => {
      this.mapService.getLocalities(provinceId).subscribe(
        (response: any) => {
          const totalLocalidades = response.total;
          this.mapService
            .getLocalitiesFilter(provinceId, totalLocalidades)
            .subscribe(
              (response: any) => {
                this.localidades = response.localidades.sort((a: any, b: any) =>
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


  loadNeeds(){
    this.organizationService.getGroupedSubcategories(this.idOrg)
    .subscribe(resp=>{
      this.needs = resp;
      console.log(this.needs);
      let total = 0;
      this.needs.forEach((need:any) => {
        total += need.subcategoria.length;
      });

      this.totalNeeds = total;

    },error =>{
      console.log(error);
    })
  }


  loadUserData(): Observable<any> {
    return this.userStore.getUserByEmail(this.email).pipe(
      switchMap((res: any) => {
        if (res.rolId === 1) {
          return this.organizationService.getOrganizationByCuit(this.cuit).pipe(
            switchMap((org: any) => {
              this.accountOrgForm.patchValue({
                nombre: org.nombre,
                cuit: org.cuit,
                telefono: org.telefono,
                direccion: org.direccion,
                localidad: org.localidad,
                provincia: org.provincia,
              });
              console.log(this.accountOrgForm.value);
              this.rolId = res.rolId;
              this.idOrg = org.id;
              return this.loadLocalidades(org.provincia).pipe(
                tap(() => {
                  this.initialAccountOrgFormValues = this.accountOrgForm.getRawValue();
                })
              );
            })
          );
        } else {
          this.accountForm.patchValue({
            nombre: res.nombre,
            apellido: res.apellido,
            email: res.email,
            telefono: res.telefono,
            direccion: res.direccion,
            localidad: res.localidad,
            provincia: res.provincia,
          });
          this.rolId = res.rolId;
          this.idUser = res.id;
          return this.loadLocalidades(res.provincia).pipe(
            tap(() => {
              this.initialAccountFormValues = this.accountForm.getRawValue();
            })
          );
        }
      }),
      catchError((err) => {
        console.error('Error loading user data:', err);
        return throwError(err);
      })
    );
  }


  loadDonations(): void {
    this.donationService.getAllDonationsByUserId(this.idUser).subscribe({
      next: (res:any) => {
        this.donations = res;
        console.log('Donations:', this.donations);
        this.calculateMostDonatedProductType();
      },
      error: (err:any) => {
        console.error('Error loading donations:', err);
      },
    });
  }

  calculateMostDonatedProductType(): void {
    if (this.donations.length === 0) {
      this.mostDonatedProductType = 'No ha realizado donaciones';
      return;
    }

    const productCount: { [key: string]: number } = {};
    const organizationCount: { [key: string]: number } = {};

    this.donations.forEach((donation) => {
      // Normaliza el nombre del producto a su forma singular
      let productType = donation.producto.toLowerCase();
      if (productType.endsWith('s')) {
        productType = productType.slice(0, -1);
      }

      // Acumula la cantidad de productos donados
      if (productCount[productType]) {
        productCount[productType] += donation.cantidad;
      } else {
        productCount[productType] = donation.cantidad;
      }

      // Acumula la cantidad donada a cada organización
      const organizationId = donation.organizacionId;
      if (organizationCount[organizationId]) {
        organizationCount[organizationId] += donation.cantidad;
      } else {
        organizationCount[organizationId] = donation.cantidad;
      }
    });

    // Encuentra el tipo de producto con la mayor cantidad donada
    this.mostDonatedProductType = Object.keys(productCount).reduce((a, b) =>
      productCount[a] > productCount[b] ? a : b
    );
    this.mostDonatedProductQuantity = productCount[this.mostDonatedProductType];

    // Encuentra la organización a la que más ha donado
    this.topOrganizationId = Object.keys(organizationCount).reduce((a, b) =>
      organizationCount[a] > organizationCount[b] ? a : b
    );

    let id = parseInt(this.topOrganizationId); // Parse topOrganizationId to number

    //para obtener el nombre de la organizacion
    this.organizationService.getOrganizationById(id).subscribe({
      next: (res) => {
        this.topOrganizationId = res.nombre;
      },
      error: (err) => {
        console.error('Error loading organization:', err);
      },
    });

    this.topOrganizationDonation = organizationCount[this.topOrganizationId];
  }

  toggleEditMode(cancel = false): void {
    this.isEditMode = !this.isEditMode;
    if (this.isEditMode) {
      if (this.role === 'usuario') {
        this.accountForm.enable(); // Enable form controls for user
      } else if (this.role === 'organizacion') {
        this.accountOrgForm.enable(); // Enable form controls for organization
      }
    } else {
      if (cancel) {
        // Restaurar los valores iniciales si se cancela
        if (this.role === 'usuario') {
          this.accountForm.patchValue(this.initialAccountFormValues);
        } else if (this.role === 'organizacion') {
          this.accountOrgForm.patchValue(this.initialAccountOrgFormValues);
        }
      }
      if (this.role === 'usuario') {
        this.accountForm.disable(); // Disable form controls for user
      } else if (this.role === 'organizacion') {
        this.accountOrgForm.disable(); // Disable form controls for organization
      }
    }
  }

  onSubmit(): void {
    if (this.role === 'usuario' && this.accountForm.invalid) {
      ValidateForm.validateAllFormFileds(this.accountForm);
      return;
    }

    if (this.role === 'organizacion' && this.accountOrgForm.invalid) {
      ValidateForm.validateAllFormFileds(this.accountOrgForm);
      return;
    }

    let userData: any;
    let orgData: any;

    if (this.role === 'usuario') {
      userData = this.accountForm.value;
      // Verifica si hay cambios
      if (
        JSON.stringify(userData) ===
        JSON.stringify(this.initialAccountFormValues)
      ) {
        console.log('No hay cambios');
        this.toast.info({
          detail: 'INFO',
          summary: 'No hay datos para actualizar',
          duration: 3000,
          position: 'bottomRight',
        });
        this.toggleEditMode(true);
        return;
      }

      userData = {
        ...this.accountForm.value,
        rolId: this.rolId,
      };
    } else if (this.role === 'organizacion') {
      orgData = this.accountOrgForm.value;

      // Verifica si hay cambios
      if (
        JSON.stringify(orgData) ===
        JSON.stringify(this.initialAccountOrgFormValues)
      ) {
        this.toast.info({
          detail: 'INFO',
          summary: 'No hay datos para actualizar',
          duration: 3000,
          position: 'bottomRight',
        });
        this.toggleEditMode(true);
        return;
      }

      orgData = {
        ...this.accountOrgForm.value,
        id: this.idOrg,
      };
    }

    if (this.role === 'usuario') {
      this.userStore.putUser(userData).subscribe({
        next: (res) => {
          if (res) {
            this.toggleEditMode();
            this.toast.success({
              detail: 'EXITO',
              summary: `${res.message}`,
              duration: 3000,
              position: 'bottomRight',
            });
          }
        },
        error: (err) => {
          console.error('Error:', err);
        },
      });
    } else if (this.role === 'organizacion') {
      this.organizationService.putOrganization(orgData).subscribe({
        next: (res) => {
          if (res) {
            console.log('Organization updated:', res);
            this.toggleEditMode();
            this.toast.success({
              detail: 'EXITO',
              summary: `${res.message}`,
              duration: 3000,
              position: 'bottomRight',
            });
          }
        },
        error: (err) => {
          console.error('Error:', err);
        },
      });
    }
  }
}
