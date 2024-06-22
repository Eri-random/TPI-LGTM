using backend.servicios.DTOs;

namespace backend.servicios.Interfaces
{
    public interface ICampaignService
    {
        Task<IEnumerable<CampaignDto>> GetCampaigns(int organizationId);

        Task CreateCampaign(CampaignDto campaign);

        Task DeleteCampaign(int campaignId);

        Task UpdateCampaign(CampaignDto campaign);
    }
}
