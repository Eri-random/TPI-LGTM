using System;
using System.Collections.Generic;

namespace backend.data.Models;

public partial class Paso
{
    public int Id { get; set; }

    public int PasoNum { get; set; }

    public string Descripcion { get; set; }

    public int IdeaId { get; set; }

    public string ImagenUrl { get; set; }

    public virtual Idea Idea { get; set; }
}
