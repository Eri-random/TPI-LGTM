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
        public async Task<IActionResult> Details([FromForm] InfoOrganizacionRequest infoOrganizacionRequest)
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

            string folderPath = Path.Combine("wwwroot", "images");
            if (!Directory.Exists(folderPath))
            {
                _logger.LogInformation("El directorio no existe, creando: {FolderPath}", folderPath);
                Directory.CreateDirectory(folderPath);
            }
            string filePath = Path.Combine(folderPath, infoOrganizacionRequest.File.FileName);

            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await infoOrganizacionRequest.File.CopyToAsync(stream);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar la imagen de la organización");
                return StatusCode(500, "Internal server error");
            }

            string fileUrl = $"http://localhost:5203/images/{infoOrganizacionRequest.File.FileName}"; // Cambia esto según sea necesario

            var infoOrganizacion = new InfoOrganizacionDto
            {
                Organizacion = infoOrganizacionRequest.Organizacion,
                DescripcionBreve = infoOrganizacionRequest.DescripcionBreve,
                DescripcionCompleta = infoOrganizacionRequest.DescripcionCompleta,
                Img = fileUrl,
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
        public async Task<IActionResult> Update([FromForm] InfoOrganizacionRequest infoOrganizacionRequest)
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

            string fileUrl = organizacion.InfoOrganizacion.Img;

            if (infoOrganizacionRequest.File != null)
            {
                string folderPath = Path.Combine("wwwroot", "images");

             
                string newFilePath = Path.Combine(folderPath, infoOrganizacionRequest.File.FileName);

                try
                {
                    // Eliminar la imagen antigua si existe
                    if (!string.IsNullOrEmpty(organizacion.InfoOrganizacion.Img))
                    {
                        string oldFilePath = Path.Combine("wwwroot", "images", Path.GetFileName(organizacion.InfoOrganizacion.Img));
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

            var infoOrganizacion = new InfoOrganizacionDto
            {
                Organizacion = infoOrganizacionRequest.Organizacion,
                DescripcionBreve = infoOrganizacionRequest.DescripcionBreve,
                DescripcionCompleta = infoOrganizacionRequest.DescripcionCompleta,
                Img = fileUrl,
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
