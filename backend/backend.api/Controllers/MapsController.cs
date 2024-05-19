using backend.servicios.Interfaces;
using Microsoft.AspNetCore.Mvc;
using backend.servicios.Interfaces;
using backend.api.Models;

namespace backend.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MapsController(IOrganizacionService organizacionService, IMapsService mapsService) : ControllerBase
    {
        private readonly IOrganizacionService _organizacionService = organizacionService ?? throw new ArgumentNullException(nameof(organizacionService));
        private readonly IMapsService _mapsService = mapsService ?? throw new ArgumentNullException(nameof(mapsService));
        [HttpGet]
        public async Task<IActionResult> GetOrganizationCoordinates()
        {

            var organizaciones = await _organizacionService.GetAllOrganizacionAsync();

            var coordinates = new List<CoordinatesResponseModel>();

            foreach (var org in organizaciones)
            {
                var (lat, lng) = await _mapsService.GetCoordinates(org.Direccion, org.Localidad, org.Provincia);
                var data = new CoordinatesResponseModel
                {
                    Lat = lat,
                    Lng = lng,
                    Nombre = org.Nombre,
                    Direccion = org.Direccion,
                    Localidad = org.Localidad,
                    Provincia = org.Provincia,
                    Telefono = org.Telefono
                };

                coordinates.Add(data);
            }


            return Ok(coordinates);
        }
    }
}
