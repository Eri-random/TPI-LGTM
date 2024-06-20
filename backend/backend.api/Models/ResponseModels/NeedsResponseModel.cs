using backend.servicios.DTOs;

namespace backend.api.Models.ResponseModels
{
    public class NeedsResponseModel
    {
        public int Id { get; set; }

        public string Nombre { get; set; }

        public string Icono { get; set; }

        public virtual ICollection<SubcategoriesDto> Subcategoria { get; set; } = new List<SubcategoriesDto>();
    }
}
