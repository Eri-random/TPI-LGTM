using backend.api.Models;
using backend.data.Models;
using backend.servicios.DTOs;
using backend.servicios.Interfaces;
using backend.servicios.Servicios;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace backend.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizationController(IOrganizationService organizationService, ILogger<UserController> logger) : ControllerBase
    {
        private readonly IOrganizationService _organizationService = organizationService ?? throw new ArgumentNullException(nameof(organizationService));
        private readonly ILogger<UserController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        [HttpGet]
        public async Task<IActionResult> GetAllOrganizations()
        {
            try
            {
                var organizations = await _organizationService.GetAllOrganizationAsync();

                var organizationResponse = new List<OrganizationResponseModel>();

                foreach (var org in organizations)
                {
                    organizationResponse.Add(new OrganizationResponseModel
                    {
                        Id = org.Id,
                        Nombre = org.Nombre,
                        Cuit = org.Cuit,
                        Direccion = org.Direccion,
                        Localidad = org.Localidad,
                        Provincia = org.Provincia,
                        Telefono = org.Telefono,
                        InfoOrganizacion = org.InfoOrganizacion != null ? new InfoOrganizationDto
                        {
                            Organizacion = org.InfoOrganizacion.Organizacion,
                            DescripcionBreve = org.InfoOrganizacion.DescripcionBreve,
                            DescripcionCompleta = org.InfoOrganizacion.DescripcionCompleta,
                            Img = org.InfoOrganizacion.Img,
                            OrganizacionId = org.InfoOrganizacion.OrganizacionId
                        } : null
                    });
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

                var organizationResponse = new OrganizationResponseModel
                {
                    Id = organization.Id,
                    Nombre = organization.Nombre,
                    Cuit = organization.Cuit,
                    Direccion = organization.Direccion,
                    Localidad = organization.Localidad,
                    Provincia = organization.Provincia,
                    Telefono = organization.Telefono,
                    InfoOrganizacion = organization.InfoOrganizacion != null ? new InfoOrganizationDto
                    {
                        Organizacion = organization.InfoOrganizacion.Organizacion,
                        DescripcionBreve = organization.InfoOrganizacion.DescripcionBreve,
                        DescripcionCompleta = organization.InfoOrganizacion.DescripcionCompleta,
                        Img = organization.InfoOrganizacion.Img,
                        OrganizacionId = organization.InfoOrganizacion.OrganizacionId
                    } : null
                };

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

                var organizationResponse = new OrganizationResponseModel
                {
                    Id = organization.Id,
                    Nombre = organization.Nombre,
                    Cuit = organization.Cuit,
                    Direccion = organization.Direccion,
                    Localidad = organization.Localidad,
                    Provincia = organization.Provincia,
                    Telefono = organization.Telefono,
                    Latitud = organization.Latitud,
                    Longitud = organization.Longitud,
                    InfoOrganizacion = organization.InfoOrganizacion != null ? new InfoOrganizationDto
                    {
                        Organizacion = organization.InfoOrganizacion.Organizacion,
                        DescripcionBreve = organization.InfoOrganizacion.DescripcionBreve,
                        DescripcionCompleta = organization.InfoOrganizacion.DescripcionCompleta,
                        Img = organization.InfoOrganizacion.Img,
                        OrganizacionId = organization.InfoOrganizacion.OrganizacionId
                    } : null
                };

                return Ok(organizationResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el organizacion por id {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("pagination")]
        public async Task<IActionResult> GetPaginatedOrganizationsAsync([FromQuery] int page = 1, [FromQuery] int pageSize = 8)
        {
            try
            {
                var organizations = await _organizationService.GetPaginatedOrganizationsAsync(page, pageSize);
                var organizationResponse = MapOrganizations(organizations);
                return Ok(organizationResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las organizaciones paginadas");
                return StatusCode(500, "Internal server error");
            }
        }

        private List<OrganizationResponseModel> MapOrganizations(IEnumerable<Organizacion> organizations)
        {
            var organizationResponse = new List<OrganizationResponseModel>();

            foreach (var org in organizations)
            {
                organizationResponse.Add(new OrganizationResponseModel
                {
                    Id = org.Id,
                    Nombre = org.Nombre,
                    Cuit = org.Cuit,
                    Direccion = org.Direccion,
                    Localidad = org.Localidad,
                    Provincia = org.Provincia,
                    Telefono = org.Telefono,
                    Latitud = org.Latitud,
                    Longitud = org.Longitud,
                    InfoOrganizacion = org.InfoOrganizacion != null ? new InfoOrganizationDto
                    {
                        Organizacion = org.InfoOrganizacion.Organizacion,
                        DescripcionBreve = org.InfoOrganizacion.DescripcionBreve,
                        DescripcionCompleta = org.InfoOrganizacion.DescripcionCompleta,
                        Img = org.InfoOrganizacion.Img,
                        OrganizacionId = org.InfoOrganizacion.OrganizacionId
                    } : null
                });
            }

          return organizationResponse;
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
                    return NotFound();
                }
                return Ok(groupedSubcategories);
            }
            catch (Exception ex)
            {
                // Manejar la excepción (por ejemplo, registrar el error)
                return StatusCode(500, "Error interno del servidor");
            }
        }

    }
}
