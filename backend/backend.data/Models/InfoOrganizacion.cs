using System;
using System.Collections.Generic;

namespace backend.data.Models;

public partial class InfoOrganizacion
{
    public int Id { get; set; }

    public string Organizacion { get; set; }

    public string DescripcionBreve { get; set; }

    public string DescripcionCompleta { get; set; }

    public string Img { get; set; }

    public int OrganizacionId { get; set; }

    public virtual Organizacion OrganizacionNavigation { get; set; }
}
