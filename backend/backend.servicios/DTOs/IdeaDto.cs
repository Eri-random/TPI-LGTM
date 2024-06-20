namespace backend.servicios.DTOs
{
    public class IdeaDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public int UsuarioId { get; set; }
        public string Dificultad { get; set; }
        public virtual ICollection<StepDto> Pasos { get; set; } = new List<StepDto>();
        public string ImageUrl { get; set; }
    }
}
