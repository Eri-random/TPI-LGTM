namespace backend.api.Models
{
    public class SedeRequestModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public string Localidad { get; set; }
        public string Provincia { get; set; }
        public string Telefono { get; set; }
        public int OrganizacionId { get; set; }
    }
}
