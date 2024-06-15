using backend.api.Models;
using backend.servicios.DTOs;
using backend.servicios.Helpers;
using backend.servicios.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace backend.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HeadquartersController(IHeadquartersService headquartersService, ILogger<HeadquartersController> logger, IMapsService mapsService, IOrganizationService organizationService) : ControllerBase
    {
        private readonly IHeadquartersService _headquartersService = headquartersService ?? throw new ArgumentNullException(nameof(headquartersService));
        private readonly ILogger<HeadquartersController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IMapsService _mapsService = mapsService ?? throw new ArgumentNullException(nameof(mapsService));
        private readonly IOrganizationService _organizationService = organizationService ?? throw new ArgumentNullException(nameof(organizationService));

        [HttpGet]
        public async Task<IActionResult> GetAllHeadquarters()
        {
            try
            {
                var headquarters = await _headquartersService.GetAllHeadquartersAsync();

                var headquartersResponse = new List<HeadquartersResponseModel>();

                foreach (var h in headquarters)
                {
                    headquartersResponse.Add(new HeadquartersResponseModel
                    {
                        Direccion = h.Direccion,
                        Localidad = h.Localidad,
                        Nombre = h.Nombre,
                        Provincia = h.Provincia,
                        Telefono = h.Telefono,
                        Latitud = h.Latitud,
                        Longitud = h.Longitud,
                        OrganizacionId = h.OrganizacionId
                    });
                }

                return Ok(headquartersResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las sedes");
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpPost]
        public async Task<IActionResult> CreateSede([FromBody] List<HeadquartersRequestModel> headquartersRequestModels)
        {
            if (headquartersRequestModels == null || !headquartersRequestModels.Any())
            {
                return BadRequest("Datos de sede inválidos");
            }


            var headquarters = headquartersRequestModels.Select(sedeRequestModel => new HeadquartersDto
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
                await _headquartersService.CreateHeadquartersAsync(headquarters);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear la sede");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{organizationId}")]
        public async Task<IActionResult> GetHeadquartersByOrganizationId(int organizationId)
        {
            try
            {
                var headquarters = await _headquartersService.GetHeadquartersByOrganizationIdAsync(organizationId);

                var headquartersResponses = new List<HeadquartersResponseModel>();

                foreach (var h in headquarters)
                {
                    headquartersResponses.Add(new HeadquartersResponseModel
                    {
                        Id = h.Id,
                        Direccion = h.Direccion,
                        Localidad = h.Localidad,
                        Nombre = h.Nombre,
                        Provincia = h.Provincia,
                        Telefono = h.Telefono,
                        Latitud = h.Latitud,
                        Longitud = h.Longitud,
                        OrganizacionId = h.OrganizacionId
                    });
                }

                return Ok(headquartersResponses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las sedes de la organización con id {OrganizacionId}", organizationId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdatehHeadquarters([FromBody] HeadquartersRequestModel headquartersRequestModel)
        {
            if (headquartersRequestModel == null)
            {
                return BadRequest("Datos de sede inválidos");
            }

            var headquarters = new HeadquartersDto
            {
                Id = headquartersRequestModel.Id,
                Nombre = headquartersRequestModel.Nombre,
                Direccion = headquartersRequestModel.Direccion,
                Localidad = headquartersRequestModel.Localidad,
                Provincia = headquartersRequestModel.Provincia,
                Telefono = headquartersRequestModel.Telefono,
                Latitud = 0,
                Longitud = 0,
                OrganizacionId = headquartersRequestModel.OrganizacionId
            };

            try
            {
                await _headquartersService.UpdateHeadquartersAsync(headquarters);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la sede");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{headquartersId}")]
        public async Task<IActionResult> DeleteHeadquarters(int headquartersId)
        {
            try
            {
                await _headquartersService.DeleteHeadquartersAsync(headquartersId);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar la sede con id {SedeId}", headquartersId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("Details/{headquartersId}")]
        public async Task<IActionResult> GetHeadquartersById(int headquartersId)
        {
            try
            {
                var headquarters = await _headquartersService.GetHeadquarterByIdAsync(headquartersId);

                var headquartersResponse = new HeadquartersResponseModel
                {
                    Id = headquarters.Id,
                    Direccion = headquarters.Direccion,
                    Localidad = headquarters.Localidad,
                    Nombre = headquarters.Nombre,
                    Provincia = headquarters.Provincia,
                    Telefono = headquarters.Telefono,
                    OrganizacionId = headquarters.OrganizacionId
                };

                return Ok(headquartersResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la sede con id {SedeId}", headquartersId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("Closest")]
        public async Task<IActionResult> Evaluar([FromBody] DataRequestModel data)
        {
            if (data == null || data.Organizacion == null || data.Usuario == null)
            {
                return BadRequest("Datos incompletos.");
            }

            var (latitudeUser, lengthUser) = await _mapsService.GetCoordinates(data.Usuario.Direccion, data.Usuario.Localidad, data.Usuario.Provincia);

            var distanceOrg = DistanceCalculator.CalculateDistance(
                latitudeUser, lengthUser,
                data.Organizacion.Latitud, data.Organizacion.Longitud
            );

            if (data.Sedes == null || !data.Sedes.Any())
            {
                // No hay sedes, devolver información de la organización
                return Ok(new HeadquartersNearby
                {
                    Id = data.Organizacion.Id,
                    Distancia = distanceOrg,
                    Nombre = data.Organizacion.Nombre,
                    Direccion = data.Organizacion.Direccion,
                    Localidad = data.Organizacion.Localidad,
                    Provincia = data.Organizacion.Provincia,
                    Telefono = data.Organizacion.Telefono,
                    Latitud = data.Organizacion.Latitud,
                    Longitud = data.Organizacion.Longitud
                });
            }

            // Calcular las distancias a las sedes
            var headquartersWithDistance = data.Sedes.Select(sede => new
            {
                Sede = sede,
                Distancia = DistanceCalculator.CalculateDistance(
                    latitudeUser, lengthUser,
                    sede.Latitud, sede.Longitud
                )
            }).ToList();

            var nearestHeadquarters = headquartersWithDistance.OrderBy(sd => sd.Distancia).First();

            if (distanceOrg < nearestHeadquarters.Distancia)
            {
                return Ok(new HeadquartersNearby
                {
                    Id = data.Organizacion.Id,
                    Distancia = distanceOrg,
                    Nombre = data.Organizacion.Nombre,
                    Direccion = data.Organizacion.Direccion,
                    Localidad = data.Organizacion.Localidad,
                    Provincia = data.Organizacion.Provincia,
                    Telefono = data.Organizacion.Telefono,
                    Latitud = data.Organizacion.Latitud,
                    Longitud = data.Organizacion.Longitud
                });
            }
            else
            {
                var organization = await _organizationService.GetOrganizationByIdAsync(nearestHeadquarters.Sede.OrganizacionId);
                if (organization == null)
                {
                    return BadRequest("No se encontró la organización a la que pertenece la sede más cercana.");
                }

                return Ok(new HeadquartersNearby
                {
                    Id = nearestHeadquarters.Sede.Id,
                    Distancia = nearestHeadquarters.Distancia,
                    Nombre = nearestHeadquarters.Sede.Nombre,
                    Direccion = nearestHeadquarters.Sede.Direccion,
                    Localidad = nearestHeadquarters.Sede.Localidad,
                    Provincia = nearestHeadquarters.Sede.Provincia,
                    Telefono = nearestHeadquarters.Sede.Telefono,
                    Latitud = nearestHeadquarters.Sede.Latitud,
                    Longitud = nearestHeadquarters.Sede.Longitud,
                    nombreOrganizacion = organization.Nombre
                });
            }

        }
    }
}
