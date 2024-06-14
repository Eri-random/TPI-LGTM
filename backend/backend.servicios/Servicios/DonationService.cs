using backend.data.Models;
using backend.repositories.interfaces;
using backend.servicios.DTOs;
using backend.servicios.Interfaces;
using Microsoft.Extensions.Logging;

namespace backend.servicios.Servicios
{
    public class DonationService(IRepository<Donacion> repository, ILogger<DonationService> logger) : IDonationService
    {
        private readonly IRepository<Donacion> _donacionRepository = repository ?? throw new ArgumentNullException(nameof(repository));
        private readonly ILogger<DonationService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public async Task SaveDonationAsync(DonationDto donationDto)
        {
            if (donationDto == null)
                throw new ArgumentNullException(nameof(donationDto), "La donacion proporcionada no puede ser nula.");

            var donation = new Donacion
            {
                Producto = donationDto.Producto,
                Cantidad = donationDto.Cantidad,
                Estado = donationDto.Estado,
                UsuarioId = donationDto.UsuarioId,
                OrganizacionId = donationDto.OrganizacionId
            };

            try
            {
                await _donacionRepository.AddAsync(donation);
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
                var donaciones = await _donacionRepository.GetAllAsync();

                return donaciones.Where(x => x.OrganizacionId == organizacionId)
                    .OrderByDescending(x => x.Id)
                    .Select(u => new DonationDto
                    {
                        Id = u.Id,
                        Producto = u.Producto,
                        Cantidad = u.Cantidad,
                        Estado = u.Estado,
                        Usuario = new UserDto()
                        {
                            Nombre = u.Usuario.Nombre,
                            Telefono = u.Usuario.Telefono,
                            Email = u.Usuario.Email
                        }
                    }).ToList();
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
                var donaciones = await _donacionRepository.GetAllAsync();

                return donaciones.Where(u => u.UsuarioId == usuarioId)
                    .Select(u => new DonationDto
                    {
                        Id = u.Id,
                        Producto = u.Producto,
                        Cantidad = u.Cantidad,
                        Estado = u.Estado,
                        OrganizacionId = u.OrganizacionId,
                        Usuario = new UserDto()
                        {
                            Nombre = u.Usuario.Nombre,
                            Telefono = u.Usuario.Telefono,
                            Email = u.Usuario.Email
                        }

                    }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las donaciones del usuario");
                throw;
            }
        }

        public async Task UpdateDonationsStateAsync(List<int> donationIds, string state)
        {
            var allDonations = await _donacionRepository.GetAllAsync();
            var donations = allDonations.Where(x => donationIds.Contains(x.Id));

            if (donations == null || !donations.Any())
            {
                throw new InvalidOperationException("Las donaciones no existen.");
            }

            foreach (var donation in donations)
            {
                donation.Estado = state;
            }

            try
            {
                await _donacionRepository.UpdateRangeAsync(donations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el estado de las donaciones");
                throw;
            }
        }

        public async Task<int> GetIdDonationAsync(DonationDto newDonation)
        {
            var donations = await _donacionRepository.GetAllAsync();
            var donation = donations.FirstOrDefault(d => d.Producto.Equals(newDonation.Producto)
                && d.Cantidad == newDonation.Cantidad
                && d.UsuarioId == newDonation.UsuarioId
                && d.OrganizacionId == newDonation.OrganizacionId
                && d.Estado.Equals(newDonation.Estado));

            return donation?.Id ?? 0;
        }
    }
}
