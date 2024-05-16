namespace backend.api.Models
{
    public class CoordinatesResponseModel
    {
        public double Lat { get; set; }
        public double Lng { get; set; }
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public string Localidad { get; set; }
        public string Provincia { get; set; }
        public string Telefono { get; set; }
    }
}
