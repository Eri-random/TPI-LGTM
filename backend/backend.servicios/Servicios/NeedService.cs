using backend.data.Models;
using backend.repositories.interfaces;
using backend.servicios.DTOs;
using backend.servicios.Interfaces;
using Microsoft.Extensions.Logging;

namespace backend.servicios.Servicios
{
    public class NeedService(IRepository<Necesidad> repository, ILogger<NeedService> logger) : INeedService
    {
        private readonly IRepository<Necesidad> _necesidadRepository = repository ?? throw new ArgumentNullException(nameof(repository));
        private readonly ILogger<NeedService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public async Task<IEnumerable<NeedDto>> GetAllNeedsAsync()
        {
            try
            {
                var necesidades = await _necesidadRepository.GetAllAsync(x => x.Subcategoria);

                return necesidades
                    .Select(u => new NeedDto
                    {
                        Id = u.Id,
                        Nombre = u.Nombre,
                        Icono = u.Icono,
                        Subcategoria = u.Subcategoria.Select(p => new SubcategoriesDto
                        {
                            Id = p.Id,
                            Nombre = p.Nombre,
                            NecesidadId = p.NecesidadId
                        }).ToList()

                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las necesidades");
                throw;
            }
        }
    }
}
