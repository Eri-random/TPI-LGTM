using AutoMapper;
using backend.api.Models;
using backend.servicios.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace backend.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MapsController(IOrganizationService organizationService, IMapsService mapsService, IHeadquartersService headquartersService, IMapper mapper) : ControllerBase
    {
        private readonly IOrganizationService _organizationService = organizationService ?? throw new ArgumentNullException(nameof(organizationService));
        private readonly IMapsService _mapsService = mapsService ?? throw new ArgumentNullException(nameof(mapsService));
        private readonly IHeadquartersService _headquartersService = headquartersService ?? throw new ArgumentNullException(nameof(headquartersService));
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        [HttpGet]
        public async Task<IActionResult> GetOrganizationCoordinates()
        {
            var organizations = await _organizationService.GetAllOrganizationAsync();

            var organizationsResponse = new List<OrganizationResponseModel>();

            foreach (var org in organizations)
            {
                organizationsResponse.Add(_mapper.Map<OrganizationResponseModel>(org));
            }

            return Ok(organizationsResponse);
        }

        [HttpGet("{organizationId}")]
        public async Task<IActionResult> GetOrganizationHeadquartersCoordinates(int organizationId)
        {
            var sedes = await _headquartersService.GetHeadquartersByOrganizationIdAsync(organizationId);
            return Ok(sedes);
        }
    }
}
