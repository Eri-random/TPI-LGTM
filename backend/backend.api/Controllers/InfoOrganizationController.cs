using AutoMapper;
using backend.api.Models.RequestModels;
using backend.servicios.DTOs;
using backend.servicios.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace backend.api.Controllers
{
    [Route("api/Information")]
    [ApiController]
    public class InfoOrganizationController(IOrganizationService organizationService, IOrganizationInfoService organizationInfoService, ILogger<UserController> logger, IMapper mapper) : ControllerBase
    {
        private readonly IOrganizationService _organizationService = organizationService ?? throw new ArgumentNullException(nameof(organizationService));
        private readonly ILogger<UserController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IOrganizationInfoService _organizationInfoService = organizationInfoService ?? throw new ArgumentNullException(nameof(organizationInfoService));
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        /// <summary>
        /// Create new organization details.
        /// </summary>
        /// <param name="infoOrganizationRequest">The organization details request model.</param>
        /// <response code="201">Returns the created organization details.</response>
        /// <response code="400">If the organization data is invalid.</response>
        /// <response code="404">If the organization is not found.</response>
        /// <response code="500">If there is an internal server error.</response>
        [HttpPost("Details")]
        [ProducesResponseType(typeof(InfoOrganizationDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Details([FromBody] InfoOrganizationRequest infoOrganizationRequest)
        {
            if (infoOrganizationRequest == null)
                return BadRequest("Invalid organization data");

            var organization = await _organizationService.GetOrganizationByIdAsync(infoOrganizationRequest.OrganizacionId);
            if (organization == null)
                return NotFound("Organization not found");

            var infoOrganization = _mapper.Map<InfoOrganizationDto>(infoOrganizationRequest);

            try
            {
                await _organizationInfoService.SaveInfoOrganizationDataAsync(infoOrganization);
                return CreatedAtAction(nameof(Details), new { id = infoOrganizationRequest.OrganizacionId }, infoOrganization);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error creating organization information");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving organization information");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Update organization details.
        /// </summary>
        /// <param name="infoOrganizationRequest">The organization details request model.</param>
        /// <response code="201">Returns the updated organization details.</response>
        /// <response code="400">If the organization data is invalid.</response>
        /// <response code="404">If the organization is not found.</response>
        /// <response code="500">If there is an internal server error.</response>
        [HttpPut]
        [ProducesResponseType(typeof(InfoOrganizationDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Update([FromBody] InfoOrganizationRequest infoOrganizationRequest)
        {
            if (infoOrganizationRequest == null)
                return BadRequest("Invalid organization data");

            var organization = await _organizationService.GetOrganizationByIdAsync(infoOrganizationRequest.OrganizacionId);
            if (organization == null)
                return NotFound("Organization not found");

            var infoOrganization = _mapper.Map<InfoOrganizationDto>(infoOrganizationRequest);

            try
            {
                await _organizationInfoService.UpdateInfoOrganizationAsync(infoOrganization);
                return CreatedAtAction(nameof(Update), new { id = infoOrganizationRequest.OrganizacionId }, infoOrganization);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error updating organization information");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating organization information");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
