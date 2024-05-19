namespace backend.servicios.Interfaces
{
    /// <summary>
    /// Service interface for generating ideas based on user input.
    /// </summary>
    public interface IGenerateIdeaApiService
    {
        /// <summary>
        /// Generates an idea based on the provided user message.
        /// </summary>
        /// <param name="userMessage">The message from the user.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the generated idea as a string.</returns>
        Task<string> GenerateIdea(string userMessage);
    }
}
