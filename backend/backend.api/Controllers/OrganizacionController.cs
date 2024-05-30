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
    public class OrganizacionController(IOrganizacionService organizacionService, ILogger<UsuariosController> logger) : ControllerBase
    {
        private readonly IOrganizacionService _organizacionService = organizacionService ?? throw new ArgumentNullException(nameof(organizacionService));
        private readonly ILogger<UsuariosController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        [HttpGet]
        public async Task<IActionResult> GetAllOrganizaciones()
        {
            try
            {
                var organizaciones = await _organizacionService.GetAllOrganizacionAsync();

                var organizacionResponse = new List<OrganizacionResponseModel>();

                foreach (var organizacion in organizaciones)
                {
                    organizacionResponse.Add(new OrganizacionResponseModel
                    {
                        Id = organizacion.Id,
                        Nombre = organizacion.Nombre,
                        Cuit = organizacion.Cuit,
                        Direccion = organizacion.Direccion,
                        Localidad = organizacion.Localidad,
                        Provincia = organizacion.Provincia,
                        Telefono = organizacion.Telefono,
                        InfoOrganizacion = organizacion.InfoOrganizacion != null ? new InfoOrganizacionDto
                        {
                            Organizacion = organizacion.InfoOrganizacion.Organizacion,
                            DescripcionBreve = organizacion.InfoOrganizacion.DescripcionBreve,
                            DescripcionCompleta = organizacion.InfoOrganizacion.DescripcionCompleta,
                            Img = organizacion.InfoOrganizacion.Img,
                            OrganizacionId = organizacion.InfoOrganizacion.OrganizacionId
                        } : null
                    });
                }

                return Ok(organizacionResponse);
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
                var organizacion = await _organizacionService.GetOrganizacionByCuitAsync(cuit);
                if (organizacion == null)
                {
                    return NotFound("Organizacion no encontrada");
                }

                var organizacionResponse = new OrganizacionResponseModel
                {
                    Id = organizacion.Id,
                    Nombre = organizacion.Nombre,
                    Cuit = organizacion.Cuit,
                    Direccion = organizacion.Direccion,
                    Localidad = organizacion.Localidad,
                    Provincia = organizacion.Provincia,
                    Telefono = organizacion.Telefono,
                    InfoOrganizacion = organizacion.InfoOrganizacion != null ? new InfoOrganizacionDto
                    {
                        Organizacion = organizacion.InfoOrganizacion.Organizacion,
                        DescripcionBreve = organizacion.InfoOrganizacion.DescripcionBreve,
                        DescripcionCompleta = organizacion.InfoOrganizacion.DescripcionCompleta,
                        Img = organizacion.InfoOrganizacion.Img,
                        OrganizacionId = organizacion.InfoOrganizacion.OrganizacionId
                    } : null
                };

                return Ok(organizacionResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el organizacion con cuit {Cuit}", cuit);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("Id/{id}")]
        public async Task<IActionResult> GetOrganizacionById(int id)
        {
            try
            {
                var organizacion = await _organizacionService.GetOrganizacionByIdAsync(id);
                if (organizacion == null)
                {
                    return NotFound("Organizacion no encontrada");
                }

                var organizacionResponse = new OrganizacionResponseModel
                {
                    Id = organizacion.Id,
                    Nombre = organizacion.Nombre,
                    Cuit = organizacion.Cuit,
                    Direccion = organizacion.Direccion,
                    Localidad = organizacion.Localidad,
                    Provincia = organizacion.Provincia,
                    Telefono = organizacion.Telefono,
                    Latitud = organizacion.Latitud,
                    Longitud = organizacion.Longitud,
                    InfoOrganizacion = organizacion.InfoOrganizacion != null ? new InfoOrganizacionDto
                    {
                        Organizacion = organizacion.InfoOrganizacion.Organizacion,
                        DescripcionBreve = organizacion.InfoOrganizacion.DescripcionBreve,
                        DescripcionCompleta = organizacion.InfoOrganizacion.DescripcionCompleta,
                        Img = organizacion.InfoOrganizacion.Img,
                        OrganizacionId = organizacion.InfoOrganizacion.OrganizacionId
                    } : null
                };

                return Ok(organizacionResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el organizacion por id {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("paginacion")]
        public async Task<IActionResult> GetOrganizacionesPaginadasAsync([FromQuery] int page = 1, [FromQuery] int pageSize = 8)
        {
            try
            {
                var organizaciones = await _organizacionService.GetOrganizacionesPaginadasAsync(page, pageSize);
                var organizacionResponse = MapearOrganizaciones(organizaciones);
                return Ok(organizacionResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las organizaciones paginadas");
                return StatusCode(500, "Internal server error");
            }
        }

        private List<OrganizacionResponseModel> MapearOrganizaciones(IEnumerable<Organizacion> organizaciones)
        {
            var organizacionResponse = new List<OrganizacionResponseModel>();

            foreach (var organizacion in organizaciones)
            {
                organizacionResponse.Add(new OrganizacionResponseModel
                {
                    Id = organizacion.Id,
                    Nombre = organizacion.Nombre,
                    Cuit = organizacion.Cuit,
                    Direccion = organizacion.Direccion,
                    Localidad = organizacion.Localidad,
                    Provincia = organizacion.Provincia,
                    Telefono = organizacion.Telefono,
                    Latitud = organizacion.Latitud,
                    Longitud = organizacion.Longitud,
                    InfoOrganizacion = organizacion.InfoOrganizacion != null ? new InfoOrganizacionDto
                    {
                        Organizacion = organizacion.InfoOrganizacion.Organizacion,
                        DescripcionBreve = organizacion.InfoOrganizacion.DescripcionBreve,
                        DescripcionCompleta = organizacion.InfoOrganizacion.DescripcionCompleta,
                        Img = organizacion.InfoOrganizacion.Img,
                        OrganizacionId = organizacion.InfoOrganizacion.OrganizacionId
                    } : null
                });
            }

          return organizacionResponse;
      }
      
        [HttpPost("{organizacionId}/asignar-necesidad")]
        public async Task<IActionResult> AsignarSubcategoriasAsync(int organizacionId, [FromBody] List<SubcategoriaDto> subcategoriasDto)
        {
            try
            {
                await _organizacionService.AsignarSubcategoriasAsync(organizacionId, subcategoriasDto);
                return Ok(new { message = "Necesidades guardadas correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Error al asignar necesidades: {ex.Message}" });
            }
        }

        [HttpGet("{organizacionId}/subcategorias")]
        public async Task<IActionResult> GetSubcategoriasAsignadas(int organizacionId)
        {
            try
            {
                var subcategorias = await _organizacionService.GetSubcategoriasAsignadasAsync(organizacionId);
                return Ok(subcategorias);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener subcategorias: {ex.Message}");
            }
        }

    }
}
