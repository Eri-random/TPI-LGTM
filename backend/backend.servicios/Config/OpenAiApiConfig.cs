namespace backend.servicios.Config
{
    public class OpenAiApiConfig
    {
        public string ApiKey { get; set; }
        public string Url { get; set; }
        public bool GenerateStepsImages { get; set; } = true;
    }
}
