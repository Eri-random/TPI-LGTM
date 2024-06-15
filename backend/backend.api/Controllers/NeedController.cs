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
    public class NeedController(INeedService needService, ILogger<NeedController> logger) : ControllerBase
    {
        private readonly INeedService _neeedService = needService ?? throw new ArgumentNullException(nameof(needService));
        private readonly ILogger<NeedController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        [HttpGet]
        public async Task<IActionResult> GetAllNeeds()
        {
            try
            {
                var needs = await _neeedService.GetAllNeedsAsync();

                var needsResponse = new List<NeedsResponseModel>();

                foreach (var need in needs)
                {
                    needsResponse.Add(new NeedsResponseModel
                    {
                        Id = need.Id,
                        Nombre = need.Nombre,
                        Icono = need.Icono,
                        Subcategoria = need.Subcategoria.Select(p => new SubcategoriesDto
                        {
                            Id = p.Id,
                            Nombre = p.Nombre,
                            NecesidadId = p.NecesidadId
                        }).ToList()
                    });
                }

                return Ok(needsResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las necesidades");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
