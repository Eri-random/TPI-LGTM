import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { ModalOrganizacionComponent } from 'src/app/components/modal-organizacion/modal-organizacion.component';
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

const PRODUCTOS: string[] = [
  'Remera',
  'Manta',
  'Remera manga larga',
  'Pantalon',
  'Campera',
];

const NAMES: string[] = [
  'Maia',
  'Asher',
  'Olivia',
  'Atticus',
  'Amelia',
  'Jack',
  'Charlotte',
  'Theodore',
  'Isla',
  'Oliver',
  'Isabella',
  'Jasper',
  'Cora',
  'Levi',
  'Violet',
  'Arthur',
  'Mia',
  'Thomas',
  'Elizabeth',
];

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css'],
})
export class DashboardComponent implements AfterViewInit, OnInit {
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
  dataSource!: MatTableDataSource<UserData>;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(
    public dialog: MatDialog,
    private authService: AuthService,
    private organizacionService: OrganizacionService,
    private donacionesService:DonacionesService
  ) {
    this.dataSource = new MatTableDataSource();
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
        }else{
          this.existDonaciones = false;
        }
      },
      (error) => {
        console.error('Error:', error);
      }
    );
  }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }

  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();

    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }

  openDialog(): void {
    this.dialog.open(ModalOrganizacionComponent, {
      width: 'auto',
      height: '80%',
      disableClose: true,
      data: {}, // puedes pasar datos si lo necesitas
    });
  }
}