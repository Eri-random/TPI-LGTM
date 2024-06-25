import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CampaignService, Campaign } from '././../../services/campaign.service';
import { OrganizationService } from 'src/app/services/organization.service';
import { AuthService } from 'src/app/services/auth.service';
import { switchMap } from 'rxjs';

@Component({
  selector: 'app-campaigns',
  templateUrl: './campaigns.component.html',
  styleUrls: ['./campaigns.component.css']
})
export class CampaignsComponent implements OnInit {

  campaigns: Campaign[] = [];
  organizationId!: number;
  campaignForm!: FormGroup;
  isLogged: boolean = true; // Adjust based on your auth logic

  constructor(
    private fb: FormBuilder,
    private campaignService: CampaignService,
    private organizationService: OrganizationService,
    private authService: AuthService
  ) {
    this.campaignForm = this.fb.group({
      title: ['', Validators.required],
      startDate: ['', Validators.required],
      endDate: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    this.isLogged = this.authService.isLoggedIn(); // Adjust based on your auth logic
    this.organizationService.getCuitFromStore().subscribe((val) => {
      const cuitFromToken = this.authService.getCuitFromToken();
      const cuit = val || cuitFromToken;
      this.loadCampaigns(cuit);
    });
  }

  loadCampaigns(cuit: string): void {
    this.organizationService
      .getOrganizationByCuit(cuit)
      .pipe(
        switchMap(({ id }) => {
          this.organizationId = id;
          return this.campaignService.getAllCampaigns(id.toString());
        })
      )
      .subscribe(
        data => this.campaigns = data,
        error => console.error(error)
      );
  }

  addCampaign(): void {
    if (this.campaignForm.valid) {
      const newCampaign: Campaign = {
        ...this.campaignForm.value,
        organizacionId: this.organizationId
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

  get fm() { return this.campaignForm.controls; }
}
