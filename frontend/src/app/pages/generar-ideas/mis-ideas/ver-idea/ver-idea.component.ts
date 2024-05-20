import { Component, OnInit, AfterViewInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { switchMap, tap } from 'rxjs';
import { ResponseIdeaService } from 'src/app/services/response-idea.service';
import * as jsPDF from 'jspdf';
import 'jspdf-autotable';
import autoTable from 'jspdf-autotable';
@Component({
  selector: 'app-ver-idea',
  templateUrl: './ver-idea.component.html',
  styleUrls: ['./ver-idea.component.css'],
})
export class VerIdeaComponent implements OnInit, AfterViewInit {
  idea: {
    titulo?: string;
    [key: string]: any;
    usuarioId?: number;
    pasos?: any;
  } = {};

  logoBase64!: string;

  constructor(
    private route: ActivatedRoute,
    private responseIdeaService: ResponseIdeaService
  ) {}
  ngAfterViewInit(): void {
    this.loadImage('assets/logos/recrea.jpg')
      .then((base64) => {
        this.logoBase64 = base64;
      })
      .catch((error) => {
        console.error('Error loading image:', error);
      });
  }

  loadImage(url: string): Promise<string> {
    return new Promise((resolve, reject) => {
      const img = new Image();
      img.crossOrigin = 'Anonymous';
      img.onload = () => {
        const canvas = document.createElement('canvas');
        canvas.width = img.width;
        canvas.height = img.height;
        const context = canvas.getContext('2d');
        if (context) {
          context.drawImage(img, 0, 0);
          const base64 = canvas.toDataURL('image/jpeg');
          resolve(base64);
        } else {
          reject('Failed to get 2D context');
        }
      };
      img.onerror = reject;
      img.src = url;
    });
  }

  ngOnInit(): void {
    this.route.params.subscribe((params) => {
      const ideaId = params['id'];
      this.responseIdeaService.getIdea(ideaId).subscribe(
        (data) => {
          this.idea = data;
          console.log('Idea cargada:', this.idea);
        },
        (error) => {
          console.error('Error al cargar la idea:', error);
        }
      );
    });
  }
  downloadPDF() {
    if (!this.logoBase64) {
      console.error('Logo not loaded yet');
      return;
    }

    const doc = new jsPDF.default();

    const logoWidth = 20; // Ancho del logo ajustado
    const logoHeight = 6; // Alto del logo ajustado
    doc.addImage(this.logoBase64, 'JPEG', 10, 10, logoWidth, logoHeight);

    doc.setFontSize(18);
    const tituloMayusculas = this.idea.titulo?.toUpperCase();
    doc.text('' + tituloMayusculas, 100, 40, { align: 'center' });

    const pasos = this.idea.pasos.map((paso: any, index: number) => [
      paso.descripcion,
    ]);

    autoTable(doc, {
      head: [['Descripción de los Pasos:']],
      body: pasos,
      startY: 60,
      theme: 'striped',
      headStyles: { fillColor: [41, 128, 185] },
      bodyStyles: { fillColor: [245, 245, 245], 
        cellPadding: 4 ,
      },
      styles: {
        cellPadding: 2,
      },
      margin: { top: 2 }, // Ajusta el margen superior de la tabla
    });

    doc.save('idea-' + this.idea.titulo + '.pdf');
  }
}
