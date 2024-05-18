import { Component, OnInit } from '@angular/core';
import { OrganizacionService } from 'src/app/services/organizacion.service';

@Component({
  selector: 'app-donaciones',
  templateUrl: './donaciones.component.html',
  styleUrls: ['./donaciones.component.css']
})
export class DonacionesComponent implements OnInit {

  organizaciones!:any[];

  constructor(private organizacionService:OrganizacionService){

  }

  ngOnInit(): void {
    this.organizacionService.getAllOrganizaciones()
    .subscribe((resp:any[]) =>{
      this.organizaciones = resp.filter(u => u.infoOrganizacion != null);
    })
  }
}
