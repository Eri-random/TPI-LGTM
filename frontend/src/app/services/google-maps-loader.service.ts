import { Injectable } from '@angular/core';
import { environments } from '../../environments/environments';

@Injectable({
  providedIn: 'root',
})
export class GoogleMapsLoaderService {
  private mapsLoaded = false;
  private mapsLoadPromise: Promise<void> | undefined;

  private API_KEY = environments.googleMapsApiKey;

  constructor() {}

  load(): Promise<void> {
    if (this.mapsLoaded) {
      return Promise.resolve();
    }

    if (this.mapsLoadPromise) {
      return this.mapsLoadPromise;
    }

    const script = document.createElement('script');
    script.src =
      'https://maps.googleapis.com/maps/api/js?key=' +
      this.API_KEY +
      '&libraries=places';
    script.async = true;
    script.defer = true;

    this.mapsLoadPromise = new Promise<void>((resolve, reject) => {
      script.onload = () => {
        this.mapsLoaded = true;
        resolve();
      };

      script.onerror = (error) => reject(error);
    });

    document.body.appendChild(script);

    return this.mapsLoadPromise;
  }
}
