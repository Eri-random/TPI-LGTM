using backend.servicios.Config;
using backend.servicios.Interfaces;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace backend.servicios.Servicios
{
    public class GroqApiService : IGenerarIdeaApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly GroqApiConfig _config;

        public GroqApiService(IHttpClientFactory httpClientFactory, GroqApiConfig config)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _config = config ?? throw new ArgumentNullException(nameof(config));
    }
        public async Task<string> GetChatCompletionAsync(string userMessage)
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
                model = "llama3-8b-8192",
                temperature = 0,
                max_tokens = 1024,
                top_p = 1
            };

            var jsonContent = JsonConvert.SerializeObject(requestBody);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var httpClient = _httpClientFactory.CreateClient();

            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_config.ApiKey}");

            var response = await httpClient.PostAsync(_config.RequestUri, httpContent);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
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
