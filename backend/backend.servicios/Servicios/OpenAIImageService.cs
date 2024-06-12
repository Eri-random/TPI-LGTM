using backend.servicios.Config;
using backend.servicios.Interfaces;
using backend.servicios.Models;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace backend.servicios.Servicios
{
    public class OpenAIImageService(IHttpClientFactory httpClientFactory, OpenAiApiConfig openAIConfig, ILogger<OpenAIImageService> logger) : IImageService
    {
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        private readonly OpenAiApiConfig _config = openAIConfig ?? throw new ArgumentNullException(nameof(openAIConfig));
        private readonly ILogger<OpenAIImageService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public async Task<OpenAIImageResponse> GenerateImageAsync(OpenAIImageRequest request)
        {
            var httpClient = GetHttpClient();
            var requestContent = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(_config.Url, requestContent);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<OpenAIImageResponse>(responseContent);
            }
            else
            {
                // Handle error response appropriately
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Error when generating image: {error}", errorContent);
                return null;
            }
        }

        private HttpClient GetHttpClient()
        {
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_config.ApiKey}");

            return httpClient;
        }
    }
}
