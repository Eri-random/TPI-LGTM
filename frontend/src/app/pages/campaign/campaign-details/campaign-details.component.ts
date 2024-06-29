import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CampaignService } from 'src/app/services/campaign.service';
import { DialogDonateComponent } from '../../info-organization/dialog-donate/dialog-donate.component';
import { MatDialog } from '@angular/material/dialog';

@Component({
  selector: 'app-campaign-details',
  templateUrl: './campaign-details.component.html',
  styleUrls: ['./campaign-details.component.css'],
})
export class CampaignDetailsComponent {
  campaignId!: number;
  campaign: any;
  organizationId!: number;

  constructor(
    private route: ActivatedRoute,
    private campaignService: CampaignService,
    private router: Router,
    public dialog: MatDialog,
  ) {}

  ngOnInit(): void {
    this.campaignId = this.route.snapshot.params['id'];
    this.getCampaignDetails();
  }

  getCampaignDetails() {
    this.campaignService.getIdCampaign(this.campaignId).subscribe(
      (data) => {
        this.campaign = data;
        this.organizationId = data.organizacionId;
      },
      (error) => console.error(error)
    );
  }

  goBack() {
    const organizationId = this.organizationId;
    this.router.navigate([`/info-organizacion/${organizationId}`]);
  }

  openDialog(): void {
    this.dialog.open(DialogDonateComponent, {
      width: 'auto',
      height: '75%',
      data: { organizacionId: this.organizationId }
    });
  }
}
