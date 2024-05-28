import { Component, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { NgToastService } from 'ng-angular-popup';
import { switchMap, tap } from 'rxjs';
import { AuthService } from 'src/app/services/auth.service';
import { OrganizacionService } from 'src/app/services/organizacion.service';
import { SedeService } from 'src/app/services/sede.service';

@Component({
  selector: 'app-sede',
  templateUrl: './sede.component.html',
  styleUrls: ['./sede.component.css'],
})
export class SedeComponent implements OnInit {
  sedes: any[] = [];
  orgNombre: any;
  organizacion: any;
  loading: boolean = true;
  selectedSede: any;

  @ViewChild('confirmDialog', { static: true }) confirmDialog!: TemplateRef<any>;

  constructor(
    private dialog: MatDialog,
    private router: Router,
    private organizacionService: OrganizacionService,
    private sedesService: SedeService,
    private toast: NgToastService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.organizacionService.getOrgNameFromStore().subscribe((val) => {
      const orgNameFromToken = this.authService.getOrgNameFromToken();
      this.orgNombre = val || orgNameFromToken;
    });

    const cuitFromToken = this.authService.getCuitFromToken();

    this.organizacionService
      .getOrganizacionByCuit(cuitFromToken)
      .pipe(
        tap((organizacion) => {
          this.organizacion = organizacion;
        }),
        switchMap(({ id }) => this.sedesService.getSedesByOrganization(id))
      )
      .subscribe(
        (sedes) => {
          this.sedes = sedes;
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

  agregarSede() {
    this.router.navigate(['/crear-sede']);
  }

  editarSede(sede: any) {
    this.router.navigate(['/editar-sede', sede.id]);
  }

  openConfirmDialog(sede: any) {
    this.selectedSede = sede;
    const dialogRef = this.dialog.open(this.confirmDialog, {
      width: '250px',
      data: { nombre: sede.nombre }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.eliminarSede(sede.id);
      }
    });
  }

  eliminarSede(id: number) {
    this.sedesService.deleteSede(id).subscribe(() => {
      this.sedes = this.sedes.filter(s => s.id !== id);
      this.toast.success({
        detail: 'EXITO',
        summary: 'Sede eliminada correctamente',
        duration: 3000,
        position: 'topRight',
      });
    }, error =>{
      this.toast.error({
        detail: 'ERROR',
        summary: 'Error al eliminar la sede',
        duration: 3000,
        position: 'topRight',
      });
    });
  }

  onCancel(): void {
    this.dialog.closeAll();
  }

  onConfirm(): void {
    this.dialog.closeAll();
    this.eliminarSede(this.selectedSede.id);
  }
}