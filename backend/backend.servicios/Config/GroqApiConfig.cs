namespace backend.servicios.Config
{
    /// <summary>
    /// Configuration settings for the Groq API.
    /// </summary>
    public class GroqApiConfig
    {
        /// <summary>
        /// Gets or sets the API key for accessing the Groq API.
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// Gets or sets the request URI for the Groq API.
        /// </summary>
        public string RequestUri { get; set; }

        /// <summary>
        /// Gets or sets the language model to be used by the Groq API.
        /// </summary>
        public string LlmModel { get; set; }
    }
}
