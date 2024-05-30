using backend.data.Models;
using backend.servicios.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace backend.servicios.Interfaces
{
    public interface IOrganizacionService
    {
        Task<IEnumerable<OrganizacionDto>> GetAllOrganizacionAsync();

        Task SaveOrganizacionAsync(OrganizacionDto organizacionDto);

        Task<OrganizacionDto> GetOrganizacionByIdAsync(int id);

        Task<OrganizacionDto> GetOrganizacionByCuitAsync(string cuit);

        Task<IEnumerable<Organizacion>> GetOrganizacionesPaginadasAsync(int page, int pageSize);

        Task AsignarSubcategoriasAsync(int organizacionId, List<SubcategoriaDto> subcategoriasDto);

        Task<List<SubcategoriaDto>> GetSubcategoriasAsignadasAsync(int organizacionId);
    }
}
