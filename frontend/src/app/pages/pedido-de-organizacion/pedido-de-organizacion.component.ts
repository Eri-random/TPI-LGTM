import { Component, OnInit, ViewChild } from '@angular/core';
import { MatAccordion } from '@angular/material/expansion';
import { FormBuilder, FormGroup } from '@angular/forms';
import { NecesidadService } from 'src/app/services/necesidad.service';

@Component({
  selector: 'app-pedido-de-organizacion',
  templateUrl: './pedido-de-organizacion.component.html',
  styleUrls: ['./pedido-de-organizacion.component.css'],
})
export class PedidoDeOrganizacionComponent implements OnInit{
  @ViewChild(MatAccordion) accordion!: MatAccordion;
  necesidades:any [] = [];
  formGroups: { [key: string]: FormGroup } = {};

  constructor(private _formBuilder: FormBuilder,
    private necesidadService:NecesidadService
  ) {}


  ngOnInit(): void {
    this.necesidadService.getAllNecesidades().subscribe(resp => {
      this.necesidades = resp;
  
      // Crear los grupos de formularios dinámicamente
      this.necesidades.forEach((necesidad:any) => {
        const formGroup = this._formBuilder.group({});
        necesidad.subcategoria.forEach((sub:any) => {
          formGroup.addControl(sub.nombre, this._formBuilder.control(false));
        });
        this.formGroups[necesidad.nombre] = formGroup; // Asigna el FormGroup a una propiedad del componente
      });
    });
  }

  mostrarSeleccionados() {
    this.necesidades.forEach(necesidad => {
      this.mostrarSeleccionGrupo(necesidad.nombre, this.formGroups[necesidad.nombre]);
    });
  }

  private mostrarSeleccionGrupo(nombreGrupo: string, formGroup: FormGroup) {
    const elementosSeleccionados = Object.keys(formGroup.value)
      .filter(clave => formGroup.value[clave])
      .map(clave => clave.replace(/_/g, ' '));

    if (elementosSeleccionados.length > 0) {
      console.log(`${nombreGrupo}:`, elementosSeleccionados);
    } else {
      console.log(`${nombreGrupo}: No hay elementos seleccionados`);
    }
  }

}
