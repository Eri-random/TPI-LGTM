using backend.data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace backend.servicios.DTOs
{
    public class DonacionDto
    {
        public int Id { get; set; }

        public string Producto { get; set; }

        public int Cantidad { get; set; }

        public int UsuarioId { get; set; }

        public int OrganizacionId { get; set; }

        public virtual OrganizacionDto Organizacion { get; set; }

        public virtual UsuarioDto Usuario { get; set; }
    }
}
