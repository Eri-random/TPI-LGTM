import { Injectable } from '@angular/core';
import { environments } from '../../environments/environments';
import { HttpClient } from '@angular/common/http';
import { of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class GenerateIdeaService {

  private baseUrl = environments.baseUrl;

  constructor(private http: HttpClient) { }

  postGenerateIdea(message: string){
    // const url = `${this.baseUrl}/Idea/generate`;
    // return this.http.post<any>(url, {message});

    const response = {
      "idea": "Manta para mascotas de Algodón reciclada",
      "dificultad": "Media",
      "steps": [
          "Corta el trozo de algodón en rectángulos de aproximadamente 50x50 cm. Asegúrate de tener suficientes piezas para cubrir el tamaño deseado de la manta. ImageURL: https://i.ibb.co/PghnwXH/paso-1-manta-2.png",
          "Organiza las piezas de algodón en el diseño que prefieras para la manta. Puedes crear un patrón simple o más complejo dependiendo de tu habilidad y creatividad. ImageURL: https://i.ibb.co/M2NpkYB/paso-2-manta-2.png",
          "Cose las piezas de algodón juntas siguiendo el diseño que has creado, utilizando una puntada recta y un margen de costura de aproximadamente 1 cm. Comienza con las piezas más grandes para formar el cuerpo de la manta. ImageURL: https://i.ibb.co/YQGtFvz/paso-3-manta-2.png",
          "Una vez que todas las piezas estén cosidas y planchadas, revisa todas las costuras y asegura que no haya hilos sueltos. Plancha la manta completa para darle una apariencia pulida y profesional. ImageURL: https://i.ibb.co/f8TC8hQ/paso-4-manta-2.png",
      ],
      "imageUrl": "https://i.ibb.co/WkRrGTt/img-principal-manta-2.png"
    };
    return of(response);
  }
}
