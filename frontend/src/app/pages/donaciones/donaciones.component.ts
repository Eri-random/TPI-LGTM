import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { OrganizacionService } from 'src/app/services/organizacion.service';

@Component({
  selector: 'app-donaciones',
  templateUrl: './donaciones.component.html',
  styleUrls: ['./donaciones.component.css'],
})
export class DonacionesComponent implements OnInit {
  organizaciones: any[] = [];
  page: number = 1;
  pageSize: number = 8;
  mostrarVerMas: boolean = true;

  constructor(
    private organizacionService: OrganizacionService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.cargarOrganizaciones();
  }

  cargarOrganizaciones(): void {
    this.organizacionService
      .getOrganizacionesPaginadas(this.page, this.pageSize)
      .subscribe((resp: any[]) => {
        this.organizaciones = this.organizaciones.concat(resp);
        if (resp.length < this.pageSize) {
          this.mostrarVerMas = false;
        }
      });
  }

  cargarMas(): void {
    this.page++;
    this.cargarOrganizaciones();
  }

  verDetalle(org: any): void {
    this.router.navigate(['/info-organizacion', org.id]);
  }
}
