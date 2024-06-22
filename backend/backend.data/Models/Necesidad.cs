namespace backend.data.Models;

public partial class Necesidad
{
    public int Id { get; set; }

    public string Nombre { get; set; }

    public string Icono { get; set; }

    public int? CampaignId { get; set; }

    public virtual Campaign Campaign { get; set; }

    public virtual ICollection<Subcategorium> Subcategoria { get; set; } = new List<Subcategorium>();
}
