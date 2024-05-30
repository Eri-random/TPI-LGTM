using backend.servicios.DTOs;

namespace backend.api.Models
{
    public class NecesidadResponseModel
    {
        public int Id { get; set; }

        public string Nombre { get; set; }

        public string Icono { get; set; }

        public virtual ICollection<SubcategoriaDto> Subcategoria { get; set; } = new List<SubcategoriaDto>();
    }
}
