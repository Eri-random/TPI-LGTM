namespace backend.servicios.Models
{
    /// <summary>
    /// Model representing the response from the chat API.
    /// </summary>
    public class ChatResponseModel
    {
        /// <summary>
        /// Gets or sets the unique identifier of the response.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the object type of the response.
        /// </summary>
        public string Object { get; set; }

        /// <summary>
        /// Gets or sets the creation timestamp of the response.
        /// </summary>
        public int Created { get; set; }

        /// <summary>
        /// Gets or sets the model used to generate the response.
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// Gets or sets the list of choices provided in the response.
        /// </summary>
        public List<Choice> Choices { get; set; }

        /// <summary>
        /// Gets or sets the usage information for the response.
        /// </summary>
        public Usage Usage { get; set; }
    }

    /// <summary>
    /// Model representing a choice in the chat response.
    /// </summary>
    public class Choice
    {
        /// <summary>
        /// Gets or sets the index of the choice.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets the message of the choice.
        /// </summary>
        public Message Message { get; set; }

        /// <summary>
        /// Gets or sets the log probabilities of the choice.
        /// </summary>
        public object Logprobs { get; set; }

        /// <summary>
        /// Gets or sets the reason why the choice was finished.
        /// </summary>
        public string FinishReason { get; set; }
    }

    /// <summary>
    /// Model representing a message in the chat response.
    /// </summary>
    public class Message
    {
        /// <summary>
        /// Gets or sets the role of the message sender.
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// Gets or sets the content of the message.
        /// </summary>
        public string Content { get; set; }
    }

    /// <summary>
    /// Model representing the usage information for the chat response.
    /// </summary>
    public class Usage
    {
        /// <summary>
        /// Gets or sets the number of tokens in the prompt.
        /// </summary>
        public int PromptTokens { get; set; }

        /// <summary>
        /// Gets or sets the time taken for the prompt.
        /// </summary>
        public double PromptTime { get; set; }

        /// <summary>
        /// Gets or sets the number of tokens in the completion.
        /// </summary>
        public int CompletionTokens { get; set; }

        /// <summary>
        /// Gets or sets the time taken for the completion.
        /// </summary>
        public double CompletionTime { get; set; }

        /// <summary>
        /// Gets or sets the total number of tokens used.
        /// </summary>
        public int TotalTokens { get; set; }

        /// <summary>
        /// Gets or sets the total time taken for the response.
        /// </summary>
        public double TotalTime { get; set; }
    }
}
