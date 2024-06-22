import { Component, OnInit } from '@angular/core';
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
  newCampaign: Partial<Campaign> = {};

  constructor(
    private campaignService: CampaignService,
    private organizationService: OrganizationService,
    private authService: AuthService
  ) { }

  ngOnInit(): void {
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
    if (this.newCampaign.title && this.newCampaign.startDate && this.newCampaign.endDate) {
      const campaignToAdd: Campaign = {
        ...this.newCampaign,
        organizacionId: this.organizationId
      } as Campaign;
      
      this.campaignService.createCampaign(campaignToAdd).subscribe(
        data => {
          this.campaigns.push(data);
          this.newCampaign = {}; // Reset the form
        },
        error => console.error(error)
      );
    } else {
      console.error('All fields are required');
    }
  }

  updateCampaign(campaign: Campaign): void {
    campaign.startDate = new Date(campaign.startDate);
    campaign.endDate = new Date(campaign.endDate);
    this.campaignService.updateCampaign(campaign).subscribe(
      data => {
        const index = this.campaigns.findIndex(c => c.id === data.id);
        if (index !== -1) {
          this.campaigns[index] = data;
        }
      },
      error => console.error(error)
    );
  }

  deleteCampaign(campaignId: number): void {
    this.campaignService.deleteCampaign(campaignId).subscribe(
      () => this.campaigns = this.campaigns.filter(c => c.id !== campaignId),
      error => console.error(error)
    );
  }
}