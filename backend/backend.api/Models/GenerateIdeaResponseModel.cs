using Newtonsoft.Json;

namespace backend.api.Models
{
    /// <summary>
    /// Model representing the response for a generated idea.
    /// </summary>
    public class GenerateIdeaResponseModel
    {
        /// <summary>
        /// Gets or sets the generated idea.
        /// </summary>
        public string Idea { get; set; }

        /// <summary>
        /// Gets or sets the steps associated with the generated idea.
        /// </summary>
        [JsonProperty("pasos")]
        public string[] Steps { get; set; }
    }
}
