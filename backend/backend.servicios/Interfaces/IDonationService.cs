using backend.data.Models;
using backend.servicios.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace backend.servicios.Interfaces
{
    public interface IDonationService
    {
        Task SaveDonationAsync(DonationDto donationDto);

        Task<IEnumerable<DonationDto>> GetDonationsByUserIdAsync(int usuarioId);

        Task<IEnumerable<DonationDto>> GetDonationsByOrganizationIdAsync(int organizacionId);

        Task UpdateDonationsStateAsync(List<int> donationIds, string state);

        Task<int> GetIdDonationAsync(DonationDto newDonation);
    }
}
