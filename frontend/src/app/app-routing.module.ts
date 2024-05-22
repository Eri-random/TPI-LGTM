import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LandingComponent } from './pages/landing/landing.component';
import { LoginComponent } from './auth/login/login.component';
import { SignupComponent } from './auth/signup/signup.component';
import { GenerarIdeasComponent} from './pages/generar-ideas/generar-ideas.component';
import { MapaOrganizacionesComponent } from './pages/mapa/mapa-organizaciones.component';
import { DonacionesComponent } from './pages/donaciones/donaciones.component';
import { DashboardComponent } from './pages/dashboard/dashboard.component';
import { EditInfoComponent } from './pages/dashboard/components/edit-info/edit-info.component';
import { InfoOrganizacionComponent } from './pages/info-organizacion/info-organizacion.component';
import { ResponseIdeaComponent } from './pages/generar-ideas/response-idea/response-idea.component';
import { MisIdeasComponent } from './pages/generar-ideas/mis-ideas/mis-ideas.component';
import { VerIdeaComponent } from './pages/generar-ideas/mis-ideas/ver-idea/ver-idea.component';
import { PedidoDeOrganizacionComponent } from './pages/pedido-de-organizacion/pedido-de-organizacion.component';


const routes: Routes = [
  {path:'',component:LandingComponent, pathMatch:'full'},
  {path:'login',component:LoginComponent},
  {path:'signup',component:SignupComponent},
  {path: 'generar-ideas', component:GenerarIdeasComponent},
  {path: 'ubicaciones', component:MapaOrganizacionesComponent},
  {path: 'donar', component:DonacionesComponent},
  {path: 'dashboard', component: DashboardComponent},
  {path: 'mi-organizacion', component: EditInfoComponent},
  {path: 'info-organizacion/:id', component: InfoOrganizacionComponent},
  {path: 'response-idea', component: ResponseIdeaComponent},
  {path: 'mis-ideas', component: MisIdeasComponent},
  {path: 'mis-ideas/:id', component: VerIdeaComponent},
  {path: 'necesidades', component: PedidoDeOrganizacionComponent},
  {path:'**',redirectTo:''}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
