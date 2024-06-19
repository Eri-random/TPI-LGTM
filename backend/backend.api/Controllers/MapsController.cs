using AutoMapper;
using backend.api.Models.ResponseModels;
using backend.servicios.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace backend.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MapsController(
        IOrganizationService organizationService,
        IHeadquartersService headquartersService,
        IMapper mapper,
        ILogger<MapsController> logger) : ControllerBase
    {
        private readonly IOrganizationService _organizationService = organizationService ?? throw new ArgumentNullException(nameof(organizationService));
        private readonly IHeadquartersService _headquartersService = headquartersService ?? throw new ArgumentNullException(nameof(headquartersService));
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        private readonly ILogger<MapsController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        /// <summary>
        /// Get coordinates of all organizations.
        /// </summary>
        /// <response code="200">Returns the list of organization coordinates.</response>
        /// <response code="500">If there is an internal server error.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<OrganizationResponseModel>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetOrganizationCoordinates()
        {
            try
            {
                var organizations = await _organizationService.GetAllOrganizationAsync();
                var organizationsResponse = _mapper.Map<IEnumerable<OrganizationResponseModel>>(organizations);

                return Ok(organizationsResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving organization coordinates");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get headquarters coordinates of an organization by ID.
        /// </summary>
        /// <param name="organizationId">The ID of the organization.</param>
        /// <response code="200">Returns the list of headquarters coordinates.</response>
        /// <response code="404">If the organization or headquarters are not found.</response>
        /// <response code="500">If there is an internal server error.</response>
        [HttpGet("{organizationId}")]
        [ProducesResponseType(typeof(IEnumerable<HeadquartersResponseModel>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetOrganizationHeadquartersCoordinates(int organizationId)
        {
            try
            {
                var headquarters = await _headquartersService.GetHeadquartersByOrganizationIdAsync(organizationId);
                if (headquarters == null || !headquarters.Any())
                    return NotFound("Headquarters not found");

                var headquartersResponse = _mapper.Map<IEnumerable<HeadquartersResponseModel>>(headquarters);

                return Ok(headquartersResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving headquarters coordinates for organization ID {OrganizationId}", organizationId);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
