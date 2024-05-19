namespace backend.servicios.Interfaces
{
    public interface IGenerateIdeaApiService
    {
        Task<string> GenerateIdea(string userMessage);
    }
}
