using backend.data.Models;
using backend.servicios.DTOs;

namespace backend.api.Models
{
    public class IdeaResponseModel
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public int UsuarioId { get; set; }
        public string Dificultad { get; set; }
        public virtual ICollection<StepDto> Pasos { get; set; } = new List<StepDto>();
    }
}
