using backend.data.Models;
using backend.servicios.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace backend.servicios.Interfaces
{
    public interface IOrganizationService
    {
        Task<IEnumerable<OrganizationDto>> GetAllOrganizationAsync();

        Task SaveOrganizationAsync(OrganizationDto organizationDto);

        Task<OrganizationDto> GetOrganizationByIdAsync(int id);

        Task<OrganizationDto> GetOrganizationByCuitAsync(string cuit);

        Task<IEnumerable<Organizacion>> GetPaginatedOrganizationsAsync(int page, int pageSize);

        Task AssignSubcategoriesAsync(int organizationId, List<SubcategoriesDto> subcategoriesDto);

        Task<List<SubcategoriesDto>> GetAssignedSubcategoriesAsync(int organizationId);

        Task<List<NeedDto>> GetAssignedSubcategoriesGroupedAsync(int organizationId);
    }
}
