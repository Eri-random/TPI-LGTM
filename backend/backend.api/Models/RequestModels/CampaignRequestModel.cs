using backend.data.Models;
using backend.servicios.DTOs;

namespace backend.api.Models.RequestModels
{
    public class CampaignRequestModel
    {
        public int Id { get; set; }

        public string Title { get; set; }
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int OrganizacionId { get; set; }

        public ICollection<Subcategorium> Subcategoria { get; set; } = new List<Subcategorium>();
    }
}
