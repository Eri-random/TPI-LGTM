import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { OrganizacionService } from 'src/app/services/organizacion.service';

@Component({
  selector: 'app-donations',
  templateUrl: './donations.component.html',
  styleUrls: ['./donations.component.css'],
})
export class DonationsComponent implements OnInit {
  organizations: any[] = [];
  page: number = 1;
  pageSize: number = 8;
  showSeeMore: boolean = true;

  constructor(
    private organizacionService: OrganizacionService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.uploadOrganizations();
  }

  uploadOrganizations(): void {
    this.organizacionService
      .getOrganizacionesPaginadas(this.page, this.pageSize)
      .subscribe((resp: any[]) => {
        this.organizations = this.organizations.concat(resp);
        if (resp.length < this.pageSize) {
          this.showSeeMore = false;
        }
      });
  }

  loadMore(): void {
    this.page++;
    this.uploadOrganizations();
  }

  seeDetail(org: any): void {
    this.router.navigate(['/info-organizacion', org.id]);
  }
}
