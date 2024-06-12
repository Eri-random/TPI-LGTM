using System.Text.Json.Serialization;

namespace backend.servicios.Models
{
    public class OpenAIImageResponse
    {
        [JsonPropertyName("created")]
        public long Created { get; set; }

        [JsonPropertyName("data")]
        public List<ImageData> Data { get; set; }
    }

    public class ImageData
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }
    }
}
