namespace backend.api.Models
{
    public class OrganizationRequestModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Cuit { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public string Localidad { get; set; }
        public string Provincia { get; set; }
        public double Latitud { get; set; }
        public double Longitud { get; set; }
    }
}
