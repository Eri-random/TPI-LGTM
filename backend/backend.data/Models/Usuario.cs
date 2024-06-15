namespace backend.data.Models;

public partial class Usuario
{
    public int Id { get; set; }

    public string Nombre { get; set; }

    public string Apellido { get; set; }

    public string Email { get; set; }

    public string Contrasena { get; set; }

    public string Telefono { get; set; }

    public string Direccion { get; set; }

    public string Localidad { get; set; }

    public string Provincia { get; set; }

    public int RolId { get; set; }

    public virtual ICollection<Donacion> Donacions { get; set; } = new List<Donacion>();

    public virtual ICollection<Idea> Ideas { get; set; } = new List<Idea>();

    public virtual Organizacion Organizacion { get; set; }

    public virtual Rol Rol { get; set; }
}
