using AutoMapper;
using backend.api.Models.RequestModels;
using backend.api.Models.ResponseModels;
using backend.servicios.DTOs;
using backend.servicios.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace backend.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampaignController(ICampaignService campaignService, ILogger<CampaignController> logger, IMapper mapper) : ControllerBase
    {
        private readonly ICampaignService _campaignService = campaignService ?? throw new ArgumentNullException(nameof(campaignService));
        private readonly ILogger<CampaignController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        [HttpGet("{organizationId}")]
        public async Task<IActionResult> GetCampaignsByOrganizationId(int organizationId)
        {
            try
            {
                var campaigns = await _campaignService.GetCampaigns(organizationId);
                return Ok(_mapper.Map<IEnumerable<CampaignResponseModel>>(campaigns));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching campaigns for organization {OrganizationId}", organizationId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateCampaign([FromBody] CampaignRequestModel campaign)
        {
            if (campaign == null)
            {
                return BadRequest("Campaign object is null");
            }

            try
            {
                var dto = _mapper.Map<CampaignDto>(campaign);

                var ids = string.Empty;

                foreach (var subcategory in campaign.Subcategoria)
                {
                    ids += subcategory.Id + ",";
                }

                dto.Subcategorias = ids;

                await _campaignService.CreateCampaign(dto);
                return CreatedAtAction(nameof(GetCampaignsByOrganizationId), new { organizationId = campaign.OrganizacionId }, campaign);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating the campaign");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdaeteCampaign([FromBody] CampaignRequestModel campaign)
        {
            if (campaign == null)
            {
                return BadRequest("Campaign object is null");
            }

            try
            {
                await _campaignService.UpdateCampaign(_mapper.Map<CampaignDto>(campaign));
                return CreatedAtAction(nameof(GetCampaignsByOrganizationId), new { organizationId = campaign.OrganizacionId }, campaign);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating the campaign");
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpDelete("{campaignId}")]
        public async Task<IActionResult> DeleteCampaign(int campaignId)
        {
            try
            {
                await _campaignService.DeleteCampaign(campaignId);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting campaign {campaignId}", campaignId);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
