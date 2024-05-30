import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatNativeDateModule, MatOptionModule } from '@angular/material/core';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { BrowserAnimationsModule, NoopAnimationsModule } from '@angular/platform-browser/animations';
import { CommonModule, JsonPipe } from '@angular/common';
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
import { MatDialogModule} from '@angular/material/dialog';
import { MatButtonModule} from '@angular/material/button';
import { InfoOrganizacionComponent } from './pages/info-organizacion/info-organizacion.component';
import { SpinnerComponent } from './components/spinner/spinner.component';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ResponseIdeaComponent } from './pages/generar-ideas/response-idea/response-idea.component';
import { register } from 'swiper/element/bundle';
import { MisIdeasComponent } from './pages/generar-ideas/mis-ideas/mis-ideas.component';
import { VerIdeaComponent } from './pages/generar-ideas/mis-ideas/ver-idea/ver-idea.component';
import { SpinnerIdeaComponent } from './components/spinner-idea/spinner-idea.component'
import { EditorModule } from '@tinymce/tinymce-angular';
import { PedidoDeOrganizacionComponent } from './pages/pedido-de-organizacion/pedido-de-organizacion.component';
import { SedeComponent } from './pages/sede/sede.component';
import { CrearSedeComponent } from './pages/sede/crear-sede/crear-sede.component';
import { EditarSedeComponent } from './pages/sede/editar-sede/editar-sede.component'
import { MiOrganizacionComponent } from './pages/mi-organizacion/mi-organizacion.component';
import { DialogDonarComponent } from './pages/info-organizacion/dialog-donar/dialog-donar.component';
import { PageNotFoundComponent } from './pages/page-not-found/page-not-found.component';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatIconModule } from '@angular/material/icon';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatCheckboxModule } from '@angular/material/checkbox';
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
    DialogDonarComponent,
    InfoOrganizacionComponent,
    SpinnerComponent,
    ResponseIdeaComponent,
    MisIdeasComponent,
    VerIdeaComponent,
    SpinnerIdeaComponent,
    SedeComponent,
    CrearSedeComponent,
    EditarSedeComponent,
    MiOrganizacionComponent,
    PedidoDeOrganizacionComponent,
    PageNotFoundComponent,
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
    EditorModule,
    MatExpansionModule,
    MatIconModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatCheckboxModule, JsonPipe
  ],
  providers: [],
  bootstrap: [AppComponent],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class AppModule {}
