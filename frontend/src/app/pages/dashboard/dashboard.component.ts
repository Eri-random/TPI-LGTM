import { AfterViewInit, Component, OnInit, ViewChild,ChangeDetectorRef } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { OrganizacionService } from 'src/app/services/organizacion.service';
import { AuthService } from 'src/app/services/auth.service';
import { switchMap,tap } from 'rxjs';
import { DonacionesService } from 'src/app/services/donaciones.service';

export interface UserData {
  name: string;
  telefono: string;
  email: string;
  producto: string;
  cantidad: number;
  progress: string;
}

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css'],
})
export class DashboardComponent implements OnInit {
  cuit!: string;
  orgNombre:any
  existDonaciones!:boolean;

  displayedColumns: string[] = [
    'name',
    'telefono',
    'email',
    'producto',
    'cantidad'
    // 'progress',
  ];
  
  dataSource: MatTableDataSource<UserData> = new MatTableDataSource();
  loading: boolean = true;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(
    public dialog: MatDialog,
    private cdr: ChangeDetectorRef,
    private authService: AuthService,
    private organizacionService: OrganizacionService,
    private donacionesService:DonacionesService
  ) {
  }

  ngOnInit() {
    this.organizacionService.getOrgNameFromStore().subscribe((val) => {
      const orgNameFromToken = this.authService.getOrgNameFromToken();
      this.orgNombre = val || orgNameFromToken;
    });

    this.organizacionService.getCuitFromStore().subscribe((val) => {
      const cuitFromToken = this.authService.getCuitFromToken();
      this.cuit = val || cuitFromToken;
    });

    this.organizacionService
    .getOrganizacionByCuit(this.cuit)
    .pipe(
      switchMap(({id}) => this.donacionesService.getDonacionesByOrganizacionId(id))
    )
    .subscribe(
      (donaciones) => {
        if(donaciones.length != 0){
          this.existDonaciones = true;
          this.dataSource.data = donaciones;
          this.cdr.detectChanges();
          this.dataSource.paginator = this.paginator;
          this.dataSource.sort = this.sort;

          setTimeout(() => {
            this.loading = false; 
          }, 1000);
        }else{
          this.existDonaciones = false;

          setTimeout(() => {
            this.loading = false; 
          }, 1000);
        }
      },
      (error) => {
        console.error('Error:', error);
      }
    );
  }

  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();

    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }

}