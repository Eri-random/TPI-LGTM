using backend.servicios.DTOs;

namespace backend.api.Models.ResponseModels
{
    public class DonationResponseModel
    {
        public int Id { get; set; }

        public string Producto { get; set; }

        public int Cantidad { get; set; }

        public int UsuarioId { get; set; }

        public int OrganizacionId { get; set; }

        public string Estado { get; set; }

        public string? Cuit { get; set; }

        public virtual OrganizationDto Organizacion { get; set; }

        public virtual UserDto Usuario { get; set; }
    }
}
