using backend.data.DataContext;
using backend.data.Models;
using backend.servicios.DTOs;
using backend.servicios.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace backend.servicios.Servicios
{
    public class IdeaService(ApplicationDbContext context, ILogger<IdeaService> logger) : IIdeaService
    {
        private readonly ApplicationDbContext _context = context ?? throw new ArgumentNullException(nameof(context));
        private readonly ILogger<IdeaService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public async Task SaveIdeaAsync(IdeaDto ideaDto)
        {
            if (ideaDto == null)
                throw new ArgumentNullException(nameof(ideaDto), "La idea proporcionada no puede ser nula.");

            var idea = new Idea
            {
                Titulo = ideaDto.Titulo,
                UsuarioId = ideaDto.UsuarioId,
                Pasos = ideaDto.Pasos.Select(paso => new Paso
                {
                    PasoNum = paso.PasoNum,
                    Descripcion = paso.Descripcion
                }).ToList()
            };

            try
            {
                _context.Add(idea);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar la idea");
                throw;
            }
        }
    }
}
