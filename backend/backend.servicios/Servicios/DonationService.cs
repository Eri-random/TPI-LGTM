using AutoMapper;
using backend.data.Models;
using backend.repositories.interfaces;
using backend.servicios.DTOs;
using backend.servicios.Interfaces;
using Microsoft.Extensions.Logging;

namespace backend.servicios.Servicios
{
    public class DonationService(IRepository<Donacion> repository, ILogger<DonationService> logger, IMapper mapper) : IDonationService
    {
        private readonly IRepository<Donacion> _donacionRepository = repository ?? throw new ArgumentNullException(nameof(repository));
        private readonly ILogger<DonationService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        public async Task SaveDonationAsync(DonationDto donationDto)
        {
            if (donationDto == null)
                throw new ArgumentNullException(nameof(donationDto), "La donacion proporcionada no puede ser nula.");

            try
            {
                var donation = _mapper.Map<Donacion>(donationDto);
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
                var donaciones = await _donacionRepository.GetAllAsync(x => x.Usuario);
                var donationsFromOrg = donaciones.Where(x => x.OrganizacionId == organizacionId).OrderByDescending(x => x.Id);

                return _mapper.Map<IEnumerable<DonationDto>>(donationsFromOrg);
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
                var donaciones = await _donacionRepository.GetAllAsync(x => x.Usuario);
                var donationsFromUser = donaciones.Where(u => u.UsuarioId == usuarioId);

                return _mapper.Map<IEnumerable<DonationDto>>(donationsFromUser);
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

        public async Task<int> GetDonationIdAsync(DonationDto newDonation)
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
