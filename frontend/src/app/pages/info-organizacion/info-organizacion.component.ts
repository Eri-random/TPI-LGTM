import { Component, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { ActivatedRoute } from '@angular/router';
import { OrganizacionService } from 'src/app/services/organizacion.service';
import { DialogDonarComponent } from './dialog-donar/dialog-donar.component';

@Component({
  selector: 'app-info-organizacion',
  templateUrl: './info-organizacion.component.html',
  styleUrls: ['./info-organizacion.component.css']
})
export class InfoOrganizacionComponent implements OnInit {
  donarForm!: FormGroup;
  organizacion:any;
  safeContent!: SafeHtml;

  constructor(
    private organizacionService:OrganizacionService,
    private sanitizer: DomSanitizer,
    public dialog: MatDialog,
    private route: ActivatedRoute,
  ) {
  }

  ngOnInit(): void {
    this.route.params.subscribe((params) => {
      const organizacionId = params['id'];
      this.organizacionService.getOrganizacionById(organizacionId).subscribe(
        (data) => {
          this.safeContent = this.sanitizeContent(data.infoOrganizacion.descripcionCompleta);
          this.organizacion = data;
        },
        (error) => {
          console.error(error);
        }
      );
    });
  }

  sanitizeContent(content: string): SafeHtml {
    return this.sanitizer.bypassSecurityTrustHtml(content);
  }

  openDialog(): void {
    this.dialog.open(DialogDonarComponent, {
      width: 'auto',
      height: '80%',
      data:{organizacionId:this.organizacion.id}
    });
  }
}
