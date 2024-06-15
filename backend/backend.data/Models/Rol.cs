namespace backend.data.Models;

public partial class Rol
{
    public int RolId { get; set; }

    public string Nombre { get; set; }

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
