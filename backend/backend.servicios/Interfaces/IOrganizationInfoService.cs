using backend.servicios.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace backend.servicios.Interfaces
{
    public interface IOrganizationInfoService
    {
        Task SaveDataInfoOrganization(InfoOrganizationDto infoOrganizationDto);

        Task UpdateInfoOrganizationAsync(InfoOrganizationDto infoOrganizationDto);
    }
}
