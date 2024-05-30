using backend.api.Models;
using backend.servicios.DTOs;
using backend.servicios.Interfaces;
using backend.servicios.Servicios;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NecesidadController(INecesidadService necesidadService, ILogger<NecesidadController> logger) : ControllerBase
    {
        private readonly INecesidadService _necesidadService = necesidadService ?? throw new ArgumentNullException(nameof(necesidadService));
        private readonly ILogger<NecesidadController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        [HttpGet]
        public async Task<IActionResult> GetAllNecesidades()
        {
            try
            {
                var necesidades = await _necesidadService.GetAllNecesidadAsync();

                var necesidadesResponse = new List<NecesidadResponseModel>();

                foreach (var necesidad in necesidades)
                {
                    necesidadesResponse.Add(new NecesidadResponseModel
                    {
                        Id = necesidad.Id,
                        Nombre = necesidad.Nombre,
                        Icono = necesidad.Icono,
                        Subcategoria = necesidad.Subcategoria.Select(p => new SubcategoriaDto
                        {
                            Id = p.Id,
                            Nombre = p.Nombre,
                            NecesidadId = p.NecesidadId
                        }).ToList()
                    });
                }

                return Ok(necesidadesResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las necesidades");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
