import { Component, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { NgToastService } from 'ng-angular-popup';
import { switchMap, tap } from 'rxjs';
import { AuthService } from 'src/app/services/auth.service';
import { OrganizationService } from 'src/app/services/organization.service';
import { HeadquartersService } from 'src/app/services/headquarters.service';

@Component({
  selector: 'app-headquarters',
  templateUrl: './headquarters.component.html',
  styleUrls: ['./headquarters.component.css'],
})
export class SedeComponent implements OnInit {
  headquarters: any[] = [];
  orgName: any;
  cuit!:string
  organization: any;
  loading: boolean = true;
  selectedSede: any;

  @ViewChild('confirmDialog', { static: true }) confirmDialog!: TemplateRef<any>;

  constructor(
    private dialog: MatDialog,
    private router: Router,
    private organizationService: OrganizationService,
    private headquartersService: HeadquartersService,
    private toast: NgToastService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.organizationService.getOrgNameFromStore().subscribe((val) => {
      const orgNameFromToken = this.authService.getOrgNameFromToken();
      this.orgName = val || orgNameFromToken;
    });

    this.organizationService.getCuitFromStore().subscribe((val) => {
      const cuitFromToken = this.authService.getCuitFromToken();
      this.cuit = val || cuitFromToken;
    });


    this.organizationService
      .getOrganizationByCuit(this.cuit)
      .pipe(
        tap((org) => {
          this.organization = org;
        }),
        switchMap(({ id }) => this.headquartersService.getHeadquartersByOrganization(id))
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

  addingHeadquarters() {
    this.router.navigate(['/crear-sede']);
  }

  editHeadquarters(headquarter: any) {
    this.router.navigate(['/editar-sede', headquarter.id]);
  }

  openConfirmDialog(headquarter: any) {
    this.selectedSede = headquarter;
    const dialogRef = this.dialog.open(this.confirmDialog, {
      width: '250px',
      data: { nombre: headquarter.nombre }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.deleteHeadquarters(headquarter.id);
      }
    });
  }

  deleteHeadquarters(id: number) {
    this.headquartersService.deleteHeadquarters(id).subscribe(() => {
      this.headquarters = this.headquarters.filter(s => s.id !== id);
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
    this.deleteHeadquarters(this.selectedSede.id);
  }
}