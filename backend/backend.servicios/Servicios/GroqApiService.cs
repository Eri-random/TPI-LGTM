using backend.servicios.Config;
using backend.servicios.Interfaces;
using Newtonsoft.Json;
using System.Text;

namespace backend.servicios.Servicios
{
    public class GroqApiService(IHttpClientFactory httpClientFactory, GroqApiConfig config) : IGenerateIdeaApiService
    {
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        private readonly GroqApiConfig _config = config ?? throw new ArgumentNullException(nameof(config));

        public async Task<string> GenerateIdea(string userMessage)
        {
            var httpClient = GetHttpClient();
            var httpRequestContent = GetHttpMessageContent(userMessage);
            var response = await httpClient.PostAsync(_config.RequestUri, httpRequestContent);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        public HttpClient GetHttpClient()
        {
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_config.ApiKey}");

            return httpClient;
        }

        public StringContent GetHttpMessageContent(string userMessage)
        {
            var requestBody = new
            {
                messages = new[]
                {
                    new
                    {
                        role = "system",
                        content = GetSystemMessage()
                    },
                    new
                    {
                        role = "user",
                        content = userMessage
                    }
                },
                model = _config.LlmModel,
                temperature = 0,
                max_tokens = 2048,
                top_p = 1,
                stream = false,
                response_format = new
                {
                    type = "json_object"
                }
            };

            var jsonContent = JsonConvert.SerializeObject(requestBody);
            var httpMessageContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            return httpMessageContent;
        }

        public static string GetSystemMessage() =>
            "Tu tarea es generar una sola idea de reciclaje utilizando sobrantes o retazos de tela, con los pasos detallados para llevarla a cabo.\n\n" +
            "La idea tiene que ser real, facil de hacer, pueden ser adornos o ropa.\n\n" +
            "Recuerda que solo puedes responder en español.\n\n" +
            "Usa los remantes entregados para generar la idea, debe ser acorde a las dimensiones de los remanentes\n\n" +
            "La respuesta final debe estar únicamente en formato JSON y debe contener:\n\n" +
            "{\n  " +
                "\"Idea\": " +
                "\"Instrucciones\",\n  \"Pasos\": [\n    \"Paso 1\",\n    \"Paso 2\",\n    \"Paso 3\",\n    ...\n  ]\n}\n" +
            "Solo devuelve el JSON como respuesta final, no agregues nada mas";
    }
}
