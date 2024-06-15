using AutoMapper;
using backend.api.Models;
using backend.servicios.DTOs;
using backend.servicios.Helpers;
using backend.servicios.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace backend.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HeadquartersController(
        IHeadquartersService headquartersService,
        ILogger<HeadquartersController> logger,
        IMapsService mapsService,
        IOrganizationService organizationService,
        IMapper mapper) : ControllerBase
    {
        private readonly IHeadquartersService _headquartersService = headquartersService ?? throw new ArgumentNullException(nameof(headquartersService));
        private readonly ILogger<HeadquartersController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IMapsService _mapsService = mapsService ?? throw new ArgumentNullException(nameof(mapsService));
        private readonly IOrganizationService _organizationService = organizationService ?? throw new ArgumentNullException(nameof(organizationService));
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        [HttpGet]
        public async Task<IActionResult> GetAllHeadquarters()
        {
            try
            {
                var headquarters = await _headquartersService.GetAllHeadquartersAsync();
                var headquartersResponse = _mapper.Map<IEnumerable<HeadquartersResponseModel>>(headquarters);

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
                return BadRequest("Datos de sede inválidos");

            var headquarters = _mapper.Map<IEnumerable<HeadquartersDto>>(headquartersRequestModels).ToList();

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
                var headquartersResponses = _mapper.Map<IEnumerable<HeadquartersResponseModel>>(headquarters);

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
                return BadRequest("Datos de sede inválidos");

            try
            {
                var headquarters = _mapper.Map<HeadquartersDto>(headquartersRequestModel);
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
                var headquartersResponse = _mapper.Map<HeadquartersResponseModel>(headquarters);

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
                var headquartersNearBy = _mapper.Map<HeadquartersNearby>(data);
                headquartersNearBy.Distancia = distanceOrg;

                return Ok(headquartersNearBy);
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
                var headquartersNearBy = _mapper.Map<HeadquartersNearby>(data);
                headquartersNearBy.Distancia = distanceOrg;

                return Ok(headquartersNearBy);
            }
            else
            {
                var organization = await _organizationService.GetOrganizationByIdAsync(nearestHeadquarters.Sede.OrganizacionId);
                if (organization == null)
                    return BadRequest("No se encontró la organización a la que pertenece la sede más cercana.");

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
