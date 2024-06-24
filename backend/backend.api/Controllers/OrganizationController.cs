using AutoMapper;
using backend.api.Models.RequestModels;
using backend.api.Models.ResponseModels;
using backend.servicios.DTOs;
using backend.servicios.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizationController(IOrganizationService organizationService, ILogger<UserController> logger, IMapper mapper) : ControllerBase
    {
        private readonly IOrganizationService _organizationService = organizationService ?? throw new ArgumentNullException(nameof(organizationService));
        private readonly ILogger<UserController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        /// <summary>
        /// Get all organizations.
        /// </summary>
        /// <response code="200">Returns the list of organizations.</response>
        /// <response code="500">If there is an internal server error.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<OrganizationResponseModel>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllOrganizations()
        {
            try
            {
                var organizations = await _organizationService.GetAllOrganizationAsync();
                var organizationResponse = _mapper.Map<IEnumerable<OrganizationResponseModel>>(organizations);

                return Ok(organizationResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all organizations");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get an organization by CUIT.
        /// </summary>
        /// <param name="cuit">The CUIT of the organization.</param>
        /// <response code="200">Returns the organization.</response>
        /// <response code="404">If the organization is not found.</response>
        /// <response code="500">If there is an internal server error.</response>
        [HttpGet("{cuit}")]
        [ProducesResponseType(typeof(OrganizationResponseModel), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetOrganizationByCuit(string cuit)
        {
            try
            {
                var organization = await _organizationService.GetOrganizationByCuitAsync(cuit);
                if (organization == null)
                    return NotFound("Organization not found");

                return Ok(_mapper.Map<OrganizationResponseModel>(organization));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving organization with CUIT {Cuit}", cuit);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get an organization by ID.
        /// </summary>
        /// <param name="id">The ID of the organization.</param>
        /// <response code="200">Returns the organization.</response>
        /// <response code="404">If the organization is not found.</response>
        /// <response code="500">If there is an internal server error.</response>
        [HttpGet("Id/{id}")]
        [ProducesResponseType(typeof(OrganizationResponseModel), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetOrganizationById(int id)
        {
            try
            {
                var organization = await _organizationService.GetOrganizationByIdAsync(id);
                if (organization == null)
                    return NotFound("Organization not found");

                return Ok(_mapper.Map<OrganizationResponseModel>(organization));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving organization by ID {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get paginated list of organizations.
        /// </summary>
        /// <param name="page">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="subcategoriaIds">Comma-separated list of subcategory IDs.</param>
        /// <param name="name">Name of the organization to filter by.</param>
        /// <response code="200">Returns the paginated list of organizations.</response>
        /// <response code="500">If there is an internal server error.</response>
        [HttpGet("pagination")]
        [ProducesResponseType(typeof(IEnumerable<OrganizationResponseModel>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetPaginatedOrganizationsAsync(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 8,
            [FromQuery] string subcategoriaIds = null,
            [FromQuery] string name = null)
        {
            try
            {
                List<int> subcategoriaIdList = null;
                if (!string.IsNullOrEmpty(subcategoriaIds))
                {
                    subcategoriaIdList = subcategoriaIds.Split(',')
                        .Select(int.Parse)
                        .ToList();
                }

                var organizations = await _organizationService.GetPaginatedOrganizationsAsync(page, pageSize, subcategoriaIdList, name);

                return Ok(_mapper.Map<IEnumerable<OrganizationResponseModel>>(organizations));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving paginated organizations");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Assign subcategories to an organization.
        /// </summary>
        /// <param name="organizationId">The ID of the organization.</param>
        /// <param name="subcategoriesDto">List of subcategories to assign.</param>
        /// <response code="200">Returns a success message.</response>
        /// <response code="500">If there is an internal server error.</response>
        /// 
        [Authorize(Roles = "organizacion")]
        [HttpPost("{organizationId}/assign-need")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> AssignSubcategoriesAsync(int organizationId, [FromBody] List<SubcategoriesDto> subcategoriesDto)
        {
            try
            {
                await _organizationService.AssignSubcategoriesAsync(organizationId, subcategoriesDto);

                return Ok(new { message = "Subcategories successfully assigned" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning subcategories to organization with ID {OrganizationId}", organizationId);
                return StatusCode(500, new { error = $"Error assigning subcategories: {ex.Message}" });
            }
        }

        /// <summary>
        /// Get assigned subcategories of an organization.
        /// </summary>
        /// <param name="organizationId">The ID of the organization.</param>
        /// <response code="200">Returns the list of assigned subcategories.</response>
        /// <response code="500">If there is an internal server error.</response>
        [HttpGet("{organizationId}/subcategories")]
        [ProducesResponseType(typeof(IEnumerable<SubcategoriesDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAssignedSubcategories(int organizationId)
        {
            try
            {
                var subcategories = await _organizationService.GetAssignedSubcategoriesAsync(organizationId);

                return Ok(subcategories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving assigned subcategories for organization with ID {OrganizationId}", organizationId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get grouped subcategories of an organization.
        /// </summary>
        /// <param name="organizationId">The ID of the organization.</param>
        /// <response code="200">Returns the list of grouped subcategories.</response>
        /// <response code="500">If there is an internal server error.</response>
        [HttpGet("{organizationId}/grouped-subcategories")]
        [ProducesResponseType(typeof(List<NeedDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<List<NeedDto>>> GetGroupedSubcategories(int organizationId)
        {
            try
            {
                var groupedSubcategories = await _organizationService.GetAssignedSubcategoriesGroupedAsync(organizationId);
                if (groupedSubcategories == null || !groupedSubcategories.Any())
                    return Ok(new List<NeedDto>());

                return Ok(groupedSubcategories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving grouped subcategories for organization with ID {OrganizationId}", organizationId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Update an organization.
        /// </summary>
        /// <param name="organizationRequest">The organization request model.</param>
        /// <response code="200">Returns a success message.</response>
        /// <response code="400">If the organization data is invalid.</response>
        /// <response code="500">If there is an internal server error.</response>
        /// 
        [Authorize(Roles = "organizacion")]
        [HttpPut]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateOrganization([FromBody] OrganizationRequestModel organizationRequest)
        {
            if (organizationRequest == null)
                return BadRequest("Organization cannot be null");

            try
            {
                var organization = _mapper.Map<OrganizationDto>(organizationRequest);
                await _organizationService.UpdateOrganizationAsync(organization);

                return Ok(new { message = $"Organization {organizationRequest.Nombre} successfully updated" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating organization with name {Nombre}", organizationRequest.Nombre);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
