using backend.data.Models;
using backend.repositories.interfaces;
using backend.servicios.DTOs;
using backend.servicios.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace backend.servicios.Servicios
{
    public class OrganizationService(IRepository<Organizacion> orgRepository, IRepository<Subcategorium> subCatRepository, ILogger<OrganizationService> logger, IMapsService mapsService) : IOrganizationService
    {
        private readonly IRepository<Organizacion> _organizacionRepository = orgRepository ?? throw new ArgumentNullException(nameof(orgRepository));
        private readonly IRepository<Subcategorium> _subCatRepository = subCatRepository ?? throw new ArgumentNullException(nameof(orgRepository));
        private readonly ILogger<OrganizationService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IMapsService _mapsService = mapsService ?? throw new ArgumentNullException(nameof(mapsService));

        public async Task<IEnumerable<OrganizationDto>> GetAllOrganizationAsync()
        {
            try
            {
                var organizacion = await _organizacionRepository.GetAllAsync();
                return organizacion
                    .Select(u => new OrganizationDto
                    {
                        Id = u.Id,
                        Nombre = u.Nombre,
                        Cuit = u.Cuit,
                        Direccion = u.Direccion,
                        Localidad = u.Localidad,
                        Provincia = u.Provincia,
                        Telefono = u.Telefono,
                        Latitud = u.Latitud,
                        Longitud = u.Longitud,
                        InfoOrganizacion = u.InfoOrganizacion != null ? new InfoOrganizationDto
                        {
                            Organizacion = u.InfoOrganizacion.Organizacion,
                            DescripcionBreve = u.InfoOrganizacion.DescripcionBreve,
                            DescripcionCompleta = u.InfoOrganizacion.DescripcionCompleta,
                            Img = u.InfoOrganizacion.Img,
                            OrganizacionId = u.InfoOrganizacion.OrganizacionId
                        } : null
                    });
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

            var organization = new Organizacion
            {
                Nombre = organizationDto.Nombre,
                Cuit = organizationDto.Cuit,
                Direccion = organizationDto.Direccion,
                Localidad = organizationDto.Localidad,
                Provincia = organizationDto.Provincia,
                Telefono = organizationDto.Telefono,
            };

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
                var organizacion = await _organizacionRepository.GetByIdAsync(id);

                if (organizacion == null)
                {
                    return null;
                }

                return new OrganizationDto
                {
                    Id = organizacion.Id,
                    Nombre = organizacion.Nombre,
                    Cuit = organizacion.Cuit,
                    Direccion = organizacion.Direccion,
                    Localidad = organizacion.Localidad,
                    Provincia = organizacion.Provincia,
                    Telefono = organizacion.Telefono,
                    Latitud = organizacion.Latitud,
                    Longitud = organizacion.Longitud,
                    InfoOrganizacion = organizacion.InfoOrganizacion != null ? new InfoOrganizationDto
                    {
                        Organizacion = organizacion.InfoOrganizacion.Organizacion,
                        DescripcionBreve = organizacion.InfoOrganizacion.DescripcionBreve,
                        DescripcionCompleta = organizacion.InfoOrganizacion.DescripcionCompleta,
                        Img = organizacion.InfoOrganizacion.Img,
                        OrganizacionId = organizacion.InfoOrganizacion.OrganizacionId
                    } : null
                };
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
                var organizations = await _organizacionRepository.GetAllAsync();
                var organization = organizations.FirstOrDefault(x => x.Cuit == cuit);

                if (organization == null)
                    return null;

                return new OrganizationDto
                {
                    Id = organization.Id,
                    Nombre = organization.Nombre,
                    Cuit = organization.Cuit,
                    Direccion = organization.Direccion,
                    Localidad = organization.Localidad,
                    Provincia = organization.Provincia,
                    Telefono = organization.Telefono,
                    InfoOrganizacion = organization.InfoOrganizacion != null ? new InfoOrganizationDto
                    {
                        Organizacion = organization.InfoOrganizacion.Organizacion,
                        DescripcionBreve = organization.InfoOrganizacion.DescripcionBreve,
                        DescripcionCompleta = organization.InfoOrganizacion.DescripcionCompleta,
                        Img = organization.InfoOrganizacion.Img,
                        OrganizacionId = organization.InfoOrganizacion.OrganizacionId
                    } : null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la organizacion por cuit");
                throw;
            }
        }

        public async Task<IEnumerable<OrganizationDto>> GetPaginatedOrganizationsAsync(int page, int pageSize, List<int> subcategoriaIds,string name)
        {
            var organizations = await _organizacionRepository.GetAllAsync();
            var query = organizations
             .Where(o => o.InfoOrganizacion != null)
             .AsQueryable();

            if (subcategoriaIds != null && subcategoriaIds.Any())
                query = query.Where(o => o.Subcategoria.Any(s => subcategoriaIds.Contains(s.Id)));

            if (!string.IsNullOrEmpty(name))
                query = query.Where(o => o.Nombre.ToLower().Contains(name.ToLower()));

            var organizationsPaginated = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var organizationDtos = organizationsPaginated.Select(o => new OrganizationDto
            {
                Id = o.Id,
                Nombre = o.Nombre,
                Cuit = o.Cuit,
                Direccion = o.Direccion,
                Localidad = o.Localidad,
                Provincia = o.Provincia,
                Telefono = o.Telefono,
                Latitud = o.Latitud,
                Longitud = o.Longitud,
                InfoOrganizacion = o.InfoOrganizacion != null ? new InfoOrganizationDto
                {
                    Organizacion = o.InfoOrganizacion.Organizacion,
                    DescripcionBreve = o.InfoOrganizacion.DescripcionBreve,
                    DescripcionCompleta = o.InfoOrganizacion.DescripcionCompleta,
                    Img = o.InfoOrganizacion.Img,
                    OrganizacionId = o.InfoOrganizacion.OrganizacionId
                } : null
            });

            return organizationDtos;
        }
        
        public async Task AssignSubcategoriesAsync(int organizationId, List<SubcategoriesDto> subcategoriesDto)
        {
            try
            {
                var organization = await _organizacionRepository.GetByIdAsync(organizationId);

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
            var subCats = await _subCatRepository.GetAllAsync();
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
            var subCats = await _subCatRepository.GetAllAsync();
            var subcategories = subCats
                .Where(s => s.Organizacions.Any(o => o.Id == organizationId))
                .Select(s => new SubcategoriesDto
                {
                    Id = s.Id,
                    Nombre = s.Nombre,
                    NecesidadId = s.NecesidadId,
                    NecesidadNombre = s.Necesidad.Nombre,
                    NecesidadIcono = s.Necesidad.Icono
                })
                .ToList();

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
