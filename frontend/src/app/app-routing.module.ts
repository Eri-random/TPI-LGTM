import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LandingComponent } from './pages/landing/landing.component';
import { LoginComponent } from './auth/login/login.component';
import { SignupComponent } from './auth/signup/signup.component';
import { GenerarIdeasComponent} from './pages/generar-ideas/generar-ideas.component';
import { MapaOrganizacionesComponent } from './pages/mapa/mapa-organizaciones.component';
import { DonacionesComponent } from './pages/donaciones/donaciones.component';
import { DashboardComponent } from './pages/dashboard/dashboard.component';
import { InfoOrganizacionComponent } from './pages/info-organizacion/info-organizacion.component';
import { ResponseIdeaComponent } from './pages/generar-ideas/response-idea/response-idea.component';
import { MisIdeasComponent } from './pages/generar-ideas/mis-ideas/mis-ideas.component';
import { VerIdeaComponent } from './pages/generar-ideas/mis-ideas/ver-idea/ver-idea.component';
import { PedidoDeOrganizacionComponent } from './pages/pedido-de-organizacion/pedido-de-organizacion.component';
import { SedeComponent } from './pages/sede/sede.component';
import { CrearSedeComponent } from './pages/sede/crear-sede/crear-sede.component';
import { EditarSedeComponent } from './pages/sede/editar-sede/editar-sede.component';
import { MiOrganizacionComponent } from './pages/mi-organizacion/mi-organizacion.component';
import { authGuard } from './guards/auth.guard';
import { PageNotFoundComponent } from './pages/page-not-found/page-not-found.component';


const routes: Routes = [
  {path:'',component:LandingComponent, pathMatch:'full'},
  {path:'login',component:LoginComponent},
  {path:'signup',component:SignupComponent},
  {path: 'generar-ideas', component:GenerarIdeasComponent, canActivate: [authGuard], data: { expectedRole: 'usuario' }},
  {path: 'ubicaciones', component:MapaOrganizacionesComponent},
  {path: 'donar', component:DonacionesComponent},
  {path: 'dashboard', component: DashboardComponent, canActivate: [authGuard], data: { expectedRole: 'organizacion' }},
  {path: 'mi-organizacion', component: MiOrganizacionComponent, canActivate: [authGuard], data: { expectedRole: 'organizacion' }},
  {path: 'info-organizacion/:id', component: InfoOrganizacionComponent},
  {path: 'sedes', component: SedeComponent, canActivate: [authGuard], data: { expectedRole: 'organizacion' }},
  {path: 'crear-sede', component: CrearSedeComponent, canActivate: [authGuard], data: { expectedRole: 'organizacion' }},
  {path: 'editar-sede/:id', component: EditarSedeComponent, canActivate: [authGuard], data: { expectedRole: 'organizacion' }},
  {path: 'response-idea', component: ResponseIdeaComponent, canActivate: [authGuard], data: { expectedRole: 'usuario' }},
  {path: 'mis-ideas', component: MisIdeasComponent, canActivate: [authGuard], data: { expectedRole: 'usuario' }},
  {path: 'mis-ideas/:id', component: VerIdeaComponent, canActivate: [authGuard], data: { expectedRole: 'usuario' }},
  {path: 'necesidades', component: PedidoDeOrganizacionComponent, canActivate: [authGuard], data: { expectedRole: 'organizacion' }},
  {path: 'not-found',component:PageNotFoundComponent},
  {path:'**',redirectTo:''}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
