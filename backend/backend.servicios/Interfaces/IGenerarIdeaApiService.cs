namespace backend.servicios.Interfaces
{
    public interface IGenerarIdeaApiService
    {
        Task<string> GetChatCompletionAsync(string userMessage);
    }
}
