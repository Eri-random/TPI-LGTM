using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace backend.servicios.DTOs
{
    public class InfoOrganizationDto
    {
        public string Organizacion { get; set; }

        public string DescripcionBreve { get; set; }

        public string DescripcionCompleta { get; set; }

        public string Img { get; set; }

        public int OrganizacionId { get; set; }
    }
}
