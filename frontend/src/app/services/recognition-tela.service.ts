import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environments } from '../../environments/environments';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class RecognitionTelaService {

  private baseUrl = environments.baseUrl;

  constructor(private http: HttpClient) { }

  classifyImage(image: File):Observable<any> {
    const formData = new FormData();
    formData.append("image", image, image.name);

    return this.http.post(`${this.baseUrl}/ImageClassification`, formData);
  }
}
