using backend.data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace backend.servicios.DTOs
{
    public class StepDto
    {
        public int Id { get; set; }

        public int PasoNum { get; set; }

        public string Descripcion { get; set; }

        public string ImagenUrl { get; set; }
        public int IdeaId { get; set; }

    }
}
