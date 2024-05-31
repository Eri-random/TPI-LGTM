namespace backend.servicios.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public string Localidad { get; set; }
        public string Provincia { get; set; }
        public int Rol { get; set; }
        public string RolNombre { get; set; }
        public OrganizationDto Organizacion { get; set; }

        public double Latitud { get; set; }
        public double Longitud { get; set; }
    }
}
