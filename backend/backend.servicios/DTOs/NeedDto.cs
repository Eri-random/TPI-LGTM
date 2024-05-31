using backend.data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace backend.servicios.DTOs
{
    public class NeedDto
    {
        public int Id { get; set; }

        public string Nombre { get; set; }

        public string Icono { get; set; }

        public virtual ICollection<SubcategoriesDto> Subcategoria { get; set; } = new List<SubcategoriesDto>();
    }
}
