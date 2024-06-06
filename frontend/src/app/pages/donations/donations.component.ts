import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NeedService } from 'src/app/services/need.service';
import { OrganizationService } from 'src/app/services/organization.service';

@Component({
  selector: 'app-donations',
  templateUrl: './donations.component.html',
  styleUrls: ['./donations.component.css'],
})
export class DonationsComponent implements OnInit {
  organizations: any[] = [];
  needs: any[] = [];
  selectedNeeds: any = {};
  selectedSubcategories: any = {};
  searchName: string = '';
  page: number = 1;
  pageSize: number = 8;
  showSeeMore: boolean = true;
  noResultsFound: boolean = false;
  loading: boolean = true;

  constructor(
    private organizacionService: OrganizationService,
    private needsService: NeedService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.uploadOrganizations();
    this.loadNeeds();
  }

  uploadOrganizations(): void {
    const selectedSubcategoryIds = this.getSelectedSubcategoryIds();

    const params = {
      page: this.page.toString(),
      pageSize: this.pageSize.toString(),
      subcategoriaIds: selectedSubcategoryIds.join(','),
      name: this.searchName
    };

    this.organizacionService.getPaginatedOrganizations(params).subscribe((resp: any[]) => {
      this.organizations = this.organizations.concat(resp);
      if (resp.length < this.pageSize) {
        this.showSeeMore = false;
      }
      setTimeout(() => {
        this.loading = false;
      }, 1000);
    });
  }

  loadNeeds() {
    this.needsService.getAllNeeds().subscribe((data) => {
      this.needs = data;
    });
  }

  applyFilter() {
    const selectedSubcategoryIds = this.getSelectedSubcategoryIds();
    this.page = 1; // Reset page to 1 when applying filters

    const params = {
      page: this.page.toString(),
      pageSize: this.pageSize.toString(),
      subcategoriaIds: selectedSubcategoryIds.join(','),
      name: this.searchName
    };

    this.organizacionService.getPaginatedOrganizations(params).subscribe((resp: any) => {
      this.organizations = resp;
      this.noResultsFound = resp.length === 0;
      this.showSeeMore = resp.length === this.pageSize;
    });
  }

  getSelectedSubcategoryIds(): number[] {
    return Object.keys(this.selectedSubcategories)
    .filter(key => this.selectedSubcategories[+key])
    .map(Number);
  }

  loadMore(): void {
    this.page++;
    this.uploadOrganizations();
  }

  seeDetail(org: any): void {
    this.router.navigate(['/info-organizacion', org.id]);
  }
}
