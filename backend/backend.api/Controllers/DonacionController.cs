﻿using backend.api.Models;
using backend.servicios.DTOs;
using backend.servicios.Interfaces;
using backend.servicios.Servicios;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DonacionController(ILogger<DonacionController> logger, IDonacionService donacionService) : ControllerBase
    {
        private readonly ILogger<DonacionController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IDonacionService _donacionService = donacionService ?? throw new ArgumentNullException(nameof(donacionService));

        [HttpPost]
        public async Task<IActionResult> SaveDonacion([FromBody] DonacionRequestModel donacionRequest)
        {
            if (donacionRequest == null)
            {
                return BadRequest("Datos de donacion inválidos");
            }

            var nuevaDonacion = new DonacionDto
            {
                Producto = donacionRequest.Producto,
                Cantidad = donacionRequest.Cantidad,
                UsuarioId = donacionRequest.UsuarioId,
                OrganizacionId = donacionRequest.OrganizacionId
            };

            try
            {
                await _donacionService.SaveDonacionAsync(nuevaDonacion);
                return CreatedAtAction(nameof(SaveDonacion), new { organizacionId = donacionRequest.OrganizacionId }, donacionRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar donacion");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("user/{usuarioId}")]
        public async Task<IActionResult> GetDonacionesByUsuarioId(int usuarioId)
        {
            try
            {
                var ideas = await _donacionService.GetDonacionesByUsuarioIdAsync(usuarioId);
                return Ok(ideas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las donaciones del usuario");
                return StatusCode(500, $"Error al obtener las donaciones del usuario");
            }
        }

        [HttpGet("organizacion/{organizacionId}")]
        public async Task<IActionResult> GetDonacionesByOrganizacionId(int organizacionId)
        {
            try
            {
                var ideas = await _donacionService.GetDonacionesByOrganizacionIdAsync(organizacionId);
                return Ok(ideas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las donaciones de la organizacion");
                return StatusCode(500, $"Error al obtener las donaciones de la organizacion");
            }
        }
    }
}
