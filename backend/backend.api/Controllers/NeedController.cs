using AutoMapper;
using backend.api.Models.ResponseModels;
using backend.servicios.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace backend.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NeedController(INeedService needService, ILogger<NeedController> logger, IMapper mapper) : ControllerBase
    {
        private readonly INeedService _needService = needService ?? throw new ArgumentNullException(nameof(needService));
        private readonly ILogger<NeedController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        /// <summary>
        /// Get all needs.
        /// </summary>
        /// <response code="200">Returns the list of needs.</response>
        /// <response code="500">If there is an internal server error.</response>
        /// 
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<NeedsResponseModel>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllNeeds()
        {
            try
            {
                var needs = await _needService.GetAllNeedsAsync();
                var needsResponse = _mapper.Map<IEnumerable<NeedsResponseModel>>(needs);

                return Ok(needsResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all needs");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
