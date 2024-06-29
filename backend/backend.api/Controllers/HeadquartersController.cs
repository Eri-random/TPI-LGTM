using AutoMapper;
using backend.api.Models.RequestModels;
using backend.api.Models.ResponseModels;
using backend.servicios.DTOs;
using backend.servicios.Helpers;
using backend.servicios.Interfaces;
using backend.servicios.Servicios;
using Microsoft.AspNetCore.Authorization;
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

        /// <summary>
        /// Get all headquarters.
        /// </summary>
        /// <response code="200">Returns the list of all headquarters.</response>
        /// <response code="500">If there is an internal server error.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<HeadquartersResponseModel>), 200)]
        [ProducesResponseType(500)]
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
                _logger.LogError(ex, "Error retrieving all headquarters");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Create new headquarters.
        /// </summary>
        /// <param name="headquartersRequestModels">The list of headquarters request models.</param>
        /// <response code="200">If the headquarters were successfully created.</response>
        /// <response code="400">If the request payload is invalid.</response>
        /// <response code="500">If there is an internal server error.</response>
        /// 
        [Authorize(Roles = "organizacion")]
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateHeadquarter([FromBody] List<HeadquartersRequestModel> headquartersRequestModels)
        {
            if (headquartersRequestModels == null || !headquartersRequestModels.Any())
                return BadRequest("Invalid headquarters data");

            try
            {
                var headquarters = _mapper.Map<IEnumerable<HeadquartersDto>>(headquartersRequestModels).ToList();
                await _headquartersService.CreateHeadquartersAsync(headquarters);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating headquarters");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get headquarters by organization ID.
        /// </summary>
        /// <param name="organizationId">The ID of the organization.</param>
        /// <response code="200">Returns the list of headquarters.</response>
        /// <response code="500">If there is an internal server error.</response>
        [HttpGet("organization/{organizationId}")]
        [ProducesResponseType(typeof(IEnumerable<HeadquartersResponseModel>), 200)]
        [ProducesResponseType(500)]
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
                _logger.LogError(ex, "Error retrieving headquarters for organization ID {organizationId}", organizationId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Update headquarters.
        /// </summary>
        /// <param name="headquartersRequestModel">The headquarters request model.</param>
        /// <response code="200">If the headquarters were successfully updated.</response>
        /// <response code="400">If the request payload is invalid.</response>
        /// <response code="500">If there is an internal server error.</response>
        /// 
        [Authorize(Roles = "organizacion")]
        [HttpPut]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateHeadquarters([FromBody] HeadquartersRequestModel headquartersRequestModel)
        {
            if (headquartersRequestModel == null)
                return BadRequest("Invalid headquarters data");

            try
            {
                var headquarters = _mapper.Map<HeadquartersDto>(headquartersRequestModel);
                await _headquartersService.UpdateHeadquartersAsync(headquarters);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating headquarters");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Delete headquarters by ID.
        /// </summary>
        /// <param name="headquartersId">The ID of the headquarters.</param>
        /// <response code="200">If the headquarters were successfully deleted.</response>
        /// <response code="500">If there is an internal server error.</response>
        /// 
        [Authorize(Roles = "organizacion")]
        [HttpDelete("{headquartersId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteHeadquarters(int headquartersId)
        {
            try
            {
                await _headquartersService.DeleteHeadquartersAsync(headquartersId);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting headquarters with ID {headquartersId}", headquartersId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get headquarters details by ID.
        /// </summary>
        /// <param name="headquartersId">The ID of the headquarters.</param>
        /// <response code="200">Returns the headquarters details.</response>
        /// <response code="500">If there is an internal server error.</response>
        [HttpGet("{headquartersId}")]
        [ProducesResponseType(typeof(HeadquartersResponseModel), 200)]
        [ProducesResponseType(500)]
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
                _logger.LogError(ex, "Error retrieving headquarters with ID {headquartersId}", headquartersId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Evaluate the closest headquarters.
        /// </summary>
        /// <param name="data">The data request model.</param>
        /// <response code="200">Returns the closest headquarters or organization information.</response>
        /// <response code="400">If the request payload is invalid.</response>
        /// <response code="500">If there is an internal server error.</response>
        [HttpPost("Closest")]
        [ProducesResponseType(typeof(HeadquartersNearbyResponseModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Evaluate([FromBody] DataRequestModel data)

        {
            if (data == null || data.Organizacion == null || data.Usuario == null)
                return BadRequest("Incomplete data.");

            try
            {
                var (latitudeUser, longitudeUser) = await _mapsService.GetCoordinates(data.Usuario.Direccion, data.Usuario.Localidad, data.Usuario.Provincia);
                var distanceOrg = DistanceCalculator.CalculateDistance(latitudeUser, longitudeUser, data.Organizacion.Latitud, data.Organizacion.Longitud);

                if (data.Sedes == null || !data.Sedes.Any())
                {
                    // No headquarters, return organization information
                    var headquartersNearby = _mapper.Map<HeadquartersNearbyDto>(data.Organizacion);
                    headquartersNearby.Distancia = distanceOrg;

                    return Ok(headquartersNearby);
                }

                // Calculate distances to headquarters
                var headquartersWithDistance = data.Sedes.Select(sede => new
                {
                    Sede = sede,
                    Distancia = DistanceCalculator.CalculateDistance(latitudeUser, longitudeUser, sede.Latitud, sede.Longitud)
                }).ToList();

                var nearestHeadquarters = headquartersWithDistance.OrderBy(sd => sd.Distancia).First();

                if (distanceOrg < nearestHeadquarters.Distancia)
                {
                    var headquartersNearby = _mapper.Map<HeadquartersNearbyDto>(data.Organizacion);
                    headquartersNearby.Distancia = distanceOrg;

                    return Ok(headquartersNearby);
                }
                else
                {
                    var organization = await _organizationService.GetOrganizationByIdAsync(nearestHeadquarters.Sede.OrganizacionId);
                    if (organization == null)
                        return BadRequest("Organization not found for the nearest headquarters.");

                    return Ok(new HeadquartersNearbyResponseModel
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error evaluating the closest headquarters");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
