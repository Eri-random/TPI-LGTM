using Newtonsoft.Json;

namespace backend.api.Models
{
    public class GenerateIdeaResponseModel
    {
        public string Idea {  get; set; }
        [JsonProperty("pasos")]
        public string[] Steps { get; set; }
    }
}
