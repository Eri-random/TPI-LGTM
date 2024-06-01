import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LandingComponent } from './pages/landing/landing.component';
import { LoginComponent } from './auth/login/login.component';
import { SignupComponent } from './auth/signup/signup.component';
import { GenerateIdeasComponent} from './pages/generate-ideas/generate-ideas.component';
import { MapOrganizationsComponent } from './pages/map/map-organizations.component';
import { DonationsComponent } from './pages/donations/donations.component';
import { DashboardComponent } from './pages/dashboard/dashboard.component';
import { InfoOrganizationComponent } from './pages/info-organization/info-organization.component';
import { ResponseIdeaComponent } from './pages/generate-ideas/response-idea/response-idea.component';
import { MyIdeasComponent } from './pages/generate-ideas/my-ideas/my-ideas.component';
import { SeeIdeaComponent } from './pages/generate-ideas/my-ideas/see-idea/see-idea.component';
import { OrganizationRequestComponent } from './pages/organization-request/organization-request.component';
import { SedeComponent } from './pages/sede/headquarters.component';
import { CreateHeadquartersComponent } from './pages/sede/create-headquarters/create-headquarters.component';
import { EditHeadquartersComponent } from './pages/sede/edit-headquarters/edit-headquarters.component';
import { MyOrganizationComponent } from './pages/my-organization/my-organization.component';
import { authGuard } from './guards/auth.guard';
import { PageNotFoundComponent } from './pages/page-not-found/page-not-found.component';


const routes: Routes = [
  {path:'',component:LandingComponent, pathMatch:'full'},
  {path:'login',component:LoginComponent},
  {path:'signup',component:SignupComponent},
  {path: 'generar-ideas', component:GenerateIdeasComponent, canActivate: [authGuard], data: { expectedRole: 'usuario' }},
  {path: 'ubicaciones', component:MapOrganizationsComponent},
  {path: 'donar', component:DonationsComponent},
  {path: 'dashboard', component: DashboardComponent, canActivate: [authGuard], data: { expectedRole: 'organizacion' }},
  {path: 'mi-organizacion', component: MyOrganizationComponent, canActivate: [authGuard], data: { expectedRole: 'organizacion' }},
  {path: 'info-organizacion/:id', component: InfoOrganizationComponent},
  {path: 'sedes', component: SedeComponent, canActivate: [authGuard], data: { expectedRole: 'organizacion' }},
  {path: 'crear-sede', component: CreateHeadquartersComponent, canActivate: [authGuard], data: { expectedRole: 'organizacion' }},
  {path: 'editar-sede/:id', component: EditHeadquartersComponent, canActivate: [authGuard], data: { expectedRole: 'organizacion' }},
  {path: 'response-idea', component: ResponseIdeaComponent, canActivate: [authGuard], data: { expectedRole: 'usuario' }},
  {path: 'mis-ideas', component: MyIdeasComponent, canActivate: [authGuard], data: { expectedRole: 'usuario' }},
  {path: 'mis-ideas/:id', component: SeeIdeaComponent, canActivate: [authGuard], data: { expectedRole: 'usuario' }},
  {path: 'necesidades', component: OrganizationRequestComponent, canActivate: [authGuard], data: { expectedRole: 'organizacion' }},
  {path: 'not-found',component:PageNotFoundComponent},
  {path:'**',redirectTo:''}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
