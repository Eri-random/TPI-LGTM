import { Component, OnInit, ViewChild } from '@angular/core';
import { MatAccordion } from '@angular/material/expansion';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CampaignService, Campaign } from '././../../services/campaign.service';
import { NeedService } from 'src/app/services/need.service';
import { OrganizationService } from 'src/app/services/organization.service';
import { AuthService } from 'src/app/services/auth.service';
import { NgToastService } from 'ng-angular-popup';
import { switchMap } from 'rxjs';

@Component({
  selector: 'app-campaigns',
  templateUrl: './campaigns.component.html',
  styleUrls: ['./campaigns.component.css']
})
export class CampaignsComponent implements OnInit {
  @ViewChild(MatAccordion) accordion!: MatAccordion;
  campaigns: Campaign[] = [];
  needs: any[] = [];
  formGroups: { [key: string]: FormGroup } = {};
  organizationId!: number;
  campaignForm: FormGroup;
  isLogged: boolean = true;
  loading: boolean = true;

  constructor(
    private fb: FormBuilder,
    private campaignService: CampaignService,
    private organizationService: OrganizationService,
    private needService: NeedService,
    private authService: AuthService,
    private toast: NgToastService
  ) {
    this.campaignForm = this.fb.group({
      title: ['', Validators.required],
      startDate: ['', Validators.required],
      endDate: ['', Validators.required],
      needs: this.fb.array([], Validators.required)
    });
  }

  ngOnInit(): void {
    this.isLogged = this.authService.isLoggedIn();
    this.organizationService.getCuitFromStore().subscribe((val) => {
      const cuitFromToken = this.authService.getCuitFromToken();
      const cuit = val || cuitFromToken;
      this.loadCampaignsAndNeeds(cuit);
    });
    this.loadAllNeeds();
  }

  loadCampaignsAndNeeds(cuit: string): void {
    this.organizationService
      .getOrganizationByCuit(cuit)
      .pipe(
        switchMap(({ id }) => {
          this.organizationId = id;
          return this.campaignService.getAllCampaigns(id.toString());
        })
      )
      .subscribe(
        data => {
          this.campaigns = data;
          this.loading = false;
        },
        error => console.error(error)
      );
  }

  loadAllNeeds(): void {
    this.needService.getAllNeeds().subscribe(
      data => {
        this.needs = data;
        this.initializeFormGroups();
        this.loading = false;
      },
      error => console.error(error)
    );
  }

  initializeFormGroups(): void {
    this.needs.forEach(need => {
      const formGroup = this.fb.group({});
      need.subcategoria.forEach(sub => {
        formGroup.addControl(sub.nombre, this.fb.control(false)); // Initialize as unchecked
      });
      this.formGroups[need.nombre] = formGroup;
    });
  }

  addCampaign(): void {
    if (this.campaignForm.valid) {
      const selectedNeeds = this.getSelectedNeeds();

      const newCampaign: Campaign = {
        ...this.campaignForm.value,
        organizacionId: this.organizationId,
        startDate: new Date(this.campaignForm.value.startDate).toISOString(),
        endDate: new Date(this.campaignForm.value.endDate).toISOString(),
        needs: selectedNeeds
      };

      this.campaignService.createCampaign(newCampaign).subscribe(
        data => {
          this.campaigns.push(data);
          this.campaignForm.reset(); // Reset the form
        },
        error => console.error(error)
      );
    } else {
      console.error('Form is invalid');
    }
  }

  deleteCampaign(campaignId: number): void {
    this.campaignService.deleteCampaign(campaignId).subscribe(
      () => this.campaigns = this.campaigns.filter(c => c.id !== campaignId),
      error => console.error(error)
    );
  }

  getSelectedNeeds(): any[] {
    const selectedNeeds = [];
    for (const need of this.needs) {
      const selectedSubcategories = [];
      for (const subcategory of need.subcategoria) {
        if (this.formGroups[need.nombre].get(subcategory.nombre).value) {
          selectedSubcategories.push(subcategory);
        }
      }
      if (selectedSubcategories.length > 0) {
        selectedNeeds.push({ ...need, subcategoria: selectedSubcategories });
      }
    }
    return selectedNeeds;
  }

  saveNeeds(): void {
    console.log('Selected needs:', this.getSelectedNeeds());
  }

  get fm() { return this.campaignForm.controls; }
}
