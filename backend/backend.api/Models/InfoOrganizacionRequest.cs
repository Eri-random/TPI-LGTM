namespace backend.api.Models
{
    public class InfoOrganizacionRequest
    {

        public string Organizacion { get; set; }

        public string DescripcionBreve { get; set; }

        public string DescripcionCompleta { get; set; }

        public IFormFile File { get; set; }

        public int OrganizacionId { get; set; }
    }
}
