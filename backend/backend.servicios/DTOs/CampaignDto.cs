namespace backend.servicios.DTOs
{
    public class CampaignDto
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int OrganizacionId { get; set; }

        public string Subcategorias { get; set; }

        public bool IsActive { get; set; }

        public string ImageUrl { get; set; }

        public string DescripcionBreve { get; set; }

        public string DescripcionCompleta { get; set; }
    }
}
