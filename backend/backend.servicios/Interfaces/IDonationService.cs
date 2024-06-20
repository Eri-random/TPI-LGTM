using backend.servicios.DTOs;

namespace backend.servicios.Interfaces
{
    public interface IDonationService
    {
        Task SaveDonationAsync(DonationDto donationDto);

        Task<IEnumerable<DonationDto>> GetDonationsByUserIdAsync(int usuarioId);

        Task<IEnumerable<DonationDto>> GetDonationsByOrganizationIdAsync(int organizacionId);

        Task UpdateDonationsStateAsync(List<int> donationIds, string state);

        Task<int> GetDonationIdAsync(DonationDto newDonation);
    }
}
