namespace backend.servicios.Models
{
    public class ChatResponseModel
    {
        public string Id { get; set; }
        public string Object { get; set; }
        public int Created { get; set; }
        public string Model { get; set; }
        public List<Choice> Choices { get; set; }
        public Usage Usage { get; set; }
    }

    public class Choice
    {
        public int Index { get; set; }
        public Message Message { get; set; }
        public object Logprobs { get; set; }
        public string FinishReason { get; set; }
    }

    public class Message
    {
        public string Role { get; set; }
        public string Content { get; set; }
    }

    public class Usage
    {
        public int PromptTokens { get; set; }
        public double PromptTime { get; set; }
        public int CompletionTokens { get; set; }
        public double CompletionTime { get; set; }
        public int TotalTokens { get; set; }
        public double TotalTime { get; set; }
    }
}
