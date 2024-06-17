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
    public class OrganizationController(IOrganizationService organizationService, ILogger<UserController> logger, IMapper mapper) : ControllerBase
    {
        private readonly IOrganizationService _organizationService = organizationService ?? throw new ArgumentNullException(nameof(organizationService));
        private readonly ILogger<UserController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        [HttpGet]
        public async Task<IActionResult> GetAllOrganizations()
        {
            try
            {
                var organizations = await _organizationService.GetAllOrganizationAsync();
                var organizationResponse = new List<OrganizationResponseModel>();

                foreach (var org in organizations)
                {
                    organizationResponse.Add(_mapper.Map<OrganizationResponseModel>(org));
                }

                return Ok(organizationResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las organizaciones");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{cuit}")]
        public async Task<IActionResult> GetOrganizacionByCuit(string cuit)
        {
            try
            {
                var organization = await _organizationService.GetOrganizationByCuitAsync(cuit);
                if (organization == null)
                {
                    return NotFound("Organizacion no encontrada");
                }

                var organizationResponse = _mapper.Map<OrganizationResponseModel>(organization);

                return Ok(organizationResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el organizacion con cuit {Cuit}", cuit);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("Id/{id}")]
        public async Task<IActionResult> GetOrganizationById(int id)
        {
            try
            {
                var organization = await _organizationService.GetOrganizationByIdAsync(id);
                if (organization == null)
                {
                    return NotFound("Organizacion no encontrada");
                }

                var organizationResponse = _mapper.Map<OrganizationResponseModel>(organization);

                return Ok(organizationResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el organizacion por id {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("pagination")]
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
                var organizationResponse = MapOrganizations(organizations);

                return Ok(organizationResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las organizaciones paginadas");
                return StatusCode(500, "Internal server error");
            }
        }
      
        [HttpPost("{organizationId}/assign-need")]
        public async Task<IActionResult> AssignSubcategoriesAsync(int organizationId, [FromBody] List<SubcategoriesDto> subcategoriesDto)
        {
            try
            {
                await _organizationService.AssignSubcategoriesAsync(organizationId, subcategoriesDto);
                return Ok(new { message = "Necesidades guardadas correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Error al asignar necesidades: {ex.Message}" });
            }
        }

        [HttpGet("{organizationId}/subcategories")]
        public async Task<IActionResult> GetAssignedSubcategories(int organizationId)
        {
            try
            {
                var subcategories = await _organizationService.GetAssignedSubcategoriesAsync(organizationId);
                return Ok(subcategories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener subcategorias: {ex.Message}");
            }
        }

        [HttpGet("{organizationId}/grouped-subcategories")]
        public async Task<ActionResult<List<NeedDto>>> GetGroupedSubcategories(int organizationId)
        {
            try
            {
                var groupedSubcategories = await _organizationService.GetAssignedSubcategoriesGroupedAsync(organizationId);
                if (groupedSubcategories == null || !groupedSubcategories.Any())
                {
                    return Ok(new List<NeedDto>());
                }
                return Ok(groupedSubcategories);
            }
            catch (Exception ex)
            {
                // Manejar la excepción (por ejemplo, registrar el error)
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateOrganization([FromBody] OrganizationRequestModel organizationRequest)
        {
            if (organizationRequest == null)
            {
                return BadRequest("Organizacion no puede ser nula");
            }

            try
            {
                var organization = _mapper.Map<OrganizationDto>(organizationRequest);
                await _organizationService.UpdateOrganizationAsync(organization);

                return Ok(new { message = $"Organización {organizationRequest.Nombre} actualizada correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Error al actualizar la organizacion: {ex.Message}" });
            }
        }

        private List<OrganizationResponseModel> MapOrganizations(IEnumerable<OrganizationDto> organizations)
        {
            var organizationResponse = new List<OrganizationResponseModel>();

            foreach (var org in organizations)
            {
                organizationResponse.Add(_mapper.Map<OrganizationResponseModel>(org));
            }

            return organizationResponse;
        }
    }
}
