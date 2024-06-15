using backend.servicios.DTOs;

namespace backend.servicios.Interfaces
{
    public interface INeedService
    {
        Task<IEnumerable<NeedDto>> GetAllNeedsAsync();
    }
}
