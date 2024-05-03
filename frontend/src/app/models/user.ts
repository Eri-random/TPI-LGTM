export class User{
    constructor(
        public id: number,
        public nombre: string,
        public apellido: string,
        public direccion: string,
        public provincia: string,
        public localidad: string,
        public email: string,
        public password: string,
        public rolId: number,
        public telefono?: number
    ){}
}