using backend.servicios.DTOs;

namespace backend.api.Models.ResponseModels
{
    public class CampaignResponseModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int OrganizacionId { get; set; }

        public IEnumerable<SubcategoriesDto> Subs { get; set; } = new List<SubcategoriesDto>();
    }
}
