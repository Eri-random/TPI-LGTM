namespace backend.servicios.Models
{
    public class OpenAIImageRequest
    {
        public string Prompt { get; set; }
        public int N { get; set; } = 1;
        public string Size { get; set; } = "1024x1024";

        public OpenAIImageRequest(string prompt, int n = 1, string size = "1024x1024")
        {
            Prompt = prompt;
            N = n;
            Size = size;
        }
    }

}
