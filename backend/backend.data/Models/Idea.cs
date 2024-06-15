namespace backend.data.Models;

public partial class Idea
{
    public int Id { get; set; }

    public string Titulo { get; set; }

    public int UsuarioId { get; set; }

    public string Dificultad { get; set; }

    public string ImageUrl { get; set; }

    public virtual ICollection<Paso> Pasos { get; set; } = new List<Paso>();

    public virtual Usuario Usuario { get; set; }
}
