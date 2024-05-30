import { Component, OnInit, ViewChild } from '@angular/core';
import { MatAccordion } from '@angular/material/expansion';
import { FormBuilder, FormGroup } from '@angular/forms';
import { NecesidadService } from 'src/app/services/necesidad.service';
import { OrganizacionService } from 'src/app/services/organizacion.service';
import { AuthService } from 'src/app/services/auth.service';
import { NgToastService } from 'ng-angular-popup';

@Component({
  selector: 'app-pedido-de-organizacion',
  templateUrl: './pedido-de-organizacion.component.html',
  styleUrls: ['./pedido-de-organizacion.component.css'],
})
export class PedidoDeOrganizacionComponent implements OnInit {
  @ViewChild(MatAccordion) accordion!: MatAccordion;
  necesidades: any[] = [];
  cuit!: string;
  id!: number;
  formGroups: { [key: string]: FormGroup } = {};


  constructor(
    private _formBuilder: FormBuilder,
    private necesidadService: NecesidadService,
    private organizacionService: OrganizacionService,
    private authService: AuthService,
    private toast: NgToastService
  ) { }

  ngOnInit(): void {
    this.organizacionService.getCuitFromStore().subscribe((val) => {
      const cuitFromToken = this.authService.getCuitFromToken();
      this.cuit = val || cuitFromToken;
    });

    this.loadFormularios();

    this.organizacionService.getOrganizacionByCuit(this.cuit).subscribe((rep) => {
      this.id = rep.id;
      this.loadNecesidades();
    });

  }

  loadFormularios() {
    this.necesidadService.getAllNecesidades().subscribe((resp) => {
      this.necesidades = resp;

      // Crear los grupos de formularios dinámicamente
      this.necesidades.forEach((necesidad: any) => {
        const formGroup = this._formBuilder.group({});
        necesidad.subcategoria.forEach((sub: any) => {
          formGroup.addControl(sub.nombre, this._formBuilder.control(false)); // Inicializar como no marcado
        });
        this.formGroups[necesidad.nombre] = formGroup; // Asigna el FormGroup a una propiedad del componente
      });
    });
  }

  loadNecesidades(): void {
    this.organizacionService.getSubcategoriasAsignadas(this.id).subscribe((asignadas) => {
      this.necesidades.forEach((necesidad: any) => {
        const formGroup = this.formGroups[necesidad.nombre];
        necesidad.subcategoria.forEach((sub: any) => {
          const isChecked = asignadas.some(asignada => asignada.id === sub.id);
          formGroup.get(sub.nombre)?.setValue(isChecked); // Marcar las subcategorías asignadas
        });
      });
    });
  }

  GetSelectedSubcategorias() {
    const selectedSubcategorias: any[] = [];

    this.necesidades.forEach((necesidad: any) => {
      const formGroup = this.formGroups[necesidad.nombre];
      necesidad.subcategoria.forEach((sub: any) => {
        if (formGroup.get(sub.nombre)?.value) {
          selectedSubcategorias.push({
            id: sub.id,
            nombre: sub.nombre,
            necesidadId: necesidad.id
          });
        }
      });
    });

    return selectedSubcategorias;
  }

  saveNecesidades() {
    const selectedSubcategorias = this.GetSelectedSubcategorias();
    this.organizacionService.asignarSubcategorias(this.id, selectedSubcategorias).subscribe({
      next: (response:any) => {
        this.toast.success({
          detail: 'EXITO',
          summary: response.message,
          duration: 5000,
          position:'topRight',
        });
      },
      error: (error:any) => {
        this.toast.success({
          detail: 'EXITO',
          summary: error.error,
          duration: 5000,
          position:'topRight',
        });
      }
    });
  }
}
