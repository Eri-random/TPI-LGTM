using backend.data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace backend.servicios.DTOs
{
    public class NecesidadDto
    {
        public int Id { get; set; }

        public string Nombre { get; set; }

        public string Icono { get; set; }

        public virtual ICollection<SubcategoriaDto> Subcategoria { get; set; } = new List<SubcategoriaDto>();
    }
}
