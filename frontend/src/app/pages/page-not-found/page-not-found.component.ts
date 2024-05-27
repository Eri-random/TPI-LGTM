import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-page-not-found',
  templateUrl: './page-not-found.component.html',
  styleUrls: ['./page-not-found.component.css']
})
export class PageNotFoundComponent implements OnInit {

  rol!:string;

  constructor(private authService:AuthService,
    private router:Router
  ){}

  ngOnInit(): void {
    this.rol = this.authService.getRoleFromToken();
  }

  goBack(){
    const roleRouteMap: { [key: string]: string } = {
      'organizacion': '/dashboard',
      'usuario': '/generar-ideas',
      // Agrega más roles y sus rutas aquí
    };

    const route = roleRouteMap[this.rol] || '';
    this.router.navigate([route]);
  }

}
