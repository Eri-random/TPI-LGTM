using backend.api.Models;
using backend.servicios.DTOs;
using backend.servicios.Helpers;
using backend.servicios.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace backend.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SedeController(ISedeService sedeService, ILogger<SedeController> logger, IMapsService mapsService, IOrganizacionService organizacionService) : ControllerBase
    {
        private readonly ISedeService _sedeService = sedeService ?? throw new ArgumentNullException(nameof(sedeService));
        private readonly ILogger<SedeController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IMapsService _mapsService = mapsService ?? throw new ArgumentNullException(nameof(mapsService));
        private readonly IOrganizacionService _organizacionService = organizacionService ?? throw new ArgumentNullException(nameof(organizacionService));

        [HttpGet]
        public async Task<IActionResult> GetAllSedes()
        {
            try
            {
                var sedes = await _sedeService.GetAllSedesAsync();

                var sedesResponse = new List<SedeResponseModel>();

                foreach (var sede in sedes)
                {
                    sedesResponse.Add(new SedeResponseModel
                    {
                        Direccion = sede.Direccion,
                        Localidad = sede.Localidad,
                        Nombre = sede.Nombre,
                        Provincia = sede.Provincia,
                        Telefono = sede.Telefono,
                        Latitud = sede.Latitud,
                        Longitud = sede.Longitud,
                        OrganizacionId = sede.OrganizacionId
                    });
                }

                return Ok(sedesResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las sedes");
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpPost]
        public async Task<IActionResult> CreateSede([FromBody] List<SedeRequestModel> sedeRequestModel)
        {
            if (sedeRequestModel == null || !sedeRequestModel.Any())
            {
                return BadRequest("Datos de sede inválidos");
            }


            var sedes = sedeRequestModel.Select(sedeRequestModel => new SedeDto
            {
                Nombre = sedeRequestModel.Nombre,
                Direccion = sedeRequestModel.Direccion,
                Localidad = sedeRequestModel.Localidad,
                Provincia = sedeRequestModel.Provincia,
                Telefono = sedeRequestModel.Telefono,
                Latitud = 0,
                Longitud = 0,
                OrganizacionId = sedeRequestModel.OrganizacionId
            }).ToList();


            try
            {
                await _sedeService.createSedeAsync(sedes);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear la sede");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{organizacionId}")]
        public async Task<IActionResult> GetSedesByOrganizacionId(int organizacionId)
        {
            try
            {
                var sedes = await _sedeService.GetSedesByOrganizacionIdAsync(organizacionId);

                var sedesResponse = new List<SedeResponseModel>();

                foreach (var sede in sedes)
                {
                    sedesResponse.Add(new SedeResponseModel
                    {
                        Id = sede.Id,
                        Direccion = sede.Direccion,
                        Localidad = sede.Localidad,
                        Nombre = sede.Nombre,
                        Provincia = sede.Provincia,
                        Telefono = sede.Telefono,
                        Latitud = sede.Latitud,
                        Longitud = sede.Longitud,
                        OrganizacionId = sede.OrganizacionId
                    });
                }

                return Ok(sedesResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las sedes de la organización con id {OrganizacionId}", organizacionId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateSede([FromBody] SedeRequestModel sedeRequestModel)
        {
            if (sedeRequestModel == null)
            {
                return BadRequest("Datos de sede inválidos");
            }

            var sede = new SedeDto
            {
                Id = sedeRequestModel.Id,
                Nombre = sedeRequestModel.Nombre,
                Direccion = sedeRequestModel.Direccion,
                Localidad = sedeRequestModel.Localidad,
                Provincia = sedeRequestModel.Provincia,
                Telefono = sedeRequestModel.Telefono,
                Latitud = 0,
                Longitud = 0,
                OrganizacionId = sedeRequestModel.OrganizacionId
            };

            try
            {
                await _sedeService.updateSedeAsync(sede);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la sede");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{sedeId}")]
        public async Task<IActionResult> DeleteSede(int sedeId)
        {
            try
            {
                await _sedeService.deleteSedeAsync(sedeId);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar la sede con id {SedeId}", sedeId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("Detalle/{sedeId}")]
        public async Task<IActionResult> GetSedeById(int sedeId)
        {
            try
            {
                var sede = await _sedeService.GetSedeByIdAsync(sedeId);

                var sedeResponse = new SedeResponseModel
                {
                    Id = sede.Id,
                    Direccion = sede.Direccion,
                    Localidad = sede.Localidad,
                    Nombre = sede.Nombre,
                    Provincia = sede.Provincia,
                    Telefono = sede.Telefono,
                    OrganizacionId = sede.OrganizacionId
                };

                return Ok(sedeResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la sede con id {SedeId}", sedeId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("MasCercana")]
        public async Task<IActionResult> Evaluar([FromBody] DatosRequestModel datos)
        {
            if (datos == null || datos.Organizacion == null || datos.Usuario == null)
            {
                return BadRequest("Datos incompletos.");
            }

            var (latitudUsuario, longitudUsuario) = await _mapsService.GetCoordinates(datos.Usuario.Direccion, datos.Usuario.Localidad, datos.Usuario.Provincia);

            var distanciaOrg = sedeService.CalculateDistance(
                latitudUsuario, longitudUsuario,
                datos.Organizacion.Latitud, datos.Organizacion.Longitud
            );

            if (datos.Sedes == null || !datos.Sedes.Any())
            {
                // No hay sedes, devolver información de la organización
                return Ok(new SedeCercana
                {
                    Id = datos.Organizacion.Id,
                    Distancia = distanciaOrg,
                    Nombre = datos.Organizacion.Nombre,
                    Direccion = datos.Organizacion.Direccion,
                    Localidad = datos.Organizacion.Localidad,
                    Provincia = datos.Organizacion.Provincia,
                    Telefono = datos.Organizacion.Telefono,
                    Latitud = datos.Organizacion.Latitud,
                    Longitud = datos.Organizacion.Longitud
                });
            }

            // Calcular las distancias a las sedes
            var sedesConDistancia = datos.Sedes.Select(sede => new
            {
                Sede = sede,
                Distancia = sedeService.CalculateDistance(
                    latitudUsuario, longitudUsuario,
                    sede.Latitud, sede.Longitud
                )
            }).ToList();

            var sedeMasCercana = sedesConDistancia.OrderBy(sd => sd.Distancia).First();

            if (distanciaOrg < sedeMasCercana.Distancia)
            {
                return Ok(new SedeCercana
                {
                    Id = datos.Organizacion.Id,
                    Distancia = distanciaOrg,
                    Nombre = datos.Organizacion.Nombre,
                    Direccion = datos.Organizacion.Direccion,
                    Localidad = datos.Organizacion.Localidad,
                    Provincia = datos.Organizacion.Provincia,
                    Telefono = datos.Organizacion.Telefono,
                    Latitud = datos.Organizacion.Latitud,
                    Longitud = datos.Organizacion.Longitud
                });
            }
            else
            {
                var organizacion = await _organizacionService.GetOrganizacionByIdAsync(sedeMasCercana.Sede.OrganizacionId);
                if (organizacion == null)
                {
                    return BadRequest("No se encontró la organización a la que pertenece la sede más cercana.");
                }

                return Ok(new SedeCercana
                {
                    Id = sedeMasCercana.Sede.Id,
                    Distancia = sedeMasCercana.Distancia,
                    Nombre = sedeMasCercana.Sede.Nombre,
                    Direccion = sedeMasCercana.Sede.Direccion,
                    Localidad = sedeMasCercana.Sede.Localidad,
                    Provincia = sedeMasCercana.Sede.Provincia,
                    Telefono = sedeMasCercana.Sede.Telefono,
                    Latitud = sedeMasCercana.Sede.Latitud,
                    Longitud = sedeMasCercana.Sede.Longitud,
                    nombreOrganizacion = organizacion.Nombre
                });
            }

        }
    }
}
