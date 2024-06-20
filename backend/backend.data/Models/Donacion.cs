namespace backend.data.Models;

public partial class Donacion
{
    public int Id { get; set; }

    public string Producto { get; set; }

    public int Cantidad { get; set; }

    public int UsuarioId { get; set; }

    public int OrganizacionId { get; set; }

    public string Estado { get; set; }

    public virtual Organizacion Organizacion { get; set; }

    public virtual Usuario Usuario { get; set; }
}
