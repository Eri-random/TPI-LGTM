import { Component } from '@angular/core';
import { FormGroup } from '@angular/forms';

@Component({
  selector: 'app-generar-ideas',
  templateUrl: './generar-ideas.component.html',
  styleUrls: ['./generar-ideas.component.css']
})
export class GenerarIdeasComponent {
  imagePreviews: string[] = ['https://media.istockphoto.com/id/1226328537/es/vector/soporte-de-posici%C3%B3n-de-imagen-con-un-icono-de-c%C3%A1mara-gris.jpg?s=612x612&w=0&k=20&c=8igCt_oe2wE-aP0qExUDfwicSNUCb4Ho9DiKCq0rSaA='];
  tipoDeTela: string = '';
  color: string = '';
  largo: number = 0;
  ancho: number = 0;
  infoExtra: string = '';
  imageFiles: File[] = [];

  constructor() { }

  onFileChange(event: any, index: number) {
    const files = event.target.files;

    if (!files || files.length === 0 || !window.FileReader) return;

    const reader = new FileReader();
    reader.readAsDataURL(files[0]);

    //this.imageFiles[index] = files[0];
    //console.log("Nombre del archivo:", files[0].name);

    reader.onloadend = () => {
      this.imagePreviews[index] = reader.result as string;
    };
  }

  addImage() {
    if (this.imagePreviews.length < 4) {
      this.imagePreviews.push('https://media.istockphoto.com/id/1226328537/es/vector/soporte-de-posici%C3%B3n-de-imagen-con-un-icono-de-c%C3%A1mara-gris.jpg?s=612x612&w=0&k=20&c=8igCt_oe2wE-aP0qExUDfwicSNUCb4Ho9DiKCq0rSaA=');
    } else {
      alert("Solo se permiten hasta 4 imágenes.");
    }
  }

  removeImage(index: number) {
    this.imagePreviews.splice(index, 1);
  }

  submitForm() {
    const formData = new FormData();
    // for (let i = 0; i < this.imagePreviews.length; i++) {
    //   const imageBlob = this.dataURItoBlob(this.imagePreviews[i]);
    //   formData.append('images[]', imageBlob, 'image' + i + '.png');
    //   console.log(imageBlob);
    // }
    console.log(this.imageFiles);
    console.log(this.tipoDeTela);
    console.log(this.tipoDeTela);
    console.log(this.color);
    console.log(String(this.largo));
    console.log(String(this.ancho));
    console.log(this.infoExtra);
  }

  // Función para convertir una URL de imagen base64 en un Blob
  dataURItoBlob(dataURI: string): Blob {
    const byteString = atob(dataURI.split(',')[1]);
    const arrayBuffer = new ArrayBuffer(byteString.length);
    const intArray = new Uint8Array(arrayBuffer);
    for (let i = 0; i < byteString.length; i++) {
      intArray[i] = byteString.charCodeAt(i);
    }
    const blob = new Blob([intArray], { type: 'image/png' });
    return blob;
  }
}
