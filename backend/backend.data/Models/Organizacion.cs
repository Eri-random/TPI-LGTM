using System;
using System.Collections.Generic;

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

    public virtual InfoOrganizacion InfoOrganizacion { get; set; }

    public virtual Usuario Usuario { get; set; }
}
