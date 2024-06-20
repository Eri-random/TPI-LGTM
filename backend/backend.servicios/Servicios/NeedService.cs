using AutoMapper;
using backend.data.Models;
using backend.repositories.interfaces;
using backend.servicios.DTOs;
using backend.servicios.Interfaces;
using Microsoft.Extensions.Logging;

namespace backend.servicios.Servicios
{
    public class NeedService(IRepository<Necesidad> repository, ILogger<NeedService> logger, IMapper mapper) : INeedService
    {
        private readonly IRepository<Necesidad> _necesidadRepository = repository ?? throw new ArgumentNullException(nameof(repository));
        private readonly ILogger<NeedService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        public async Task<IEnumerable<NeedDto>> GetAllNeedsAsync()
        {
            try
            {
                var necesidades = await _necesidadRepository.GetAllAsync(x => x.Subcategoria);

                return _mapper.Map<IEnumerable<NeedDto>>(necesidades);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las necesidades");
                throw;
            }
        }
    }
}
