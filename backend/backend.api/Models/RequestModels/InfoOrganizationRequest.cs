namespace backend.api.Models.RequestModels
{
    public class InfoOrganizationRequest
    {
        public string Organizacion { get; set; }

        public string DescripcionBreve { get; set; }

        public string DescripcionCompleta { get; set; }

        public string ImageUrl { get; set; }

        public int OrganizacionId { get; set; }
    }
}
