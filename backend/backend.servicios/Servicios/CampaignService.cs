using AutoMapper;
using backend.data.Models;
using backend.repositories.interfaces;
using backend.servicios.DTOs;
using backend.servicios.Interfaces;
using Microsoft.Extensions.Logging;

namespace backend.servicios.Servicios
{
    public class CampaignService(IRepository<Campaign> repository, ILogger<NeedService> logger, IMapper mapper) : ICampaignService
    {
        private readonly IRepository<Campaign> _campaignRepository = repository ?? throw new ArgumentNullException(nameof(repository));
        private readonly ILogger<NeedService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        public async Task CreateCampaign(CampaignDto campaign)
        {
            if (campaign == null) throw new ArgumentNullException(nameof(campaign));

            var entity = _mapper.Map<Campaign>(campaign);
            await _campaignRepository.AddAsync(entity);
        }

        public async Task DeleteCampaign(int campaignId)
        {
            await _campaignRepository.DeleteAsync(campaignId);
        }

        public async Task<IEnumerable<CampaignDto>> GetCampaigns(int organizationId)
        {
            var campaigns = await _campaignRepository.GetAllAsync();

            return _mapper.Map<IEnumerable<CampaignDto>>(campaigns);
        }

        public async Task UpdateCampaign(CampaignDto campaign)
        {
            await _campaignRepository.UpdateAsync(_mapper.Map<Campaign>(campaign));
        }
    }
}
