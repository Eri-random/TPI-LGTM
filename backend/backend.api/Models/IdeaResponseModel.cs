using backend.data.Models;
using backend.servicios.DTOs;

namespace backend.api.Models
{
    public class IdeaResponseModel
    {
        public string Titulo { get; set; }
        public int UsuarioId { get; set; }
        public virtual ICollection<PasoDto> Pasos { get; set; } = new List<PasoDto>();
    }
}
