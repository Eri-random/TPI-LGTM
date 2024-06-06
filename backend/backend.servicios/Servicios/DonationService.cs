using backend.data.DataContext;
using backend.data.Models;
using backend.servicios.DTOs;
using backend.servicios.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace backend.servicios.Servicios
{
    public class DonationService(ApplicationDbContext context, ILogger<DonationService> logger) : IDonationService
    {

        private readonly ApplicationDbContext _context = context ?? throw new ArgumentNullException(nameof(context));
        private readonly ILogger<DonationService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public async Task SaveDonationAsync(DonationDto donationDto)
        {
            if (donationDto == null)
                throw new ArgumentNullException(nameof(donationDto), "La donacion proporcionada no puede ser nulo.");

            var donation = new Donacion
            {
               Producto = donationDto.Producto,
               Cantidad = donationDto.Cantidad,
               UsuarioId = donationDto.UsuarioId,
               OrganizacionId = donationDto.OrganizacionId
            };


            try
            {
                _context.Donacions.Add(donation);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar una donacion");
                throw;
            }
        }

        public async Task<IEnumerable<DonationDto>> GetDonationsByOrganizationIdAsync(int organizacionId)
        {
            try
            {
                var donaciones = await _context.Donacions
                    .Include(u => u.Usuario)
                    .Where(u => u.OrganizacionId == organizacionId)
                    .OrderByDescending(u => u.Id) // Ordenar por Id descendente
                    .Select(u => new DonationDto
                    {
                        Id = u.Id,
                        Producto = u.Producto,
                        Cantidad = u.Cantidad,
                        Usuario = new UserDto()
                        {
                            Nombre = u.Usuario.Nombre,
                            Telefono = u.Usuario.Telefono,
                            Email = u.Usuario.Email
                        }
                    }).ToListAsync();

                return donaciones;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las donaciones de la organizacion");
                throw;
            }
        }

        public async Task<IEnumerable<DonationDto>> GetDonationsByUserIdAsync(int usuarioId)
        {
            try
            {
                var donaciones = await _context.Donacions
                    .Include(u => u.Usuario)
                    .Where(u => u.UsuarioId == usuarioId)
                    .Select(u => new DonationDto
                    {
                        Id = u.Id,
                        Producto = u.Producto,
                        Cantidad = u.Cantidad,
                        OrganizacionId = u.OrganizacionId,
                        Usuario = new UserDto()
                        {
                            Nombre = u.Usuario.Nombre,
                            Telefono = u.Usuario.Telefono,
                            Email = u.Usuario.Email
                        }

                    }).ToListAsync();

                return donaciones;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las donaciones del usuario");
                throw;
            }
        }
    }
}
