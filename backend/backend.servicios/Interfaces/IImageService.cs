using backend.servicios.Models;

namespace backend.servicios.Interfaces
{
    public interface IImageService 
    {
        Task<OpenAIImageResponse> GenerateImageAsync(OpenAIImageRequest request);
    }
}
