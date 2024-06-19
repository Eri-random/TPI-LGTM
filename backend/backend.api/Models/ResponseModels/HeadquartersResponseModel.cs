using backend.data.Models;

namespace backend.api.Models.ResponseModels
{
    public class HeadquartersResponseModel
    {
        public int Id { get; set; }
        public string Direccion { get; set; }

        public string Localidad { get; set; }

        public string Provincia { get; set; }

        public int OrganizacionId { get; set; }

        public string Nombre { get; set; }

        public string Telefono { get; set; }

        public double Latitud { get; set; }

        public double Longitud { get; set; }

        public virtual Organizacion Organizacion { get; set; }
    }
}
