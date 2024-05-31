using backend.data.Models;
using backend.servicios.DTOs;

namespace backend.api.Models
{
    public class DonationRequestModel
    {

        public int Id { get; set; }
        public string Producto { get; set; }

        public int Cantidad { get; set; }

        public int UsuarioId { get; set; }

        public int OrganizacionId { get; set; }

    }
}
