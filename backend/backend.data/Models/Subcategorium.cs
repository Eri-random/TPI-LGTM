namespace backend.data.Models;

public partial class Subcategorium
{
    public int Id { get; set; }

    public string Nombre { get; set; }

    public int NecesidadId { get; set; }

    public virtual Necesidad Necesidad { get; set; }

    public virtual ICollection<Organizacion> Organizacions { get; set; } = new List<Organizacion>();
}
