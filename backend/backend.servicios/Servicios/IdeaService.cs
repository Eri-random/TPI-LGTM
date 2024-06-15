using AutoMapper;
using backend.data.Models;
using backend.repositories.interfaces;
using backend.servicios.DTOs;
using backend.servicios.Interfaces;
using Microsoft.Extensions.Logging;

namespace backend.servicios.Servicios
{
    public class IdeaService(IRepository<Idea> ideaRepository, IRepository<Paso> pasoRepository, ILogger<IdeaService> logger, IMapper mapper) : IIdeaService
    {
        private readonly IRepository<Idea> _ideaRepository = ideaRepository ?? throw new ArgumentNullException(nameof(ideaRepository));
        private readonly IRepository<Paso> _pasoRepository = pasoRepository ?? throw new ArgumentNullException(nameof(pasoRepository));
        private readonly ILogger<IdeaService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        public async Task SaveIdeaAsync(IdeaDto ideaDto)
        {
            if (ideaDto == null)
                throw new ArgumentNullException(nameof(ideaDto), "La idea proporcionada no puede ser nula.");

            try
            {
                var idea = _mapper.Map<Idea>(ideaDto);
                idea.Pasos = _mapper.Map<List<Paso>>(ideaDto.Pasos);
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
                var ideas = await _ideaRepository.GetAllAsync(x => x.Pasos);
                var ideasFromUser = ideas.Where(x => x.UsuarioId == userId);

                return _mapper.Map<IEnumerable<IdeaDto>>(ideasFromUser);
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
                var ideas = await _ideaRepository.GetAllAsync(x => x.Pasos);
                var idea = ideas.FirstOrDefault(x => x.Id == ideaId);

                if (idea == null)
                    return null;

                return _mapper.Map<IdeaDto>(idea);
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
                var ideas = await _ideaRepository.GetAllAsync(x => x.Pasos);
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
