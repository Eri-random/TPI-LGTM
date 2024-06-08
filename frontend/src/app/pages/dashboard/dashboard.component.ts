import {
  AfterViewInit,
  Component,
  OnInit,
  ViewChild,
  ChangeDetectorRef,
  ViewContainerRef,
} from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator, MatPaginatorIntl } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { OrganizationService } from 'src/app/services/organization.service';
import { AuthService } from 'src/app/services/auth.service';
import { switchMap, tap } from 'rxjs';
import { DonationsService } from 'src/app/services/donations.service';
import { WebsocketService } from 'src/app/services/websocket.service';
import { NgToastService } from 'ng-angular-popup';
import * as XLSX from 'xlsx';
import { saveAs } from 'file-saver';

export interface UserData {
  id: number;
  name: string;
  telefono: string;
  email: string;
  producto: string;
  cantidad: number;
  estado: string;
  highlight?: boolean; // Add an optional highlight property
  selected?:boolean;
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
  topDonor: { name: string, amount: number } | null = null;
  checkboxEnabled = false;

  displayedColumns: string[] = [
    'name',
    'telefono',
    'email',
    'producto',
    'cantidad',
    'estado',
  ];

  dataSource: MatTableDataSource<UserData> = new MatTableDataSource();
  selectedDonations: number[] = [];
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
  ) { }

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
      if (message.type === 'actualizarDonaciones' && message.data.newDonation.Cuit === this.cuit) {
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
              id: donation.id,
              name: donation.usuario.nombre,
              telefono: donation.usuario.telefono,
              email: donation.usuario.email,
              producto: donation.producto,
              cantidad: donation.cantidad,
              estado: donation.estado,
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
            this.calculateTopDonor(donations);

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
      const normalizedProduct = normalizeProductName(donation.producto);
      if (productoMap.has(normalizedProduct)) {
        productoMap.set(normalizedProduct, productoMap.get(normalizedProduct)! + donation.cantidad);
      } else {
        productoMap.set(normalizedProduct, donation.cantidad);
      }
    });

    const sortedProductos = Array.from(productoMap, ([product, amount]) => ({ product, amount }))
      .sort((a, b) => b.amount - a.amount);

    this.productMostDonate = sortedProductos.length > 0 ? sortedProductos[0] : null;
  }

  calculateTopDonor(donations: any[]) {
    const donorMap = new Map<string, number>();

    donations.forEach((donation) => {
      if (donorMap.has(donation.usuario.nombre)) {
        donorMap.set(donation.usuario.nombre, donorMap.get(donation.usuario.nombre)! + donation.cantidad);
      } else {
        donorMap.set(donation.usuario.nombre, donation.cantidad);
      }
    });

    const sortedDonors = Array.from(donorMap, ([name, amount]) => ({ name, amount }))
      .sort((a, b) => b.amount - a.amount);

    this.topDonor = sortedDonors.length > 0 ? sortedDonors[0] : null;
  }

  handleNewDonation(data: any) {
    if (data && data.newDonation && data.user) {
      this.totalDonations += data.newDonation.Cantidad;
      this.totalDonationsCount += 1;
      this.averageDonations = this.totalDonations / this.totalDonationsCount;

      let newDonation: UserData = {
        id: data.newDonation.Id,
        name: data.user.Nombre,
        telefono: data.user.Telefono,
        email: data.user.Email,
        producto: data.newDonation.Producto,
        cantidad: data.newDonation.Cantidad,
        estado: data.newDonation.Estado,
        highlight: true,
      };

      let newData = [newDonation, ...this.dataSource.data];
      this.dataSource.data = newData;

      this.calculateProductMostDonated(newData);
      this.calculateTopDonor(newData);

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

  toggleCheckboxes() {
    this.checkboxEnabled = !this.checkboxEnabled;
    if (this.checkboxEnabled) {
      this.displayedColumns.push('select');
    } else {
      this.displayedColumns = this.displayedColumns.filter(column => column !== 'select');
    }
  }

  updateSelectedStates() {
    this.selectedDonations = this.dataSource.data
      .filter(row => row.selected)
      .map(row => row.id); // Recolectar IDs de donaciones seleccionadas

    this.donationsService.updateDonationsState(this.selectedDonations, 'Recibido')
      .subscribe(
        response => {
          console.log('Estado actualizado con éxito', response);
          // Actualizar la interfaz de usuario según sea necesario...
          this.dataSource.data.forEach(row => {
            if (row.selected) {
              row.estado = 'Recibido';
              row.selected = false; // Desmarcar el checkbox
            }
          });
          this.checkboxEnabled = false; // Deshabilitar los checkboxes después de actualizar los estados
          this.displayedColumns = this.displayedColumns.filter(column => column !== 'select'); // Ocultar la columna de selección
        },
        error => {
          console.error('Error al actualizar el estado', error);
        }
      );
  }

  exportAsExcel() {
    const dataToExport = this.dataSource.data.map(donation => ({
      Nombre: donation.name,
      Teléfono: donation.telefono,
      Email: donation.email,
      Producto: donation.producto,
      Cantidad: donation.cantidad
    }));

    // Add a blank row
    dataToExport.push({
      Nombre: '',
      Teléfono: '',
      Email: '',
      Producto: ''
    } as any);

    // Add totals, averages, and most donated product at the end of the data
    dataToExport.push({
      Nombre: 'Total',
      Teléfono: '',
      Email: '',
      Producto: '',
      Cantidad: this.totalDonations
    });

    let amountFixed = parseFloat(this.averageDonations.toFixed(2));

    dataToExport.push({
      Nombre: 'Promedio de Productos donados',
      Teléfono: '',
      Email: '',
      Producto: '',
      Cantidad: amountFixed
    });

    if (this.productMostDonate) {
      dataToExport.push({
        Nombre: 'Producto más donado',
        Teléfono: '',
        Email: '',
        Producto: this.productMostDonate.product,
        Cantidad: this.productMostDonate.amount
      });
    }

    if (this.topDonor) {
      dataToExport.push({
        Nombre: 'Mayor donante',
        Teléfono: '',
        Email: '',
        Producto: this.topDonor.name,
        Cantidad: this.topDonor.amount
      });
    }

    const worksheet: XLSX.WorkSheet = XLSX.utils.json_to_sheet(dataToExport);
    this.applyExcelStyles(worksheet);
    const workbook: XLSX.WorkBook = { Sheets: { 'Donaciones': worksheet }, SheetNames: ['Donaciones'] };
    const excelBuffer: any = XLSX.write(workbook, { bookType: 'xlsx', type: 'array' });
    this.saveAsExcelFile(excelBuffer, 'donaciones');
  }

  private saveAsExcelFile(buffer: any, fileName: string): void {
    const EXCEL_TYPE = 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=UTF-8';
    const data: Blob = new Blob([buffer], { type: EXCEL_TYPE });
    const currentDate = new Date();
    const formattedDate = `${currentDate.getFullYear()}${(currentDate.getMonth() + 1).toString().padStart(2, '0')}${currentDate.getDate().toString().padStart(2, '0')}`;
    const formattedTime = `${currentDate.getHours().toString().padStart(2, '0')}${currentDate.getMinutes().toString().padStart(2, '0')}${currentDate.getSeconds().toString().padStart(2, '0')}`;
    saveAs(data, `${fileName}_export_${formattedDate}_${formattedTime}.xlsx`);
  }

  private applyExcelStyles(worksheet: XLSX.WorkSheet) {
    const range = XLSX.utils.decode_range(worksheet['!ref']!);

    // Apply styles to header row
    for (let C = range.s.c; C <= range.e.c; ++C) {
      const address = XLSX.utils.encode_col(C) + '1'; // Get the cell address
      if (!worksheet[address]) continue;
      if (!worksheet[address].s) worksheet[address].s = {};
      worksheet[address].s = {
        font: { bold: true, color: { rgb: "FFFFFF" } },
        fill: { fgColor: { rgb: "4CAF50" } }, // Green background
        alignment: { horizontal: "center" }
      };
    }

    // Apply styles to total, average, most donated product, and top donor rows
    for (let R = range.e.r - 4; R <= range.e.r; ++R) {
      for (let C = range.s.c; C <= range.e.c; ++C) {
        const address = XLSX.utils.encode_cell({ r: R, c: C });
        if (!worksheet[address]) continue;
        if (!worksheet[address].s) worksheet[address].s = {};
        worksheet[address].s = {
          font: { bold: true, color: { rgb: "FFFFFF" } },
          fill: { fgColor: { rgb: "4CAF50" } }, // Green background
          alignment: { horizontal: "center" }
        };
      }
    }

    // Apply styles to data cells
    for (let R = range.s.r + 1; R <= range.e.r - 5; ++R) {
      for (let C = range.s.c; C <= range.e.c; ++C) {
        const address = XLSX.utils.encode_cell({ r: R, c: C });
        if (!worksheet[address]) continue;
        if (!worksheet[address].s) worksheet[address].s = {};
        worksheet[address].s.alignment = { horizontal: "center" };
      }
    }

    // Set column widths
    worksheet['!cols'] = [
      { wch: 20 }, { wch: 20 }, { wch: 30 }, { wch: 20 }, { wch: 15 }
    ];
  }
}

function normalizeProductName(product: string): string {
  if (product.toLowerCase().endsWith('s')) {
    return product.slice(0, -1).toLowerCase();
  }
  return product.toLowerCase();
}