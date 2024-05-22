import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { OrganizacionService } from 'src/app/services/organizacion.service';

@Component({
  selector: 'app-donaciones',
  templateUrl: './donaciones.component.html',
  styleUrls: ['./donaciones.component.css']
})
export class DonacionesComponent implements OnInit {

  organizaciones!:any[];

  constructor(private organizacionService:OrganizacionService,
    private router:Router
  ){

  }

  ngOnInit(): void {
    this.organizacionService.getAllOrganizaciones()
    .subscribe((resp:any[]) =>{
      this.organizaciones = resp.filter(u => u.infoOrganizacion != null);
    })
  }

  verDetalle(org:any){
    this.router.navigate(['/info-organizacion', org.id]);
  }
}
