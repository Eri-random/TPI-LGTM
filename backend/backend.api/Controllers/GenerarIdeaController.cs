using backend.api.Models;
using backend.servicios.Interfaces;
using backend.servicios.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace backend.api.Controllers
{
    /// <summary>
    /// Controller for generating ideas based on user input.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class IdeaController(IGenerateIdeaApiService groqApiService, ILogger<UsuariosController> logger) : ControllerBase
    {
        private readonly IGenerateIdeaApiService _groqApiService = groqApiService ?? throw new ArgumentNullException(nameof(groqApiService));
        private readonly ILogger<UsuariosController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));


        /// <summary>
        /// Generates an idea based on the user message.
        /// </summary>
        /// <param name="request">The request model containing the user message.</param>
        /// <returns>A response containing the generated idea.</returns>
        [HttpPost("generate")]
        public async Task<IActionResult> GenerateIdea([FromBody] GenerateIdeaRequestModel request)
        {
            if (request == null || string.IsNullOrEmpty(request.Message))
            {
                _logger.LogWarning("Invalid request payload received.");
                return BadRequest("Invalid request payload");
            }

            try
            {
                _logger.LogInformation("Received user message: {message}", request.Message);

                var result = await _groqApiService.GenerateIdea(request.Message);
                var chatResponse = JsonConvert.DeserializeObject<ChatResponseModel>(result);
                var idea = chatResponse?.Choices?.FirstOrDefault()?.Message?.Content;

                if (string.IsNullOrEmpty(idea))
                {
                    _logger.LogWarning("No idea generated from the message: {message}", request.Message);
                    return BadRequest("No idea generated.");
                }

                _logger.LogInformation("Generated idea: {idea}", idea);
                _logger.LogInformation("Total Tokens: {tokenInfo}", chatResponse?.Usage?.TotalTokens);
                _logger.LogInformation("Total time: {totalTime}", chatResponse?.Usage?.TotalTime);

                var ideaResponse = JsonConvert.DeserializeObject<GenerateIdeaResponseModel>(idea);
                return Ok(ideaResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when generating idea");
                return StatusCode(500, $"Error when generating idea");
            }
        }
    }
}
