﻿using backend.servicios.DTOs;

namespace backend.api.Models
{
    public class DonacionRequestModel
    {

        public string Producto { get; set; }

        public int Cantidad { get; set; }

        public int UsuarioId { get; set; }

        public int OrganizacionId { get; set; }

    }
}
