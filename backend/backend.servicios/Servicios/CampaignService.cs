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

            try
            {
                var entity = _mapper.Map<Campaign>(campaign);
                await _campaignRepository.AddAsync(entity);
                _logger.LogInformation("Campaign created successfully: {CampaignId}", entity.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating campaign");
                throw;
            }
        }

        public async Task DeleteCampaign(int campaignId)
        {
            try
            {
                await _campaignRepository.DeleteAsync(campaignId);
                _logger.LogInformation("Campaign deleted successfully: {CampaignId}", campaignId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting campaign {CampaignId}", campaignId);
                throw;
            }
        }

        public async Task<CampaignDto> GetCampaignById(int campaignId)
        {
            try
            {
                var campaign = await _campaignRepository.GetByIdAsync(campaignId);
                if (campaign == null)
                {
                    _logger.LogWarning("Campaign not found: {CampaignId}", campaignId);
                    return null;
                }

                return _mapper.Map<CampaignDto>(campaign);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving campaign {CampaignId}", campaignId);
                throw;
            }
        }

        public async Task<IEnumerable<CampaignDto>> GetCampaigns(int organizationId)
        {
            try
            {
                var campaigns = await _campaignRepository.GetAllAsync();
                await ValidateCampaigns(campaigns);

                var orgCampaigns = campaigns.Where(x => x.OrganizacionId == organizationId);
                return _mapper.Map<IEnumerable<CampaignDto>>(orgCampaigns);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving campaigns for organization {OrganizationId}", organizationId);
                throw;
            }
        }

        public async Task UpdateCampaign(CampaignDto campaign)
        {
            if (campaign == null) throw new ArgumentNullException(nameof(campaign));

            try
            {
                var campaignToUpdate = await _campaignRepository.GetByIdAsync(campaign.Id);
                if (campaignToUpdate == null)
                {
                    _logger.LogWarning("Campaign not found: {CampaignId}", campaign.Id);
                    return;
                }

                UpdateCampaignEntity(campaign, campaignToUpdate);

                await _campaignRepository.UpdateAsync(campaignToUpdate);
                _logger.LogInformation("Campaign updated successfully: {CampaignId}", campaign.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating campaign {CampaignId}", campaign.Id);
                throw;
            }
        }

        private async Task ValidateCampaigns(IEnumerable<Campaign> campaigns)
        {
            var campaignsToUpdate = campaigns.Where(x => x.IsActive && (DateTime.Now > x.EndDate));

            if (campaignsToUpdate.Any())
            {
                foreach (var campaign in campaignsToUpdate)
                {
                    campaign.IsActive = false;
                }

                try
                {
                    await _campaignRepository.UpdateRangeAsync(campaignsToUpdate);
                    _logger.LogInformation("Validated and updated inactive campaigns");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while validating campaigns");
                    throw;
                }
            }
        }

        private static void UpdateCampaignEntity(CampaignDto source, Campaign target)
        {
            target.IsActive = source.IsActive;
            target.StartDate = source.StartDate;
            target.EndDate = source.EndDate;
        }
    }
}
