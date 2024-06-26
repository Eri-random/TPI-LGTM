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

        public IEnumerable<NeedDto> Needs { get; set; } = new List<NeedDto>();

        public bool IsActive { get; set; }

        public string ImageUrl { get; set; }
    }
}
