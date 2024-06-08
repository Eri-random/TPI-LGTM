using backend.servicios.Config;
using backend.servicios.Interfaces;
using backend.servicios.Models;
using System.Text;
using System.Text.Json;

namespace backend.servicios.Servicios
{
    public class OpenAIImageService(IHttpClientFactory httpClientFactory, OpenAiApiConfig openAIConfig) : IImageService
    {
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        private readonly OpenAiApiConfig _config = openAIConfig ?? throw new ArgumentNullException(nameof(openAIConfig));

        public async Task<OpenAIImageResponse> GenerateImageAsync(OpenAIImageRequest request)
        {
            var httpClient = GetHttpClient();
            var requestContent = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync("https://api.openai.com/v1/images/generations", requestContent);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<OpenAIImageResponse>(responseContent);
            }
            else
            {
                // Handle error response appropriately
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"OpenAI API request failed: {errorContent}");
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
