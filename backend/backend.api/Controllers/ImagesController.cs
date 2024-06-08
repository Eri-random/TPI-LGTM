using backend.api.Models;
using backend.servicios.Interfaces;
using backend.servicios.Models;
using Microsoft.AspNetCore.Mvc;

namespace backend.api.Controllers
{
    public class ImagesController(IImageService imageService) : ControllerBase
    {
        private readonly IImageService _imageService = imageService ?? throw new ArgumentNullException(nameof(imageService));

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateImage([FromBody] GenerateImageRequestModel model)
        {
            var request = new OpenAIImageRequest(model.Prompt);
            var result = await _imageService.GenerateImageAsync(request);
            return new OkObjectResult(result);
        }
    }
}
