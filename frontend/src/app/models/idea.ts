export interface Idea {
    titulo:    string;
    usuarioId: number;
    dificultad: string;
    pasos:     Paso[];
    imageUrl: string;
}

export interface Paso {
    id:          number;
    pasoNum:     number;
    descripcion: string;
    ideaId:      number;
}