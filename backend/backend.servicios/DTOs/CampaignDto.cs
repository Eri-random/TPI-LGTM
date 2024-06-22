namespace backend.servicios.DTOs
{
    public class CampaignDto
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int OrganizacionId { get; set; }

        public ICollection<NeedDto> Needs { get; set; } = new List<NeedDto>();
    }

}
