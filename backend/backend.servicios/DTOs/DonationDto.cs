namespace backend.servicios.DTOs
{
    public class DonationDto
    {
        public int Id { get; set; }

        public string Producto { get; set; }

        public int Cantidad { get; set; }

        public int UsuarioId { get; set; }

        public int OrganizacionId { get; set; }

        public string Estado { get; set; }

        public DateTime ?Fecha { get; set; }

        public string ?Cuit { get; set; }

        public virtual OrganizationDto Organizacion { get; set; }

        public virtual UserDto Usuario { get; set; }
    }
}
