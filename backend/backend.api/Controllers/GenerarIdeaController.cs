using backend.api.Models;
using backend.servicios.Interfaces;
using backend.servicios.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace backend.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IdeaController(IGenerateIdeaApiService groqApiService) : ControllerBase
    {
        private readonly IGenerateIdeaApiService _groqApiService = groqApiService ?? throw new ArgumentNullException(nameof(groqApiService));

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateIdea([FromBody] GenerateIdeaRequestModel request)
        {
            if (request == null || string.IsNullOrEmpty(request.Message))
                return BadRequest("Invalid request payload");

            try
            {
                var result = await _groqApiService.GenerateIdea(request.Message);
                var chatResponse = JsonConvert.DeserializeObject<ChatResponseModel>(result);
                var idea = JsonConvert.DeserializeObject<GenerateIdeaResponseModel>(chatResponse?.Choices?.FirstOrDefault()?.Message?.Content);

                return Ok(idea);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
