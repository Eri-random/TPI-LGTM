using backend.servicios.DTOs;

namespace backend.servicios.Interfaces
{
    public interface IIdeaService
    {
        Task SaveIdeaAsync(IdeaDto ideaDto);
        Task<IEnumerable<IdeaDto>> GetIdeasByUserIdAsync(int userId);

        Task<IdeaDto> GetIdeaByIdAsync(int ideaId);

        Task DeleteIdeaByIdAsync(int ideaId);
    }
}
