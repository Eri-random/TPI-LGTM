using backend.servicios.DTOs;

namespace backend.api.Models
{
    public class OrganizacionResponseModel
    {
        public string Nombre { get; set; }
        public string Cuit { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public string Localidad { get; set; }
        public string Provincia { get; set; }
        public InfoOrganizacionDto InfoOrganizacion { get; set; }
    }
}
