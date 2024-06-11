namespace backend.servicios.Models
{
    public class OpenAIImageRequest
    {
        public string prompt { get; set; }
        public int n { get; set; } = 1;
        public string size { get; set; } = "1024x1024";

        public OpenAIImageRequest(string requestPrompt, int n = 1, string size = "1024x1024")
        {
            prompt = requestPrompt;
        }
    }

}
