using AutoMapper;
using backend.api.Models;
using backend.servicios.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace backend.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NeedController(INeedService needService, ILogger<NeedController> logger, IMapper mapper) : ControllerBase
    {
        private readonly INeedService _neeedService = needService ?? throw new ArgumentNullException(nameof(needService));
        private readonly ILogger<NeedController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        [HttpGet]
        public async Task<IActionResult> GetAllNeeds()
        {
            try
            {
                var needs = await _neeedService.GetAllNeedsAsync();
                var needsResponse = new List<NeedsResponseModel>();

                foreach (var need in needs)
                {
                    needsResponse.Add(_mapper.Map<NeedsResponseModel>(need));
                }

                return Ok(needsResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las necesidades");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
