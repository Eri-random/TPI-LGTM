export interface Idea {
    titulo:    string;
    usuarioId: number;
    dificultad: string;
    pasos:     Paso[];
}

export interface Paso {
    id:          number;
    pasoNum:     number;
    descripcion: string;
    ideaId:      number;
}