import { AfterViewInit, Component, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';

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
export class DashboardComponent implements AfterViewInit {
  displayedColumns: string[] = ['name', 'telefono', 'email', 'producto', 'cantidad', 'progress'];
  dataSource: MatTableDataSource<UserData>;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor() {
    const users = Array.from({ length: 100 }, (_, k) => createNewUser(k + 1));
    this.dataSource = new MatTableDataSource(users);
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
}

function createNewUser(p0?: number): UserData {
  const name = NAMES[Math.floor(Math.random() * NAMES.length)];
  const telefono = generateRandomPhoneNumber();
  const email = generateEmail(name/*, id*/);
  const producto = PRODUCTOS[Math.floor(Math.random() * PRODUCTOS.length)];
  const cantidad = Math.floor(Math.random() * 20) + 1;
  const progress = Math.random() < 0.5 ? 'Pendiente' : 'Recibido';
  return { name, telefono, email, producto, cantidad, progress };
}

function generateRandomPhoneNumber(): string {
  let phoneNumber = '';
  for (let i = 0; i < 6; i++) {
    phoneNumber += Math.floor(Math.random() * 10).toString();
  }
  return phoneNumber;
}

function generateEmail(name: string): string {
  const formattedName = name.toLowerCase().replace(/\s/g, '');
  return `${formattedName}@example.com`;
}
