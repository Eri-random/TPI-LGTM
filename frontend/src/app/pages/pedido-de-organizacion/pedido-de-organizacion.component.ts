import { Component, ViewChild } from '@angular/core';
import { MatAccordion, MatExpansionModule } from '@angular/material/expansion';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatNativeDateModule } from '@angular/material/core';

import { FormBuilder, FormsModule, ReactiveFormsModule, FormGroup } from '@angular/forms';
import { JsonPipe } from '@angular/common';
import { MatCheckboxModule } from '@angular/material/checkbox';

@Component({
  selector: 'app-pedido-de-organizacion',
  templateUrl: './pedido-de-organizacion.component.html',
  styleUrls: ['./pedido-de-organizacion.component.css'],
  standalone: true,
  imports: [
    MatButtonModule,
    MatExpansionModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatDatepickerModule,
    MatNativeDateModule,
    FormsModule, ReactiveFormsModule, MatCheckboxModule, JsonPipe
  ],
})
export class PedidoDeOrganizacionComponent {
  @ViewChild(MatAccordion) accordion!: MatAccordion;

  ropa_y_vestimenta: FormGroup;
  accesorios: FormGroup;
  ropa_de_cama_y_banio: FormGroup;
  productos_para_el_hogar: FormGroup;
  juguetes_y_productos_para_ninios: FormGroup;
  productos_para_mascotas: FormGroup;

  constructor(private _formBuilder: FormBuilder) {
    this.ropa_y_vestimenta = this._formBuilder.group({
      camisetas: false,
      pantalones: false,
      chaquetas: false,
      ropa_interior: false,
      ropa_para_niños: false
    });

    this.accesorios = this._formBuilder.group({
      bolsas_de_mano: false,
      mochilas: false,
      sombreros: false,
      bufandas: false,
      guantes: false
    });

    this.ropa_de_cama_y_banio = this._formBuilder.group({
      sabanas: false,
      fundas_de_almohada: false,
      toallas: false,
      cortinas_de_banio: false,
      colchas: false
    });

    this.productos_para_el_hogar = this._formBuilder.group({
      fundas_para_cojines: false,
      manteles: false,
      servilletas_de_tela: false,
      alfombras: false,
      tapetes: false
    });

    this.juguetes_y_productos_para_ninios = this._formBuilder.group({
      peluches: false,
      mantitas: false,
      ropa_de_cama_infantil: false,
      baberos: false,
      muniecos_de_tela: false
    });

    this.productos_para_mascotas = this._formBuilder.group({
      camas_para_mascotas: false,
      juguetes_de_tela: false,
      mantas_para_mascotas: false,
      ropa_para_mascotas: false,
      bolsas_de_transporte: false
    });
  }

  mostrarSeleccionados() {
    this.mostrarSeleccionGrupo('Ropa y Vestimenta', this.ropa_y_vestimenta);
    this.mostrarSeleccionGrupo('Accesorios', this.accesorios);
    this.mostrarSeleccionGrupo('Ropa de Cama y Baño', this.ropa_de_cama_y_banio);
    this.mostrarSeleccionGrupo('Productos para el Hogar', this.productos_para_el_hogar);
    this.mostrarSeleccionGrupo('Juguetes y Productos para Niños', this.juguetes_y_productos_para_ninios);
    this.mostrarSeleccionGrupo('Productos para Mascotas', this.productos_para_mascotas);
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
