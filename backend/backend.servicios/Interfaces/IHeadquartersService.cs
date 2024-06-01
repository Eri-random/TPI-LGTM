using backend.servicios.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace backend.servicios.Interfaces
{
    public interface IHeadquartersService
    {
        Task createHeadquartersAsync(List<HeadquartersDto> headquartersDtos);

        Task<IEnumerable<HeadquartersDto>> GetAllHeadquartersAsync();

        Task<IEnumerable<HeadquartersDto>> GetHeadquartersByOrganizationIdAsync(int organizationId);

        Task updateHeadquartersAsync(HeadquartersDto headquartersDto);

        Task deleteHeadquartersAsync(int headquartersId);

        Task<HeadquartersDto> GetHeadquarterByIdAsync(int headquartersId);

        double CalculateDistance(double lat1, double lon1, double lat2, double lon2);
    }
}
