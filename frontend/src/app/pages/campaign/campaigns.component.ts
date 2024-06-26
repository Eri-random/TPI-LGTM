import { Component, OnInit, ViewChild } from '@angular/core';
import { MatAccordion } from '@angular/material/expansion';
import { FormBuilder, FormGroup, FormArray, Validators } from '@angular/forms';
import { CampaignService, Campaign } from '././../../services/campaign.service';
import { NeedService } from 'src/app/services/need.service';
import { OrganizationService } from 'src/app/services/organization.service';
import { AuthService } from 'src/app/services/auth.service';
import { NgToastService } from 'ng-angular-popup';
import { switchMap } from 'rxjs';

@Component({
  selector: 'app-campaigns',
  templateUrl: './campaigns.component.html',
  styleUrls: ['./campaigns.component.css']
})
export class CampaignsComponent implements OnInit {
  @ViewChild(MatAccordion) accordion!: MatAccordion;
  campaigns: Campaign[] = [];
  needs: any[] = [];
  formGroups: { [key: string]: FormGroup } = {};
  organizationId!: number;
  campaignForm: FormGroup;
  isLogged: boolean = true;
  loading: boolean = true;
  id!: number;
  cuit!: string;

  constructor(
    private fb: FormBuilder,
    private campaignService: CampaignService,
    private organizationService: OrganizationService,
    private needService: NeedService,
    private authService: AuthService,
    private toast: NgToastService
  ) {
    this.campaignForm = this.fb.group({
      title: ['', Validators.required],
      startDate: ['', Validators.required],
      endDate: ['', Validators.required],
      needs: this.fb.array([], Validators.required)
    });
  }

  ngOnInit(): void {
    this.isLogged = this.authService.isLoggedIn();
    this.organizationService.getCuitFromStore().subscribe((val) => {
      const cuitFromToken = this.authService.getCuitFromToken();
      this.cuit = val || cuitFromToken;
    });

    this.organizationService.getOrganizationByCuit(this.cuit).subscribe((rep) => {
      this.organizationId = rep.id;
      this.loadForm();
    });
    this.loadCampaignsAndNeeds(this.cuit);
    // this.loadAllNeeds();
  }

  loadForm() {
    this.needService.getAllNeeds().subscribe((resp) => {
      this.needs = resp;
  
      // Crear los grupos de formularios dinámicamente
      this.needs.forEach((need: any) => {
        const formGroup = this.fb.group({});
        need.subcategoria.forEach((sub: any) => {
          formGroup.addControl(sub.nombre, this.fb.control(false)); // Inicializar como no marcado
        });
        this.formGroups[need.nombre] = formGroup; // Asigna el FormGroup a una propiedad del componente
      });

      this.loading = false; 
      
    });
  }

  loadCampaignsAndNeeds(cuit: string): void {
    this.organizationService
      .getOrganizationByCuit(cuit)
      .pipe(
        switchMap(({ id }) => {
          this.organizationId = id;
          return this.campaignService.getAllCampaigns(id.toString());
        })
      )
      .subscribe(
        data => {
          this.campaigns = data;
          this.loading = false;
        },
        error => console.error(error)
      );
  }

  loadAllNeeds(): void {
    this.needService.getAllNeeds().subscribe(
      data => {
        this.needs = data;
        this.initializeFormGroups();
        this.loading = false;
      },
      error => console.error(error)
    );
  }

  initializeFormGroups(): void {
    this.needs.forEach((need: any) => {
      const formGroup = this.fb.group({});
      need.subcategoria.forEach((sub: any) => {
        formGroup.addControl(sub.nombre, this.fb.control(false)); // Initialize as unchecked
      });
      this.formGroups[need.nombre] = formGroup; // Assign the FormGroup to the formGroups object
    });
  }

  addCampaign(): void {
    const selectedSubcategories = this.getSelectedSubcategories();
    const newCampaign: Campaign = {
      ...this.campaignForm.value,
      organizacionId: this.organizationId,
      startDate: new Date(this.campaignForm.value.startDate).toISOString(),
      endDate: new Date(this.campaignForm.value.endDate).toISOString(),
      subcategoria: selectedSubcategories, // Assign selected subcategories as needs
    };

    this.campaignService.createCampaign(newCampaign).subscribe(
      data => {
        this.campaigns.push(data);
        this.campaignForm.reset(); // Reset the form
      },
      error => console.error(error)
    );
  }

  getSelectedSubcategories(): any[] {
    const selectedSubcategories: any[] = [];

    this.needs.forEach((need: any) => {
      const formGroup = this.formGroups[need.nombre];
      need.subcategoria.forEach((sub: any) => {
        if (formGroup.get(sub.nombre)?.value) {
          selectedSubcategories.push({
            id: sub.id,
            nombre: sub.nombre,
            necesidadId: need.id
          });
        }
      });
    });

    return selectedSubcategories;
  }

  saveNeeds(): void {
    console.log('Selected subcategories:', this.getSelectedSubcategories());
  }

  getUniqueNeeds(subs: any[]): any[] {
    const uniqueNeeds = new Map<number, any>();
    subs.forEach(sub => {
      if (!uniqueNeeds.has(sub.NecesidadId)) {
        uniqueNeeds.set(sub.NecesidadId, {
          NecesidadId: sub.NecesidadId,
          NecesidadNombre: sub.NecesidadNombre,
          NecesidadIcono: sub.NecesidadIcono
        });
      }
    });
    return Array.from(uniqueNeeds.values());
  }  

  deleteCampaign(id: number): void {
    this.campaignService.deleteCampaign(id).subscribe(
      () => {
        this.campaigns = this.campaigns.filter(campaign => campaign.id !== id);
        this.toast.success({
          detail: 'Éxito',
          summary: 'Campaña eliminada correctamente',
          duration: 5000,
          position: 'bottomRight',
        });
      },
      error => {
        console.error(error);
        this.toast.error({
          detail: 'Error',
          summary: 'Error al eliminar la campaña',
          duration: 5000,
          position: 'bottomRight',
        });
      }
    );
  }

  get fm() { return this.campaignForm.controls; }
}
