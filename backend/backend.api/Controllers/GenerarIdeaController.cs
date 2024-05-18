using backend.api.Models;
using backend.servicios.Interfaces;
using backend.servicios.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace backend.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IGenerarIdeaApiService _groqApiService;

        public ChatController(IGenerarIdeaApiService groqApiService)
        {
            _groqApiService = groqApiService ?? throw new ArgumentNullException(nameof(groqApiService));
        }

        [HttpPost("generar")]
        public async Task<IActionResult> GetChatCompletion([FromBody] GenerarIdeaRequestModel request)
        {
            if (request == null || string.IsNullOrEmpty(request.Message))
            {
                return BadRequest("Invalid request payload");
            }

            try
            {
                var result = await _groqApiService.GetChatCompletionAsync(request.Message);
                var chatResponse = JsonConvert.DeserializeObject<ChatResponseModel>(result);
                var assistantMessage = chatResponse?.Choices?.FirstOrDefault()?.Message?.Content;

                return Ok(assistantMessage);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
