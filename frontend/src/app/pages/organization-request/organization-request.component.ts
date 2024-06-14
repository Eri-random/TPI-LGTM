import { Component, OnInit, ViewChild } from '@angular/core';
import { MatAccordion } from '@angular/material/expansion';
import { FormBuilder, FormGroup } from '@angular/forms';
import { NeedService } from 'src/app/services/need.service';
import { OrganizationService } from 'src/app/services/organization.service';
import { AuthService } from 'src/app/services/auth.service';
import { NgToastService } from 'ng-angular-popup';

@Component({
  selector: 'app-organization-request',
  templateUrl: './organization-request.component.html',
  styleUrls: ['./organization-request.component.css'],
})
export class OrganizationRequestComponent implements OnInit {
  @ViewChild(MatAccordion) accordion!: MatAccordion;
  needs: any[] = [];
  cuit!: string;
  id!: number;
  formGroups: { [key: string]: FormGroup } = {};
  loading: boolean = true;


  constructor(
    private _formBuilder: FormBuilder,
    private needsService: NeedService,
    private organizationService: OrganizationService,
    private authService: AuthService,
    private toast: NgToastService
  ) { }

  ngOnInit(): void {
    this.organizationService.getCuitFromStore().subscribe((val) => {
      const cuitFromToken = this.authService.getCuitFromToken();
      this.cuit = val || cuitFromToken;
    });

    this.organizationService.getOrganizationByCuit(this.cuit).subscribe((rep) => {
      this.id = rep.id;
      this.loadForm();
    });

  }


  loadForm() {
    this.needsService.getAllNeeds().subscribe((resp) => {
      this.needs = resp;
  
      // Crear los grupos de formularios dinámicamente
      this.needs.forEach((need: any) => {
        const formGroup = this._formBuilder.group({});
        need.subcategoria.forEach((sub: any) => {
          formGroup.addControl(sub.nombre, this._formBuilder.control(false)); // Inicializar como no marcado
        });
        this.formGroups[need.nombre] = formGroup; // Asigna el FormGroup a una propiedad del componente
      });
  
      // Obtener subcategorías asignadas a la organización
      this.organizationService.getAssignedSubcategories(this.id).subscribe((assigned) => {
        this.needs.forEach((need: any) => {
          const formGroup = this.formGroups[need.nombre];
          need.subcategoria.forEach((sub: any) => {
            const isChecked = assigned.some(assign => assign.id === sub.id);
            formGroup.get(sub.nombre)?.setValue(isChecked); // Marcar las subcategorías asignadas
          });
        });
  
        // Desactivar el estado de carga
        setTimeout(() => {
          this.loading = false; 
        }, 1000);
      });
    });
  }

  GetSelectedSubcategories() {
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

  saveNeeds() {
    const selectedSubcategories = this.GetSelectedSubcategories();
    this.organizationService.assignSubcategories(this.id, selectedSubcategories).subscribe({
      next: (response:any) => {
        this.toast.success({
          detail: 'EXITO',
          summary: response.message,
          duration: 5000,
          position:'bottomRight',
        });
      },
      error: (error:any) => {
        this.toast.success({
          detail: 'EXITO',
          summary: error.error,
          duration: 5000,
          position:'bottomRight',
        });
      }
    });
  }
}
