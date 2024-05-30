import {
  AfterViewInit,
  Component,
  OnInit,
  ViewChild,
  ChangeDetectorRef,
} from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { OrganizacionService } from 'src/app/services/organizacion.service';
import { AuthService } from 'src/app/services/auth.service';
import { switchMap, tap } from 'rxjs';
import { DonacionesService } from 'src/app/services/donaciones.service';
import { WebsocketService } from 'src/app/services/websocket.service';
import { NgToastService } from 'ng-angular-popup';

export interface UserData {
  name: string;
  telefono: string;
  email: string;
  producto: string;
  cantidad: number;
  progress: string;
  highlight?: boolean; // Add an optional highlight property
}

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css'],
})
export class DashboardComponent implements OnInit {
  cuit!: string;
  orgNombre: any;
  existDonaciones!: boolean;
  totalDonaciones: number = 0;
  totalDonacionesCount: number = 0;
  productoMasDonado: { producto: string, cantidad: number } | null = null;
  promedioDonaciones: number = 0;

  displayedColumns: string[] = [
    'name',
    'telefono',
    'email',
    'producto',
    'cantidad',
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
    private donacionesService: DonacionesService,
    private webSocketService: WebsocketService,
    private toast: NgToastService
  ) {}

  ngOnInit() {
    this.organizacionService.getOrgNameFromStore().subscribe((val) => {
      const orgNameFromToken = this.authService.getOrgNameFromToken();
      this.orgNombre = val || orgNameFromToken;
    });

    this.organizacionService.getCuitFromStore().subscribe((val) => {
      const cuitFromToken = this.authService.getCuitFromToken();
      this.cuit = val || cuitFromToken;
    });

    this.loadDonaciones();

    this.webSocketService.messages.subscribe((message) => {
      if (message.type === 'actualizarDonaciones') {
        this.handleNewDonation(message.data);
      }
    });
  }

  loadDonaciones() {
    this.organizacionService
      .getOrganizacionByCuit(this.cuit)
      .pipe(
        switchMap(({ id }) =>
          this.donacionesService.getDonacionesByOrganizacionId(id)
        )
      )
      .subscribe(
        (donaciones) => {
          if (donaciones.length != 0) {
            const formattedDonations = donaciones.map((donacion: any) => ({
              name: donacion.usuario.nombre,
              telefono: donacion.usuario.telefono,
              email: donacion.usuario.email,
              producto: donacion.producto,
              cantidad: donacion.cantidad,
              progress: '',
            }));

            this.existDonaciones = true;
            this.dataSource.data = formattedDonations;
            this.cdr.detectChanges();
            this.dataSource.paginator = this.paginator;
            this.dataSource.sort = this.sort;
            this.totalDonaciones = donaciones.reduce(
              (total: any, donacion: any) => total + donacion.cantidad,
              0
            );

            this.totalDonacionesCount = donaciones.length;
            this.promedioDonaciones = this.totalDonaciones / this.totalDonacionesCount;
            this.calcularProductoMasDonado(donaciones);

            setTimeout(() => {
              this.loading = false;
            }, 1000);
          } else {
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


  calcularProductoMasDonado(donaciones: any[]) {
    const productoMap = new Map<string, number>();

    donaciones.forEach((donacion) => {
      if (productoMap.has(donacion.producto)) {
        productoMap.set(donacion.producto, productoMap.get(donacion.producto)! + donacion.cantidad);
      } else {
        productoMap.set(donacion.producto, donacion.cantidad);
      }
    });

    const sortedProductos = Array.from(productoMap, ([producto, cantidad]) => ({ producto, cantidad }))
      .sort((a, b) => b.cantidad - a.cantidad);

    this.productoMasDonado = sortedProductos.length > 0 ? sortedProductos[0] : null;
  }

  handleNewDonation(data: any) {
    if (data && data.nuevaDonacion && data.usuario) {
      this.totalDonaciones += data.nuevaDonacion.Cantidad;
      this.totalDonacionesCount += 1;
      this.promedioDonaciones = this.totalDonaciones / this.totalDonacionesCount;

      let newDonation: UserData = {
        name: data.usuario.Nombre,
        telefono: data.usuario.Telefono,
        email: data.usuario.Email,
        producto: data.nuevaDonacion.Producto,
        cantidad: data.nuevaDonacion.Cantidad,
        progress: '',
        highlight: true,
      };

      let newData = [...this.dataSource.data, newDonation];
      this.dataSource.data = newData;

      this.calcularProductoMasDonado(newData);

      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
      this.dataSource._updateChangeSubscription(); // Notifica a la tabla que hay nuevos datos
      this.cdr.detectChanges();

      this.toast.success({
        detail: 'EXITO',
        summary: 'Nueva donación recibida',
        duration: 5000,
        position: 'topRight',
      });

      setTimeout(() => {
        newDonation.highlight = false;
        this.cdr.detectChanges();
      }, 8000);
    } else {
      console.error('Invalid data format received:', data);
    }
  }

  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();

    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }
}
