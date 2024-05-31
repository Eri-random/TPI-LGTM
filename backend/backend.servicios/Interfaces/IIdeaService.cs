using backend.servicios.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
