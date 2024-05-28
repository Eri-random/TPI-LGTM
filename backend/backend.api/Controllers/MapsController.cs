using backend.servicios.Interfaces;
using Microsoft.AspNetCore.Mvc;
using backend.servicios.Interfaces;
using backend.api.Models;

namespace backend.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MapsController(IOrganizacionService organizacionService, IMapsService mapsService, ISedeService sedeService) : ControllerBase
    {
        private readonly IOrganizacionService _organizacionService = organizacionService ?? throw new ArgumentNullException(nameof(organizacionService));
        private readonly IMapsService _mapsService = mapsService ?? throw new ArgumentNullException(nameof(mapsService));
        private readonly ISedeService _sedeService = sedeService ?? throw new ArgumentNullException(nameof(sedeService));

        [HttpGet]
        public async Task<IActionResult> GetOrganizationCoordinates()
        {
            var organizaciones = await _organizacionService.GetAllOrganizacionAsync();

            var organizacionesResponse = new List<OrganizacionResponseModel>();

            foreach (var organizacion in organizaciones)
            {
                organizacionesResponse.Add(new OrganizacionResponseModel
                {
                    Id = organizacion.Id,
                    Nombre = organizacion.Nombre,
                    Direccion = organizacion.Direccion,
                    Localidad = organizacion.Localidad,
                    Provincia = organizacion.Provincia,
                    Telefono = organizacion.Telefono,
                    Latitud = organizacion.Latitud,
                    Longitud = organizacion.Longitud
                });
            }

            return Ok(organizacionesResponse);
        }

        [HttpGet("{organizacionId}")]
        public async Task<IActionResult> GetOrganizationSedesCoordinates(int organizacionId)
        {
            var sedes = await _sedeService.GetSedesByOrganizacionIdAsync(organizacionId);
            return Ok(sedes);
        }
    }
}
