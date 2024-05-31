using backend.data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace backend.servicios.DTOs
{
    public class IdeaDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public int UsuarioId { get; set; }
        public string Dificultad { get; set; }
        public virtual ICollection<StepDto> Pasos { get; set; } = new List<StepDto>();
    }
}
