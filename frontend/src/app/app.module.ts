import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatNativeDateModule, MatOptionModule } from '@angular/material/core';
import { MatPaginatorIntl, MatPaginatorModule } from '@angular/material/paginator';
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
import { GenerateIdeasComponent } from './pages/generate-ideas/generate-ideas.component';
import { MapOrganizationsComponent } from './pages/map/map-organizations.component';
import { DashboardComponent } from './pages/dashboard/dashboard.component';
import { MatDialogModule} from '@angular/material/dialog';
import { MatButtonModule} from '@angular/material/button';
import { InfoOrganizationComponent } from './pages/info-organization/info-organization.component';
import { SpinnerComponent } from './components/spinner/spinner.component';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ResponseIdeaComponent } from './pages/generate-ideas/response-idea/response-idea.component';
import { register } from 'swiper/element/bundle';
import { MyIdeasComponent } from './pages/generate-ideas/my-ideas/my-ideas.component';
import { SeeIdeaComponent } from './pages/generate-ideas/my-ideas/see-idea/see-idea.component';
import { SpinnerIdeaComponent } from './components/spinner-idea/spinner-idea.component'
import { EditorModule } from '@tinymce/tinymce-angular';
import { OrganizationRequestComponent } from './pages/organization-request/organization-request.component';
import { SedeComponent } from './pages/sede/headquarters.component';
import { CreateHeadquartersComponent } from './pages/sede/create-headquarters/create-headquarters.component';
import { EditHeadquartersComponent } from './pages/sede/edit-headquarters/edit-headquarters.component'
import { MyOrganizationComponent } from './pages/my-organization/my-organization.component';
import { DialogDonateComponent } from './pages/info-organization/dialog-donate/dialog-donate.component';
import { PageNotFoundComponent } from './pages/page-not-found/page-not-found.component';
import { DonationsComponent } from './pages/donations/donations.component';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatIconModule } from '@angular/material/icon';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { CustomMatPaginatorIntl } from './components/custom-mat-paginator-intl/custom-mat-paginator-intl.component';
import { UpdateAccountComponent } from './pages/update-account/update-account.component';
import { TokenInterceptor } from './interceptors/token.interceptor';
register();
@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    SignupComponent,
    NavbarComponent,
    FooterComponent,
    LandingComponent,
    GenerateIdeasComponent,
    MapOrganizationsComponent,
    DonationsComponent,
    DashboardComponent,
    DialogDonateComponent,
    InfoOrganizationComponent,
    SpinnerComponent,
    ResponseIdeaComponent,
    MyIdeasComponent,
    SeeIdeaComponent,
    SpinnerIdeaComponent,
    SedeComponent,
    CreateHeadquartersComponent,
    EditHeadquartersComponent,
    MyOrganizationComponent,
    OrganizationRequestComponent,
    PageNotFoundComponent,
    UpdateAccountComponent,
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
  providers: [
      { provide: MatPaginatorIntl, useClass: CustomMatPaginatorIntl },
      { provide:HTTP_INTERCEPTORS, useClass:TokenInterceptor, multi:true}
  ],
  bootstrap: [AppComponent],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class AppModule {}
