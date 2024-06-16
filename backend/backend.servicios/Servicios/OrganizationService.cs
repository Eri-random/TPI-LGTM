using AutoMapper;
using backend.data.Models;
using backend.repositories.interfaces;
using backend.servicios.DTOs;
using backend.servicios.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace backend.servicios.Servicios
{
    public class OrganizationService(
        IRepository<Organizacion> orgRepository,
        IRepository<Subcategorium> subCatRepository,
        ILogger<OrganizationService> logger,
        IMapsService mapsService,
        IMapper mapper) : IOrganizationService
    {
        private readonly IRepository<Organizacion> _organizacionRepository = orgRepository ?? throw new ArgumentNullException(nameof(orgRepository));
        private readonly IRepository<Subcategorium> _subCatRepository = subCatRepository ?? throw new ArgumentNullException(nameof(orgRepository));
        private readonly ILogger<OrganizationService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IMapsService _mapsService = mapsService ?? throw new ArgumentNullException(nameof(mapsService));
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        public async Task<IEnumerable<OrganizationDto>> GetAllOrganizationAsync()
        {
            try
            {
                var organizacion = await _organizacionRepository.GetAllAsync(x => x.InfoOrganizacion);
                return _mapper.Map<IEnumerable<OrganizationDto>>(organizacion);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las organizaciones");
                throw;
            }
        }

        public async Task SaveOrganizationAsync(OrganizationDto organizationDto)
        {
            if (organizationDto == null)
                throw new ArgumentNullException(nameof(organizationDto), "La organizacion proporcionado no puede ser nula.");

            var organization = _mapper.Map<Organizacion>(organizationDto);
            try
            {
                await _organizacionRepository.AddAsync(organization);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear una nueva organizacion");
                throw;
            }
        }

        public async Task<OrganizationDto> GetOrganizationByIdAsync(int id)
        {
            try
            {
                var organizacion = await _organizacionRepository.GetByIdAsync(id, x => x.InfoOrganizacion);

                if (organizacion == null)
                {
                    return null;
                }

                return _mapper.Map<OrganizationDto>(organizacion);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la organizacion por id");
                throw;
            }
        }

        public async Task<OrganizationDto> GetOrganizationByCuitAsync(string cuit)
        {
            try
            {
                var organizations = await _organizacionRepository.GetAllAsync(x => x.InfoOrganizacion);
                var organization = organizations.FirstOrDefault(x => x.Cuit == cuit);

                if (organization == null)
                    return null;

                return _mapper.Map<OrganizationDto>(organization);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la organizacion por cuit");
                throw;
            }
        }

        public async Task<IEnumerable<OrganizationDto>> GetPaginatedOrganizationsAsync(int page, int pageSize, List<int> subcategoriaIds,string name)
        {
            var organizations = await _organizacionRepository.GetAllAsync(x => x.InfoOrganizacion);
            var query = organizations
                .Where(o => o.InfoOrganizacion != null)
                .AsQueryable();

            if (subcategoriaIds != null && subcategoriaIds.Any())
                query = query.Where(o => o.Subcategoria.Any(s => subcategoriaIds.Contains(s.Id)));

            if (!string.IsNullOrEmpty(name))
                query = query.Where(o => o.Nombre.ToLower().Contains(name.ToLower()));

            var totalCount = query.Count();

            // Apply pagination
            var organizationsPaginated = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var organizationDtos = _mapper.Map<IEnumerable<OrganizationDto>>(organizationsPaginated);

            return organizationDtos;
        }
        
        public async Task AssignSubcategoriesAsync(int organizationId, List<SubcategoriesDto> subcategoriesDto)
        {
            try
            {
                var organization = await _organizacionRepository.GetByIdAsync(organizationId, x => x.Subcategoria);

                if (organization == null)
                    throw new Exception("Organización no encontrada");

                organization.Subcategoria.Clear();

                var subCategories = await _subCatRepository.GetAllAsync();
                var newSubcategories = subCategories
                    .Where(s => subcategoriesDto.Select(dto => dto.Id).Contains(s.Id))
                    .ToList();

                foreach (var subcategory in newSubcategories)
                {
                    organization.Subcategoria.Add(subcategory);
                }

                await _organizacionRepository.UpdateAsync(organization);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al asignar subcategorías");
                throw;
            }
        }

        public async Task<List<SubcategoriesDto>> GetAssignedSubcategoriesAsync(int organizationId)
        {
            var subCats = await _subCatRepository.GetAllAsync(x => x.Organizacions);
            var subcategories = subCats
               .Where(s => s.Organizacions.Any(o => o.Id == organizationId))
               .Select(s => new SubcategoriesDto
               {
                   Id = s.Id,
                   Nombre = s.Nombre,
                   NecesidadId = s.NecesidadId
               })
               .ToList();

            return subcategories;
        }

        public async Task<List<NeedDto>> GetAssignedSubcategoriesGroupedAsync(int organizationId)
        {
            var subCats = await _subCatRepository.GetAllAsync(x => x.Organizacions);
            var subcategories = _mapper.Map<IEnumerable<SubcategoriesDto>>(subCats
                .Where(s => s.Organizacions.Any(o => o.Id == organizationId)));

            var groupedSubcategories = subcategories
                .GroupBy(s => new { s.NecesidadId, s.NecesidadNombre, s.NecesidadIcono })
                .Select(g => new NeedDto
                {
                    Id = g.Key.NecesidadId,
                    Nombre = g.Key.NecesidadNombre,
                    Icono = g.Key.NecesidadIcono,
                    Subcategoria = g.Select(sub => new SubcategoriesDto
                    {
                        Id = sub.Id,
                        Nombre = sub.Nombre,
                        NecesidadId = sub.NecesidadId
                    }).ToList()
                })
                .ToList();

            return groupedSubcategories;
        }

        public async Task UpdateOrganizationAsync(OrganizationDto organizationDto)
        {
            if (organizationDto == null)
                throw new ArgumentNullException(nameof(organizationDto), "La organizacion proporcionado no puede ser nula.");
            
            var organization = await _organizacionRepository.GetByIdAsync(organizationDto.Id);

            if (organization == null)
                throw new Exception("Organización no encontrada");

            var (lat, lng) = await _mapsService.GetCoordinates(organizationDto.Direccion, organizationDto.Localidad, organizationDto.Provincia);

            try
            {
                organization.Nombre = organizationDto.Nombre;
                organization.Cuit = organizationDto.Cuit;
                organization.Direccion = organizationDto.Direccion;
                organization.Localidad = organizationDto.Localidad;
                organization.Provincia = organizationDto.Provincia;
                organization.Telefono = organizationDto.Telefono;
                organization.Latitud = lat;
                organization.Longitud = lng;

                await _organizacionRepository.UpdateAsync(organization);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la organizacion");
                throw;
            }
        }
    }
}
