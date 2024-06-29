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
    public class CampaignController(ICampaignService campaignService, ILogger<CampaignController> logger, IMapper mapper, INeedService needService) : ControllerBase
    {
        private readonly ICampaignService _campaignService = campaignService ?? throw new ArgumentNullException(nameof(campaignService));
        private readonly ILogger<CampaignController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        private readonly INeedService _needService = needService ?? throw new ArgumentNullException(nameof(needService));

        /// <summary>
        /// Gets the campaigns for a specific organization.
        /// </summary>
        /// <param name="organizationId">The ID of the organization.</param>
        /// <returns>A list of campaigns for the specified organization.</returns>
        /// <response code="200">Returns the list of campaigns</response>
        /// <response code="404">If no campaigns are found for the organization</response>
        /// <response code="500">If there is an internal server error</response>
        [HttpGet("organization/{organizationId}")]
        [ProducesResponseType(typeof(List<CampaignResponseModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCampaignsByOrganizationId(int organizationId)
        {
            try
            {
                var campaigns = await _campaignService.GetCampaigns(organizationId);
                if (campaigns == null || !campaigns.Any())
                {
                    return NotFound($"No campaigns found for organization ID {organizationId}");
                }

                var needs = await _needService.GetAllNeedsAsync();
                var responseCampaigns = GetCampaignResponses(campaigns, needs);

                return Ok(responseCampaigns);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching campaigns for organization {OrganizationId}", organizationId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Creates a new campaign.
        /// </summary>
        /// <param name="campaign">The campaign request model.</param>
        /// <returns>The created campaign.</returns>
        /// <response code="201">Returns the newly created campaign</response>
        /// <response code="400">If the request is invalid</response>
        /// <response code="500">If there is an internal server error</response>
        [HttpPost]
        [ProducesResponseType(typeof(CampaignRequestModel), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateCampaign([FromBody] CampaignRequestModel campaign)
        {
            if (campaign == null)
            {
                return BadRequest("Campaign request is null");
            }

            if (campaign.StartDate >= campaign.EndDate)
            {
                return BadRequest("The start date must be earlier than the end date.");
            }

            try
            {
                var campaignDto = PrepareCampaignDto(campaign);
                await _campaignService.CreateCampaign(campaignDto);

                return CreatedAtAction(nameof(GetCampaignsByOrganizationId),
                    new { organizationId = campaign.OrganizacionId }, campaign);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating the campaign");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Deletes a campaign by its ID.
        /// </summary>
        /// <param name="campaignId">The ID of the campaign to delete.</param>
        /// <returns>No content.</returns>
        /// <response code="204">If the campaign was successfully deleted</response>
        /// <response code="404">If the campaign is not found</response>
        /// <response code="500">If there is an internal server error</response>
        [HttpDelete("{campaignId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteCampaign(int campaignId)
        {
            try
            {
                var campaignExists = await _campaignService.GetCampaignById(campaignId);

                if (campaignExists is null)
                    return NotFound($"Campaign with ID {campaignId} not found");

                await _campaignService.DeleteCampaign(campaignId);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting campaign {campaignId}", campaignId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Updates the status of a campaign.
        /// </summary>
        /// <param name="campaignRequest">The campaign request model.</param>
        /// <returns>No content.</returns>
        /// <response code="204">If the campaign status was successfully updated</response>
        /// <response code="400">If the request is invalid</response>
        /// <response code="404">If the campaign is not found</response>
        /// <response code="500">If there is an internal server error</response>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCampaignStatus([FromBody] CampaignRequestModel campaignRequest)
        {
            if (campaignRequest == null)
            {
                return BadRequest("Campaign request object is null");
            }

            try
            {
                var campaignExists = await _campaignService.GetCampaignById(campaignRequest.Id);

                if (campaignExists is null)
                {
                    return NotFound($"Campaign with ID {campaignRequest.Id} not found");
                }

                var campaignDto = _mapper.Map<CampaignDto>(campaignRequest);
                await _campaignService.UpdateCampaign(campaignDto);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating campaign {campaignId}", campaignRequest.Id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Gets a campaign by its ID.
        /// </summary>
        /// <param name="campaignId">The ID of the campaign.</param>
        /// <returns>The campaign with the specified ID.</returns>
        /// <response code="200">Returns the campaign with the specified ID</response>
        /// <response code="404">If the campaign is not found</response>
        /// <response code="500">If there is an internal server error</response>
        [HttpGet("{campaignId}")]
        [ProducesResponseType(typeof(CampaignResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCampaignById(int campaignId)
        {
            try
            {
                var campaign = await _campaignService.GetCampaignById(campaignId);

                if (campaign == null)
                {
                    return NotFound("Campaign not found");
                }

                var needs = await _needService.GetAllNeedsAsync();
                var response = CreateCampaignResponse(campaign, needs);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving campaign with ID {CampaignId}", campaignId);
                return StatusCode(500, "Internal server error");
            }
        }

        private List<CampaignResponseModel> GetCampaignResponses(IEnumerable<CampaignDto> campaigns, IEnumerable<NeedDto> needs)
        {
            var responseCampaigns = new List<CampaignResponseModel>();
            var needsList = needs.ToList();

            foreach (var campaign in campaigns)
            {
                var filteredNeeds = GetFilteredNeedsForCampaign(campaign, needsList);
                var response = _mapper.Map<CampaignResponseModel>(campaign);
                response.Needs = filteredNeeds;
                responseCampaigns.Add(response);
            }

            return responseCampaigns;
        }

        private static List<NeedDto> GetFilteredNeedsForCampaign(CampaignDto campaign, List<NeedDto> needs)
        {
            var idsSet = new HashSet<string>(campaign.Subcategorias.Split(','));

            return needs
                .Select(need => new NeedDto
                {
                    Id = need.Id,
                    Nombre = need.Nombre,
                    Icono = need.Icono,
                    Subcategoria = need.Subcategoria
                        .Where(sub => idsSet.Contains(sub.Id.ToString()))
                        .ToList()
                })
                .Where(need => need.Subcategoria.Any())
                .ToList();
        }

        private CampaignDto PrepareCampaignDto(CampaignRequestModel campaign)
        {
            var dto = _mapper.Map<CampaignDto>(campaign);
            dto.IsActive = true;
            dto.Subcategorias = string.Join(",", campaign.Subcategoria.Select(sub => sub.Id));

            return dto;
        }

        private CampaignResponseModel CreateCampaignResponse(CampaignDto campaign, IEnumerable<NeedDto> needs)
        {
            var idsSet = new HashSet<string>(campaign.Subcategorias.Split(','));

            var filteredNeeds = needs.Select(need => new NeedDto
            {
                Id = need.Id,
                Nombre = need.Nombre,
                Icono = need.Icono,
                Subcategoria = need.Subcategoria
                    .Where(sub => idsSet.Contains(sub.Id.ToString()))
                    .ToList()
            })
            .Where(need => need.Subcategoria.Any())
            .ToList();

            var response = _mapper.Map<CampaignResponseModel>(campaign);
            response.Needs = filteredNeeds;

            return response;
        }
    }
}
