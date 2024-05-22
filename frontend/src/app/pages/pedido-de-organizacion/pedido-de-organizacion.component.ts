import { Component, ViewChild } from '@angular/core';
import { MatAccordion, MatExpansionModule } from '@angular/material/expansion';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatNativeDateModule } from '@angular/material/core';

import {FormBuilder, FormsModule, ReactiveFormsModule} from '@angular/forms';
import {JsonPipe} from '@angular/common';
import {MatCheckboxModule} from '@angular/material/checkbox';

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

  ropa_y_estimenta = this._formBuilder.group({
    camisetas: false,
    pantalones: false,
    chaquetas: false,
    ropa_interior: false,
    ropa_para_niños: false
  });


  constructor(private _formBuilder: FormBuilder) {}
}
