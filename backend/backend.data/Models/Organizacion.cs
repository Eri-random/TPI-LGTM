namespace backend.data.Models;

public partial class Organizacion
{
    public int Id { get; set; }

    public string Nombre { get; set; }

    public string Cuit { get; set; }

    public string Telefono { get; set; }

    public string Direccion { get; set; }

    public string Localidad { get; set; }

    public string Provincia { get; set; }

    public int UsuarioId { get; set; }

    public double Latitud { get; set; }

    public double Longitud { get; set; }

    public virtual ICollection<Donacion> Donacions { get; set; } = new List<Donacion>();

    public virtual InfoOrganizacion InfoOrganizacion { get; set; }

    public virtual ICollection<Sede> Sedes { get; set; } = new List<Sede>();

    public virtual Usuario Usuario { get; set; }

    public virtual ICollection<Subcategorium> Subcategoria { get; set; } = new List<Subcategorium>();
}
