export interface Provinces {
    cantidad:   number;
    inicio:     number;
    parametros: Parametros;
    provincias: Province[];
    total:      number;
}

export interface Parametros {
}

export interface Province {
    centroide: Centroide;
    id:        string;
    nombre:    string;
}

export interface Centroide {
    lat: number;
    lon: number;
}