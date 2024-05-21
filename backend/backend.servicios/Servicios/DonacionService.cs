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
    public class DonacionService(ApplicationDbContext context, ILogger<DonacionService> logger) : IDonacionService
    {

        private readonly ApplicationDbContext _context = context ?? throw new ArgumentNullException(nameof(context));
        private readonly ILogger<DonacionService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public async Task SaveDonacionAsync(DonacionDto donacionDto)
        {
            if (donacionDto == null)
                throw new ArgumentNullException(nameof(donacionDto), "La donacion proporcionada no puede ser nulo.");

            var donacion = new Donacion
            {
               Producto = donacionDto.Producto,
               Cantidad = donacionDto.Cantidad,
               UsuarioId = donacionDto.UsuarioId,
               OrganizacionId = donacionDto.OrganizacionId
            };


            try
            {
                _context.Donacions.Add(donacion);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar una donacion");
                throw;
            }
        }

        public async Task<IEnumerable<DonacionDto>> GetDonacionesByOrganizacionIdAsync(int organizacionId)
        {
            try
            {
                var donaciones = await _context.Donacions
                    .Include(u => u.Usuario)
                    .Where(u => u.OrganizacionId == organizacionId)
                    .Select(u => new DonacionDto
                    {
                       Id  = u.Id,
                       Producto = u.Producto,
                       Cantidad = u.Cantidad,
                       Usuario = new UsuarioDto()
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

        public async Task<IEnumerable<DonacionDto>> GetDonacionesByUsuarioIdAsync(int usuarioId)
        {
            try
            {
                var donaciones = await _context.Donacions
                    .Include(u => u.Usuario)
                    .Where(u => u.UsuarioId == usuarioId)
                    .Select(u => new DonacionDto
                    {
                        Id = u.Id,
                        Producto = u.Producto,
                        Cantidad = u.Cantidad,
                        Usuario = new UsuarioDto()
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
