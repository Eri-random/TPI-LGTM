﻿using backend.api.Models;
using backend.servicios.DTOs;
using backend.servicios.Interfaces;
using backend.servicios.Servicios;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace backend.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizacionController(IOrganizacionService organizacionService, ILogger<UsuariosController> logger) : ControllerBase
    {
        private readonly IOrganizacionService _organizacionService = organizacionService ?? throw new ArgumentNullException(nameof(organizacionService));
        private readonly ILogger<UsuariosController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        [HttpGet]
        public async Task<IActionResult> GetAllUsuarios()
        {
            try
            {
                var organizaciones = await _organizacionService.GetAllOrganizacionAsync();

                var organizacionResponse = new List<OrganizacionResponseModel>();

                foreach (var organizacion in organizaciones)
                {
                    organizacionResponse.Add(new OrganizacionResponseModel
                    {
                        Nombre = organizacion.Nombre,
                        Cuit = organizacion.Cuit,
                        Direccion = organizacion.Direccion,
                        Localidad = organizacion.Localidad,
                        Provincia = organizacion.Provincia,
                        Telefono = organizacion.Telefono,
                    });
                }

                return Ok(organizacionResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las organizaciones");
                return StatusCode(500, "Internal server error");
            }
        }

    }
}