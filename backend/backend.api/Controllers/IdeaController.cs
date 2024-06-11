using backend.api.Models;
using backend.servicios.DTOs;
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
    public class IdeaController(IGenerateIdeaApiService groqApiService, ILogger<IdeaController> logger, IIdeaService ideaService, IImageService imageService) : ControllerBase
    {
        private readonly IGenerateIdeaApiService _groqApiService = groqApiService ?? throw new ArgumentNullException(nameof(groqApiService));
        private readonly ILogger<IdeaController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IIdeaService _ideaService = ideaService ?? throw new ArgumentNullException(nameof(ideaService));
        private readonly IImageService _imageService = imageService ?? throw new ArgumentNullException(nameof(imageService));

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

                var imageGenerationRequest = new OpenAIImageRequest(ideaResponse.Idea);
                var image = await _imageService.GenerateImageAsync(imageGenerationRequest);

                if (image != null)
                    ideaResponse.ImageUrl = image.Data[0].Url;

                return Ok(ideaResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when generating idea");
                return StatusCode(500, $"Error when generating idea");
            }
        }


        [HttpPost("save")]
        public async Task<IActionResult> SaveIdea([FromBody] IdeaResponseModel idea)
        {
            if (idea == null)
            {
                _logger.LogWarning("Invalid request payload received.");
                return BadRequest("Invalid request payload");
            }

            try
            {
                var ideaDto = new IdeaDto
                {
                    Titulo = idea.Titulo,
                    UsuarioId = idea.UsuarioId,
                    Dificultad = idea.Dificultad,
                    Pasos = idea.Pasos.Select(paso => new StepDto
                    {
                        PasoNum = paso.PasoNum,
                        Descripcion = paso.Descripcion
                    }).ToList()
                };


                await _ideaService.SaveIdeaAsync(ideaDto);
                return CreatedAtAction(nameof(SaveIdea), new { id = idea.UsuarioId }, idea);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar la idea");
                return StatusCode(500, $"Error al guardar la idea");
            }
        }

        [HttpGet("user/{usuarioId}")]
        public async Task<IActionResult> GetIdeasByUsuarioId(int usuarioId)
        {
            try
            {
                var ideas = await _ideaService.GetIdeasByUserIdAsync(usuarioId);
                return Ok(ideas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las ideas del usuario");
                return StatusCode(500, $"Error al obtener las ideas del usuario");
            }
        }

        [HttpGet("see-detail/{ideaId}")]
        public async Task<IActionResult> GetIdeaById(int ideaId)
        {
            try
            {
                var idea = await _ideaService.GetIdeaByIdAsync(ideaId);

                if (idea == null)
                {
                    return NotFound("Idea no encontrada");
                }

                return Ok(idea);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la idea con id {ideaId}", ideaId);
                return StatusCode(500, "Error al obtener la idea");
            }
        }

        [HttpDelete("delete/{ideaId}")]
        public async Task<IActionResult> DeleteIdea(int ideaId)
        {
            try
            {
                await _ideaService.DeleteIdeaByIdAsync(ideaId);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar la idea con id {ideaId}", ideaId);
                return StatusCode(500, "Error al eliminar la idea");
            }
        }
    }
}
