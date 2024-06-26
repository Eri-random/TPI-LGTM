﻿using AutoMapper;
using backend.api.Models.RequestModels;
using backend.api.Models.ResponseModels;
using backend.servicios.DTOs;
using backend.servicios.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace backend.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampaignController(ICampaignService campaignService, ILogger<CampaignController> logger, IMapper mapper, INeedService needService) : ControllerBase
    {
        private readonly ICampaignService _campaignService = campaignService ?? throw new ArgumentNullException(nameof(campaignService));
        private readonly ILogger<CampaignController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        private readonly INeedService _needService = needService ?? throw new ArgumentNullException(nameof(needService));

        [HttpGet("{organizationId}")]
        public async Task<IActionResult> GetCampaignsByOrganizationId(int organizationId)
        {
            try
            {
                var campaigns = await _campaignService.GetCampaigns(organizationId);
                var responseCampaign = new List<CampaignResponseModel>();
                var needs = await _needService.GetAllNeedsAsync();

                foreach (var campaign in campaigns)
                {
                    var ids = campaign.Subcategorias.Split(',');
                    var response = _mapper.Map<CampaignResponseModel>(campaign);
                    response.Subs = needs.SelectMany(x => x.Subcategoria.Where(y => ids.Contains(y.Id.ToString())));

                    responseCampaign.Add(response);
                }

                return Ok(responseCampaign);
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
