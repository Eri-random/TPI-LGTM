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

        [HttpPost("Details")]
        public async Task<IActionResult> Details([FromBody] InfoOrganizationRequest infoOrganizationRequest)
        {
            if (infoOrganizationRequest == null)
            {
                return BadRequest("Datos de organización inválidos");
            }

            var organization = await _organizationService.GetOrganizationByIdAsync(infoOrganizationRequest.OrganizacionId);

            if (organization == null)
                return NotFound("Organización no encontrada");

            var infoOrganization = _mapper.Map<InfoOrganizationDto>(infoOrganizationRequest);

            try
            {
                await _organizationInfoService.SaveInfoOrganizationDataAsync(infoOrganization);
                return CreatedAtAction(nameof(Details), new { id = infoOrganizationRequest.OrganizacionId }, infoOrganization);
            }
            catch(InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al crear la informacion de la organizacion");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar la información de la organización");
                return StatusCode(500, "Internal server error");
            }

        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] InfoOrganizationRequest infoOrganizacionRequest)
        {
            if (infoOrganizacionRequest == null)
                return BadRequest("Datos de organización inválidos");

            var organization = await _organizationService.GetOrganizationByIdAsync(infoOrganizacionRequest.OrganizacionId);

            if (organization == null)
                return NotFound("Organización no encontrada");

            var infoOrganization = _mapper.Map<InfoOrganizationDto>(infoOrganizacionRequest);

            try
            {
                await _organizationInfoService.UpdateInfoOrganizationAsync(infoOrganization);
                return CreatedAtAction(nameof(Update), new { id = infoOrganizacionRequest.OrganizacionId }, infoOrganization);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al actualizar la informacion de la organizacion");
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
