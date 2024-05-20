export interface Idea {
    titulo:    string;
    usuarioId: number;
    pasos:     Paso[];
}

export interface Paso {
    id:          number;
    pasoNum:     number;
    descripcion: string;
    ideaId:      number;
}