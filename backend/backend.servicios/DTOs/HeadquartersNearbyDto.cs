namespace backend.servicios.DTOs
{
    public class HeadquartersNearbyDto
    {
        public int Id { get; set; }
        public double Distancia { get; set; }
        public string Nombre { get; set; }

        public string Direccion { get; set; }
        public string Localidad { get; set; }
        public string Provincia { get; set; }
        public string Telefono { get; set; }

        public double Latitud { get; set; }
        public double Longitud { get; set; }

        public string? nombreOrganizacion { get; set; }
    }
}
