export interface Provincias {
    cantidad:   number;
    inicio:     number;
    parametros: Parametros;
    provincias: Provincia[];
    total:      number;
}

export interface Parametros {
}

export interface Provincia {
    centroide: Centroide;
    id:        string;
    nombre:    string;
}

export interface Centroide {
    lat: number;
    lon: number;
}