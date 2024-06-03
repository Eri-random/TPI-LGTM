import { Component, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { ActivatedRoute } from '@angular/router';
import { OrganizationService } from 'src/app/services/organization.service';
import { DialogDonateComponent } from './dialog-donate/dialog-donate.component';

@Component({
  selector: 'app-info-organization',
  templateUrl: './info-organization.component.html',
  styleUrls: ['./info-organization.component.css']
})
export class InfoOrganizationComponent implements OnInit {
  donateForm!: FormGroup;
  organization:any;
  needs:any;
  safeContent!: SafeHtml;

  constructor(
    private organizationService:OrganizationService,
    private sanitizer: DomSanitizer,
    public dialog: MatDialog,
    private route: ActivatedRoute,
  ) {
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
      this.organizationService.getGroupedSubcategories(organizacionId)
      .subscribe(resp=>{
        this.needs = resp;
      },error =>{
        console.log(error);
      })
    });
  }

  sanitizeContent(content: string): SafeHtml {
    return this.sanitizer.bypassSecurityTrustHtml(content);
  }

  openDialog(): void {
    this.dialog.open(DialogDonateComponent, {
      width: 'auto',
      height: '75%',
      data:{organizacionId:this.organization.id}
    });
  }
}
