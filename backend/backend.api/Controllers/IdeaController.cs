using AutoMapper;
using backend.api.Models.RequestModels;
using backend.api.Models.ResponseModels;
using backend.servicios.Config;
using backend.servicios.DTOs;
using backend.servicios.Interfaces;
using backend.servicios.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;


namespace backend.api.Controllers
{
    /// <summary>
    /// Controller for generating ideas based on user input.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class IdeaController(IGenerateIdeaApiService groqApiService, ILogger<IdeaController> logger, IIdeaService ideaService, IImageService imageService, IMapper mapper, OpenAiApiConfig config) : ControllerBase
    {
        private readonly IGenerateIdeaApiService _groqApiService = groqApiService ?? throw new ArgumentNullException(nameof(groqApiService));
        private readonly ILogger<IdeaController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IIdeaService _ideaService = ideaService ?? throw new ArgumentNullException(nameof(ideaService));
        private readonly IImageService _imageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        private readonly OpenAiApiConfig _openAiApiConfig = config ?? throw new ArgumentNullException(nameof(config));

        /// <summary>
        /// Generates an idea based on the user message.
        /// </summary>
        /// <param name="request">The request model containing the user message.</param>
        /// <response code="200">Returns the generated idea.</response>
        /// <response code="400">If the request payload is invalid.</response>
        /// <response code="500">If there is an internal server error.</response>
        /// 
        [Authorize(Roles = "usuario")]
        [HttpPost("generate")]
        [ProducesResponseType(typeof(GenerateIdeaResponseModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
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
                var image = await GenerateMainImage(ideaResponse.Idea);

                if (!string.IsNullOrEmpty(image))
                    ideaResponse.ImageUrl = image;

                if (_openAiApiConfig.GenerateStepsImages)
                {
                    // Generate images for each step
                    var stepImageTasks = GenerateStepsImage(ideaResponse.Steps);
                    var stepImages = await Task.WhenAll(stepImageTasks);

                    for (int i = 0; i < ideaResponse.Steps.Length; i++)
                    {
                        if (stepImages[i]?.Data != null && stepImages[i].Data.Count > 0)
                        {
                            ideaResponse.Steps[i] += $" ImageURL: {stepImages[i].Data[0].Url}";
                        }
                    }
                }

                return StatusCode(StatusCodes.Status201Created,ideaResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when generating idea");

                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Saves an idea.
        /// </summary>
        /// <param name="idea">The idea request model.</param>
        /// <response code="201">Returns the saved idea.</response>
        /// <response code="400">If the request payload is invalid.</response>
        /// <response code="500">If there is an internal server error.</response>
        /// 
        [Authorize(Roles = "usuario")]
        [HttpPost("save")]
        [ProducesResponseType(typeof(IdeaResponseModel), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SaveIdea([FromBody] IdeaRequestModel idea)
        {
            if (idea == null)
            {
                _logger.LogWarning("Invalid request payload received.");
                return BadRequest("Invalid request payload");
            }

            try
            {
                var ideaDto = _mapper.Map<IdeaDto>(idea);
                await _ideaService.SaveIdeaAsync(ideaDto);

                return CreatedAtAction(nameof(SaveIdea), new { id = idea.UsuarioId }, idea);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving the idea");
                return StatusCode(500, "Internal server error");
            }
        }


        /// <summary>
        /// Get ideas by user ID.
        /// </summary>
        /// <param name="usuarioId">The ID of the user.</param>
        /// <response code="200">Returns the list of ideas.</response>
        /// <response code="500">If there is an internal server error.</response>
        /// 
        [Authorize(Roles = "usuario")]
        [HttpGet("user/{usuarioId}")]
        [ProducesResponseType(typeof(IEnumerable<IdeaResponseModel>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetIdeasByUsuarioId(int usuarioId)
        {
            try
            {
                var ideas = await _ideaService.GetIdeasByUserIdAsync(usuarioId);
                var ideaResponse = _mapper.Map<IEnumerable<IdeaResponseModel>>(ideas);

                return Ok(ideaResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user ideas");
                return StatusCode(500, "Internal server error");
            }
        }


        /// <summary>
        /// Get an idea by ID.
        /// </summary>
        /// <param name="ideaId">The ID of the idea.</param>
        /// <response code="200">Returns the idea.</response>
        /// <response code="404">If the idea is not found.</response>
        /// <response code="500">If there is an internal server error.</response>
        /// 
        [Authorize(Roles = "usuario")]
        [HttpGet("see-detail/{ideaId}")]
        [ProducesResponseType(typeof(IdeaResponseModel), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetIdeaById(int ideaId)
        {
            try
            {
                var idea = await _ideaService.GetIdeaByIdAsync(ideaId);

                if (idea == null)
                    return NotFound("Idea not found");

                var ideaResponse = _mapper.Map<IdeaResponseModel>(idea);

                return Ok(ideaResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving idea with ID {ideaId}", ideaId);
                return StatusCode(500, "Internal server error");
            }
        }


        /// <summary>
        /// Delete an idea by ID.
        /// </summary>
        /// <param name="ideaId">The ID of the idea.</param>
        /// <response code="200">If the idea was successfully deleted.</response>
        /// <response code="500">If there is an internal server error.</response>
        /// 
        [Authorize(Roles = "usuario")]
        [HttpDelete("delete/{ideaId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteIdea(int ideaId)
        {
            try
            {
                await _ideaService.DeleteIdeaByIdAsync(ideaId);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting idea with ID {ideaId}", ideaId);
                return StatusCode(500, "Internal server error");
            }
        }

        private async Task<string> GenerateMainImage(string idea)
        {
            var imageGenerationRequest = new OpenAIImageRequest(idea);
            var imageResult = await _imageService.GenerateImageAsync(imageGenerationRequest);

            return imageResult?.Data != null && imageResult.Data.Count > 0 ? imageResult.Data[0].Url : null;
        }

        private Task<OpenAIImageResponse>[] GenerateStepsImage(string[] steps)
        {
            return steps.Select(step => _imageService.GenerateImageAsync(new OpenAIImageRequest(step))).ToArray();
        }
    }
}
