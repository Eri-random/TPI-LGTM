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
