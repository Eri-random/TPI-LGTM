using backend.api.Models;
using backend.servicios.DTOs;
using backend.servicios.Helpers;
using backend.servicios.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace backend.api.Controllers
{
    [Route("api/Informacion")]
    [ApiController]
    public class InfoOrganizacionController(IOrganizacionService organizacionService, IOrganizacionInfoService organizacionInfoService, ILogger<UsuariosController> logger) : ControllerBase
    {
        private readonly IOrganizacionService _organizacionService = organizacionService ?? throw new ArgumentNullException(nameof(organizacionService));
        private readonly ILogger<UsuariosController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IOrganizacionInfoService _organizacionInfoService = organizacionInfoService ?? throw new ArgumentNullException(nameof(organizacionInfoService));

        [HttpPost("Detalles")]
        public async Task<IActionResult> Details([FromBody] InfoOrganizacionRequest infoOrganizacionRequest)
        {
            if (infoOrganizacionRequest == null)
            {
                return BadRequest("Datos de organización inválidos");
            }

            var organizacion = await _organizacionService.GetOrganizacionByIdAsync(infoOrganizacionRequest.OrganizacionId);

            if (organizacion == null)
            {
                return NotFound("Organización no encontrada");
            }

            var infoOrganizacion = new InfoOrganizacionDto
            {
                Organizacion = infoOrganizacionRequest.Organizacion,
                DescripcionBreve = infoOrganizacionRequest.DescripcionBreve,
                DescripcionCompleta = infoOrganizacionRequest.DescripcionCompleta,
                Img = infoOrganizacionRequest.Img,
                OrganizacionId = infoOrganizacionRequest.OrganizacionId,
            };


            try
            {
                await _organizacionInfoService.SaveDataInfoOrganizacion(infoOrganizacion);
                return CreatedAtAction(nameof(Details), new { id = infoOrganizacionRequest.OrganizacionId }, infoOrganizacion);
            }
            catch(InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al crear la informacion de la organizacion", infoOrganizacion.OrganizacionId);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar la información de la organización");
                return StatusCode(500, "Internal server error");
            }

        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] InfoOrganizacionRequest infoOrganizacionRequest)
        {
            if (infoOrganizacionRequest == null)
            {
                return BadRequest("Datos de organización inválidos");
            }

            var organizacion = await _organizacionService.GetOrganizacionByIdAsync(infoOrganizacionRequest.OrganizacionId);

            if (organizacion == null)
            {
                return NotFound("Organización no encontrada");
            }

            var infoOrganizacion = new InfoOrganizacionDto
            {
                Organizacion = infoOrganizacionRequest.Organizacion,
                DescripcionBreve = infoOrganizacionRequest.DescripcionBreve,
                DescripcionCompleta = infoOrganizacionRequest.DescripcionCompleta,
                Img = infoOrganizacionRequest.Img,
                OrganizacionId = infoOrganizacionRequest.OrganizacionId,
            };

            try
            {
                await _organizacionInfoService.UpdateInfoOrganizacionAsync(infoOrganizacion);
                return CreatedAtAction(nameof(Update), new { id = infoOrganizacionRequest.OrganizacionId }, infoOrganizacion);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al actualizar la informacion de la organizacion", infoOrganizacion.OrganizacionId);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la información de la organización");
                return StatusCode(500, "Internal server error");
            }
        }

    }
}
