import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { ActivatedRoute } from '@angular/router';
import { OrganizationService } from 'src/app/services/organization.service';
import { DialogDonateComponent } from './dialog-donate/dialog-donate.component';
import { CampaignService, Campaign } from 'src/app/services/campaign.service';

@Component({
  selector: 'app-info-organization',
  templateUrl: './info-organization.component.html',
  styleUrls: ['./info-organization.component.css']
})
export class InfoOrganizationComponent implements OnInit {
  donateForm!: FormGroup;
  organization: any;
  campaigns: Campaign[] = [];
  safeContent!: SafeHtml;

  constructor(
    private organizationService: OrganizationService,
    private campaignService: CampaignService,
    private sanitizer: DomSanitizer,
    public dialog: MatDialog,
    private route: ActivatedRoute,
  ) { }

  @ViewChild('swiperEl', { static: false }) swiperEl!: ElementRef;

  ngAfterViewInit() {
    if (this.swiperEl) {
      const swiperElement = this.swiperEl.nativeElement;

      Object.assign(swiperElement, {
        slidesPerView: 1,
        spaceBetween: 10,
        pagination: {
          clickable: true,
        },
        breakpoints: {
          640: {
            slidesPerView: 2,
            spaceBetween: 20,
          },
          768: {
            slidesPerView: 3,
            spaceBetween: 40,
          },
          1024: {
            slidesPerView: 3,
            spaceBetween: 50,
          },
        },
      });

      swiperElement.initialize();
    }
  }

  ngOnInit(): void {
    this.route.params.subscribe((params) => {
      const organizacionId = params['id'];
      this.organizationService.getOrganizationById(organizacionId).subscribe(
        (data) => {
          this.safeContent = this.sanitizeContent(data.infoOrganizacion.descripcionCompleta);
          this.organization = data;
        },
        (error) => {
          console.error(error);
        }
      );
      this.loadCampaigns(organizacionId);
    });
  }

  loadCampaigns(organizacionId: string): void {
    this.campaignService.getAllCampaigns(organizacionId).subscribe(
      (resp) => {
        this.campaigns = resp;
      },
      (error) => {
        console.log(error);
      }
    );
  }

  sanitizeContent(content: string): SafeHtml {
    return this.sanitizer.bypassSecurityTrustHtml(content);
  }

  openDialog(): void {
    this.dialog.open(DialogDonateComponent, {
      width: 'auto',
      height: '75%',
      data: { organizacionId: this.organization.id }
    });
  }
}
