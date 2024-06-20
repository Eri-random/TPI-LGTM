using backend.servicios.DTOs;

namespace backend.servicios.Interfaces
{
    public interface IHeadquartersService
    {
        Task CreateHeadquartersAsync(List<HeadquartersDto> headquartersDtos);

        Task<IEnumerable<HeadquartersDto>> GetAllHeadquartersAsync();

        Task<IEnumerable<HeadquartersDto>> GetHeadquartersByOrganizationIdAsync(int organizationId);

        Task UpdateHeadquartersAsync(HeadquartersDto headquartersDto);

        Task DeleteHeadquartersAsync(int headquartersId);

        Task<HeadquartersDto> GetHeadquarterByIdAsync(int headquartersId);
    }
}
