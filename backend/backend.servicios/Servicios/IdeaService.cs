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
                Dificultad = ideaDto.Dificultad,
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

        public async Task<IEnumerable<IdeaDto>> GetIdeasByUsuarioIdAsync(int usuarioId)
        {
            try
            {
                var ideas = await _context.Ideas
                    .Include(i => i.Pasos)
                    .Where(i => i.UsuarioId == usuarioId)
                    .Select(i => new IdeaDto
                    {
                        Id = i.Id,
                        Titulo = i.Titulo,
                        UsuarioId = i.UsuarioId,
                        Dificultad = i.Dificultad,
                        Pasos = i.Pasos.Select(p => new PasoDto
                        {
                            Id = p.Id,
                            PasoNum = p.PasoNum,
                            Descripcion = p.Descripcion,
                            IdeaId = p.IdeaId
                        }).ToList()
                    }).ToListAsync();

                return ideas;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las ideas del usuario");
                throw;
            }
        }

        public async Task<IdeaDto> GetIdeaByIdAsync(int ideaId)
        {
            try
            {
                var idea = await _context.Ideas
                    .Include(i => i.Pasos)
                    .FirstOrDefaultAsync(i => i.Id == ideaId);

                if (idea == null)
                {
                    return null;
                }

                return new IdeaDto
                {
                    Titulo = idea.Titulo,
                    UsuarioId = idea.UsuarioId,
                    Dificultad = idea.Dificultad,
                    Pasos = idea.Pasos.Select(p => new PasoDto
                    {
                        PasoNum = p.PasoNum,
                        Descripcion = p.Descripcion
                    }).ToList()
                };

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la idea");
                throw;
            }
        }

        public async Task DeleteIdeaByIdAsync(int ideaId)
        {
            try
            {
                var idea = await _context.Ideas
                    .Include(i => i.Pasos)
                    .FirstOrDefaultAsync(i => i.Id == ideaId);

                if (idea == null)
                    throw new InvalidOperationException("La idea no existe.");

                _context.Pasos.RemoveRange(idea.Pasos);
                _context.Ideas.Remove(idea);

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al borrar la idea");
                throw;
            }
        }
    }
}
