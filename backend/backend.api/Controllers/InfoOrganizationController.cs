using backend.api.Models;
using backend.servicios.DTOs;
using backend.servicios.Helpers;
using backend.servicios.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace backend.api.Controllers
{
    [Route("api/Information")]
    [ApiController]
    public class InfoOrganizationController(IOrganizationService organizationService, IOrganizationInfoService organizationInfoService, ILogger<UserController> logger) : ControllerBase
    {
        private readonly IOrganizationService _organizationService = organizationService ?? throw new ArgumentNullException(nameof(organizationService));
        private readonly ILogger<UserController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IOrganizationInfoService _organizationInfoService = organizationInfoService ?? throw new ArgumentNullException(nameof(organizationInfoService));

        [HttpPost("Details")]
        public async Task<IActionResult> Details([FromForm] InfoOrganizationRequest infoOrganizationRequest)
        {
            if (infoOrganizationRequest == null)
            {
                return BadRequest("Datos de organización inválidos");
            }

            var organization = await _organizationService.GetOrganizationByIdAsync(infoOrganizationRequest.OrganizacionId);

            if (organization == null)
            {
                return NotFound("Organización no encontrada");
            }

            string folderPath = Path.Combine("wwwroot", "images");
            if (!Directory.Exists(folderPath))
            {
                _logger.LogInformation("El directorio no existe, creando: {FolderPath}", folderPath);
                Directory.CreateDirectory(folderPath);
            }
            string filePath = Path.Combine(folderPath, infoOrganizationRequest.File.FileName);

            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await infoOrganizationRequest.File.CopyToAsync(stream);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar la imagen de la organización");
                return StatusCode(500, "Internal server error");
            }

            string fileUrl = $"http://localhost:5203/images/{infoOrganizationRequest.File.FileName}"; // Cambia esto según sea necesario

            var infoOrganization = new InfoOrganizationDto
            {
                Organizacion = infoOrganizationRequest.Organizacion,
                DescripcionBreve = infoOrganizationRequest.DescripcionBreve,
                DescripcionCompleta = infoOrganizationRequest.DescripcionCompleta,
                Img = fileUrl,
                OrganizacionId = infoOrganizationRequest.OrganizacionId,
            };


            try
            {
                await _organizationInfoService.SaveInfoOrganizationData(infoOrganization);
                return CreatedAtAction(nameof(Details), new { id = infoOrganizationRequest.OrganizacionId }, infoOrganization);
            }
            catch(InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al crear la informacion de la organizacion", infoOrganization.OrganizacionId);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar la información de la organización");
                return StatusCode(500, "Internal server error");
            }

        }

        [HttpPut]
        public async Task<IActionResult> Update([FromForm] InfoOrganizationRequest infoOrganizacionRequest)
        {
            if (infoOrganizacionRequest == null)
            {
                return BadRequest("Datos de organización inválidos");
            }

            var organization = await _organizationService.GetOrganizationByIdAsync(infoOrganizacionRequest.OrganizacionId);

            if (organization == null)
            {
                return NotFound("Organización no encontrada");
            }

            string fileUrl = organization.InfoOrganizacion.Img;

            if (infoOrganizacionRequest.File != null)
            {
                string folderPath = Path.Combine("wwwroot", "images");

             
                string newFilePath = Path.Combine(folderPath, infoOrganizacionRequest.File.FileName);

                try
                {
                    // Eliminar la imagen antigua si existe
                    if (!string.IsNullOrEmpty(organization.InfoOrganizacion.Img))
                    {
                        string oldFilePath = Path.Combine("wwwroot", "images", Path.GetFileName(organization.InfoOrganizacion.Img));
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    // Guardar la nueva imagen
                    using (var stream = new FileStream(newFilePath, FileMode.Create))
                    {
                        await infoOrganizacionRequest.File.CopyToAsync(stream);
                    }

                    fileUrl = $"http://localhost:5203/images/{infoOrganizacionRequest.File.FileName}"; 
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al guardar la imagen de la organización");
                    return StatusCode(500, "Internal server error");
                }
            }

            var infoOrganization = new InfoOrganizationDto
            {
                Organizacion = infoOrganizacionRequest.Organizacion,
                DescripcionBreve = infoOrganizacionRequest.DescripcionBreve,
                DescripcionCompleta = infoOrganizacionRequest.DescripcionCompleta,
                Img = fileUrl,
                OrganizacionId = infoOrganizacionRequest.OrganizacionId,
            };

            try
            {
                await _organizationInfoService.UpdateInfoOrganizationAsync(infoOrganization);
                return CreatedAtAction(nameof(Update), new { id = infoOrganizacionRequest.OrganizacionId }, infoOrganization);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al actualizar la informacion de la organizacion", infoOrganization.OrganizacionId);
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
