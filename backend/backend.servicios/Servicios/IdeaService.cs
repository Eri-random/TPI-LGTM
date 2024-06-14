using backend.data.Models;
using backend.repositories.interfaces;
using backend.servicios.DTOs;
using backend.servicios.Interfaces;
using Microsoft.Extensions.Logging;

namespace backend.servicios.Servicios
{
    public class IdeaService(IRepository<Idea> ideaRepository, IRepository<Paso> pasoRepository, ILogger<IdeaService> logger) : IIdeaService
    {
        private readonly IRepository<Idea> _ideaRepository = ideaRepository ?? throw new ArgumentNullException(nameof(ideaRepository));
        private readonly IRepository<Paso> _pasoRepository = pasoRepository ?? throw new ArgumentNullException(nameof(pasoRepository));
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
                    Descripcion = paso.Descripcion,
                    ImagenUrl = paso.ImagenUrl
                }).ToList(),
                ImageUrl = ideaDto.ImageUrl
            };

            try
            {
                await _ideaRepository.AddAsync(idea);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar la idea");
                throw;
            }
        }

        public async Task<IEnumerable<IdeaDto>> GetIdeasByUserIdAsync(int userId)
        {
            try
            {
                var ideas = await _ideaRepository.GetAllAsync();
                return ideas.Where(x => x.UsuarioId == userId)
                    .Select(y => new IdeaDto
                    {
                        Id = y.Id,
                        Titulo = y.Titulo,
                        UsuarioId = y.UsuarioId,
                        Dificultad = y.Dificultad,
                        Pasos = y.Pasos.Select(p => new StepDto
                        {
                            Id = p.Id,
                            PasoNum = p.PasoNum,
                            Descripcion = p.Descripcion,
                            IdeaId = p.IdeaId
                        }).ToList(),
                       ImageUrl = y.ImageUrl

                    });
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
                var ideas = await _ideaRepository.GetAllAsync();
                var idea = ideas.FirstOrDefault(x => x.Id == ideaId);

                if (idea == null)
                    return null;

                return new IdeaDto
                {
                    Titulo = idea.Titulo,
                    UsuarioId = idea.UsuarioId,
                    Dificultad = idea.Dificultad,
                    Pasos = idea.Pasos.Select(p => new StepDto
                    {
                        PasoNum = p.PasoNum,
                        Descripcion = p.Descripcion,
                        ImagenUrl = p.ImagenUrl
                    }).ToList(),
                    ImageUrl = idea.ImageUrl
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
                var ideas = await _ideaRepository.GetAllAsync();
                var idea = ideas.FirstOrDefault(i => i.Id == ideaId);

                if (idea == null)
                    throw new InvalidOperationException("La idea no existe.");

                await _pasoRepository.DeleteRangeAsync(idea.Pasos);
                await _ideaRepository.DeleteAsync(idea.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al borrar la idea");
                throw;
            }
        }
    }
}
