import {
  AfterViewInit,
  Component,
  OnInit,
  ViewChild,
  ChangeDetectorRef,
  ViewContainerRef,
} from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { OrganizationService } from 'src/app/services/organization.service';
import { AuthService } from 'src/app/services/auth.service';
import { switchMap, tap } from 'rxjs';
import { DonationsService } from 'src/app/services/donations.service';
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
  orgName: any;
  existDonations!: boolean;
  totalDonations: number = 0;
  totalDonationsCount: number = 0;
  productMostDonate: { product: string, amount: number } | null = null;
  averageDonations: number = 0;

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
    private organizationService: OrganizationService,
    private donationsService: DonationsService,
    private webSocketService: WebsocketService,
    private viewContainerRef: ViewContainerRef,
    private toast: NgToastService
  ) {}

  ngOnInit() {
    this.organizationService.getOrgNameFromStore().subscribe((val) => {
      const orgNameFromToken = this.authService.getOrgNameFromToken();
      this.orgName = val || orgNameFromToken;
    });

    this.organizationService.getCuitFromStore().subscribe((val) => {
      const cuitFromToken = this.authService.getCuitFromToken();
      this.cuit = val || cuitFromToken;
    });

    this.loadDonations();

    this.webSocketService.messages.subscribe((message) => {
      if (message.type === 'actualizarDonaciones') {
        this.handleNewDonation(message.data);
      }
    });
  }

  loadDonations() {
    this.organizationService
      .getOrganizationByCuit(this.cuit)
      .pipe(
        switchMap(({ id }) =>
          this.donationsService.getDonationsByOrganizationId(id)
        )
      )
      .subscribe(
        (donations) => {
          console.log(donations);
          if (donations.length != 0) {
            const formattedDonations = donations.map((donation: any) => ({
              name: donation.usuario.nombre,
              telefono: donation.usuario.telefono,
              email: donation.usuario.email,
              producto: donation.producto,
              cantidad: donation.cantidad,
              progress: '',
            }));

            this.existDonations = true;
            this.dataSource.data = formattedDonations;
            // this.cdr.detectChanges();
            this.totalDonations = donations.reduce(
              (total: any, donation: any) => total + donation.cantidad,
              0
            );

            this.totalDonationsCount = donations.length;
            this.averageDonations = this.totalDonations / this.totalDonationsCount;
            this.calculateProductMostDonated(donations);

            setTimeout(() => {
              this.loading = false;
            }, 1000);
          } else {
            this.existDonations = false;

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

  ngAfterViewInit(): void {  
    // Observador para esperar hasta que los elementos estén disponibles
    const observer = new MutationObserver(() => {
      if (this.paginator && this.sort) {
        // Asignar paginator y sort a dataSource cuando estén disponibles
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;
        observer.disconnect(); // Detener el observador una vez que se han asignado los elementos
      }
    });
  
    observer.observe(this.viewContainerRef.element.nativeElement, {
      childList: true, // Observar cambios en los hijos del contenedor
      subtree: true // Observar cambios en todo el árbol del contenedor
    });
  }


  calculateProductMostDonated(donations: any[]) {
    const productoMap = new Map<string, number>();

    donations.forEach((donation) => {
      if (productoMap.has(donation.producto)) {
        productoMap.set(donation.producto, productoMap.get(donation.producto)! + donation.cantidad);
      } else {
        productoMap.set(donation.producto, donation.cantidad);
      }
    });

    const sortedProductos = Array.from(productoMap, ([product, amount]) => ({ product, amount }))
      .sort((a, b) => b.amount - a.amount);

    this.productMostDonate = sortedProductos.length > 0 ? sortedProductos[0] : null;
  }

  handleNewDonation(data: any) {
    if (data && data.newDonation && data.user) {
      this.totalDonations += data.newDonation.Cantidad;
      this.totalDonationsCount += 1;
      this.averageDonations = this.totalDonations / this.totalDonationsCount;

      let newDonation: UserData = {
        name: data.user.Nombre,
        telefono: data.user.Telefono,
        email: data.user.Email,
        producto: data.newDonation.Producto,
        cantidad: data.newDonation.Cantidad,
        progress: '',
        highlight: true,
      };

      let newData = [...this.dataSource.data, newDonation];
      this.dataSource.data = newData;

      this.calculateProductMostDonated(newData);

      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
      this.dataSource._updateChangeSubscription(); // Notifica a la tabla que hay nuevos datos
      // this.cdr.detectChanges();

      this.toast.success({
        detail: 'EXITO',
        summary: 'Nueva donación recibida',
        duration: 5000,
        position: 'topRight',
      });

      setTimeout(() => {
        newDonation.highlight = false;
        // this.cdr.detectChanges();
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
