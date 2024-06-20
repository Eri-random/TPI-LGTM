using backend.servicios.DTOs;

namespace backend.servicios.Interfaces
{
    public interface IOrganizationInfoService
    {
        Task SaveInfoOrganizationDataAsync(InfoOrganizationDto infoOrganizationDto);

        Task UpdateInfoOrganizationAsync(InfoOrganizationDto infoOrganizationDto);

    }
}
