﻿using backend.data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace backend.servicios.DTOs
{
    public class SubcategoriesDto
    {
        public int Id { get; set; }

        public string Nombre { get; set; }

        public int NecesidadId { get; set; }

        public string NecesidadNombre { get; set; }

        public string NecesidadIcono { get; set; }

    }
}
