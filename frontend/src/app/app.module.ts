import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatOptionModule } from '@angular/material/core';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { BrowserAnimationsModule, NoopAnimationsModule } from '@angular/platform-browser/animations';
import { CommonModule } from '@angular/common';
import { MatInputModule } from '@angular/material/input';

import { NavbarComponent } from './shared/navbar/navbar.component';
import { FooterComponent } from './shared/footer/footer.component';
import { LoginComponent } from './auth/login/login.component';
import { SignupComponent } from './auth/signup/signup.component';
import { LandingComponent } from './pages/landing/landing.component';
import { NgToastModule } from 'ng-angular-popup';
import { GenerarIdeasComponent } from './pages/generar-ideas/generar-ideas.component';
import { MapaOrganizacionesComponent } from './pages/mapa/mapa-organizaciones.component';
import { DonacionesComponent } from './pages/donaciones/donaciones.component';
import { DashboardComponent } from './pages/dashboard/dashboard.component';
import { ModalOrganizacionComponent } from './components/modal-organizacion/modal-organizacion.component';
import { MatDialogModule} from '@angular/material/dialog';
import { MatButtonModule} from '@angular/material/button';
import { EditInfoComponent } from './pages/dashboard/components/edit-info/edit-info.component';
import { InfoOrganizacionComponent } from './pages/info-organizacion/info-organizacion.component';
import { SpinnerComponent } from './components/spinner/spinner.component';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ResponseIdeaComponent } from './pages/generar-ideas/response-idea/response-idea.component';
import { register } from 'swiper/element/bundle';
import { MisIdeasComponent } from './pages/generar-ideas/mis-ideas/mis-ideas.component';
import { VerIdeaComponent } from './pages/generar-ideas/mis-ideas/ver-idea/ver-idea.component';
import { SpinnerIdeaComponent } from './components/spinner-idea/spinner-idea.component'
import { EditorModule } from '@tinymce/tinymce-angular';

register();
@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    SignupComponent,
    NavbarComponent,
    FooterComponent,
    LandingComponent,
    GenerarIdeasComponent,
    MapaOrganizacionesComponent,
    DonacionesComponent,
    DashboardComponent,
    ModalOrganizacionComponent,
    EditInfoComponent,
    InfoOrganizacionComponent,
    SpinnerComponent,
    ResponseIdeaComponent,
    MisIdeasComponent,
    VerIdeaComponent,
    SpinnerIdeaComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    ReactiveFormsModule,
    MatTableModule,
    HttpClientModule,
    NgToastModule,
    FormsModule,
    MatFormFieldModule,
    MatSelectModule,
    MatOptionModule,
    MatPaginatorModule,
    MatSortModule,  
    BrowserAnimationsModule,
    NoopAnimationsModule,
    CommonModule,
    MatInputModule,
    MatButtonModule,
    MatDialogModule,
    MatProgressSpinnerModule,
    EditorModule
  ],
  providers: [],
  bootstrap: [AppComponent],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class AppModule {}
