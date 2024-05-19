using backend.servicios.Config;
using backend.servicios.Interfaces;
using Newtonsoft.Json;
using System.Text;

namespace backend.servicios.Servicios
{
    /// <summary>
    /// Service for interacting with the Groq API to generate ideas.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="GroqApiService"/> class.
    /// </remarks>
    /// <param name="httpClientFactory">The HTTP client factory.</param>
    /// <param name="config">The Groq API configuration settings.</param>
    /// <exception cref="ArgumentNullException">Thrown if httpClientFactory or config is null.</exception>
    public class GroqApiService(IHttpClientFactory httpClientFactory, GroqApiConfig config) : IGenerateIdeaApiService
    {
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        private readonly GroqApiConfig _config = config ?? throw new ArgumentNullException(nameof(config));

        /// <summary>
        /// Generates an idea based on the provided user message.
        /// </summary>
        /// <param name="userMessage">The message from the user.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the generated idea as a string.</returns>
        public async Task<string> GenerateIdea(string userMessage)
        {
            var httpClient = GetHttpClient();
            var httpRequestContent = GetHttpMessageContent(userMessage);
            var response = await httpClient.PostAsync(_config.RequestUri, httpRequestContent);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Creates and configures an HTTP client.
        /// </summary>
        /// <returns>A configured HTTP client instance.</returns>
        private HttpClient GetHttpClient()
        {
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_config.ApiKey}");

            return httpClient;
        }

        /// <summary>
        /// Creates the HTTP message content for the API request.
        /// </summary>
        /// <param name="userMessage">The message from the user.</param>
        /// <returns>A <see cref="StringContent"/> instance containing the serialized request body.</returns>
        private StringContent GetHttpMessageContent(string userMessage)
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

        /// <summary>
        /// Gets the system message to be included in the API request.
        /// </summary>
        /// <returns>A string containing the system message.</returns>
        private static string GetSystemMessage() =>
            "Tu tarea es generar una sola idea de reciclaje utilizando sobrantes o retazos de tela, con los pasos detallados para llevarla a cabo.\n\n" +
            "La idea tiene que ser real, facil de hacer, pueden ser adornos o ropa.\n\n" +
            "Recuerda que solo puedes responder en español.\n\n" +
            "Usa los remantes entregados para generar la idea, debe ser acorde a las dimensiones de los remanentes\n\n" +
            "La respuesta final debe estar únicamente en formato JSON y debe contener:\n\n" +
            "{\n  " +
                "\"Idea\": \"Instrucciones\",\n  \"Pasos\": [\n    \"Paso 1\",\n    \"Paso 2\",\n    \"Paso 3\",\n    ...\n  ]\n}\n" +
            "Solo devuelve el JSON como respuesta final, no agregues nada mas";
    }
}
