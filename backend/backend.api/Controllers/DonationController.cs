﻿using backend.api.Models;
using backend.servicios.DTOs;
using backend.servicios.Interfaces;
using backend.servicios.Servicios;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace backend.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DonationController(ILogger<DonationController> logger, IUserService userService, IDonationService donationService) : ControllerBase
    {
        private readonly ILogger<DonationController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IDonationService _donationService = donationService ?? throw new ArgumentNullException(nameof(donationService));
        private readonly IUserService _userService = userService ?? throw new ArgumentNullException(nameof(userService));

        [HttpPost]
        public async Task<IActionResult> SaveDonacion([FromBody] DonationRequestModel donationRequest)
        {
            if (donationRequest == null)
            {
                return BadRequest("Datos de donacion inválidos");
            }

            var newDonation = new DonationDto
            {
                Id = donationRequest.Id,
                Producto = donationRequest.Producto,
                Cantidad = donationRequest.Cantidad,
                Estado = donationRequest.Estado,
                UsuarioId = donationRequest.UsuarioId,
                OrganizacionId = donationRequest.OrganizacionId,
                Cuit = donationRequest.Cuit
            };

            try
            {
                await _donationService.SaveDonationAsync(newDonation);

                newDonation.Id = await _donationService.GetDonationIdAsync(newDonation);

                var user = await _userService.GetUserByIdAsync(donationRequest.UsuarioId);

                var response = new
                {
                    newDonation,
                    user
                };
                // Notificar a los clientes conectados
                await WebSocketHandler.NotifyNewDonationAsync(response);

                return CreatedAtAction(nameof(SaveDonacion), new { organizacionId = donationRequest.OrganizacionId }, donationRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar donacion");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("user/{usuarioId}")]
        public async Task<IActionResult> GetDonationsByUserId(int usuarioId)
        {
            try
            {
                var ideas = await _donationService.GetDonationsByUserIdAsync(usuarioId);
                return Ok(ideas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las donaciones del user");
                return StatusCode(500, $"Error al obtener las donaciones del user");
            }
        }

        [HttpGet("organization/{organizacionId}")]
        public async Task<IActionResult> GetDonationsByOrganizationId(int organizacionId)
        {
            try
            {
                var ideas = await _donationService.GetDonationsByOrganizationIdAsync(organizacionId);
                return Ok(ideas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las donaciones de la organizacion");
                return StatusCode(500, $"Error al obtener las donaciones de la organizacion");
            }
        }
        [HttpPut("updateState")]
        public async Task<IActionResult> UpdateDonationsState([FromBody] UpdateDonationsStateRequest request)
        {
            try
            {
                await _donationService.UpdateDonationsStateAsync(request.DonationIds, request.State);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el estado de las donaciones");
                return StatusCode(500, "Internal server error");
            }
        }

    }
}
