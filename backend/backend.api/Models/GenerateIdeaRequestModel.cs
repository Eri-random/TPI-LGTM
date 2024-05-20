namespace backend.api.Models
{
    /// <summary>
    /// Model representing the request to generate an idea.
    /// </summary>
    public class GenerateIdeaRequestModel
    {
        /// <summary>
        /// Gets or sets the message from the user.
        /// </summary>
        public string Message { get; set; }
    }
}