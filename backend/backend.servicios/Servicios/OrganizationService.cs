using backend.data.DataContext;
using backend.data.Models;
using backend.servicios.DTOs;
using backend.servicios.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace backend.servicios.Servicios
{
    public class OrganizationService(ApplicationDbContext context, ILogger<OrganizationService> logger) : IOrganizationService
    {
        private readonly ApplicationDbContext _context = context ?? throw new ArgumentNullException(nameof(context));
        private readonly ILogger<OrganizationService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        public async Task<IEnumerable<OrganizationDto>> GetAllOrganizationAsync()
        {
            try
            {
                var organization = await _context.Organizacions.Include(u => u.InfoOrganizacion)
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
                    }).ToListAsync();

                return organization;

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
                _context.Organizacions.Add(organization);
                await _context.SaveChangesAsync();

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
                var organization = await _context.Organizacions.Include(x => x.InfoOrganizacion).FirstOrDefaultAsync(u => u.Id == id);

                if (organization == null)
                {
                    return null;
                }

                return new OrganizationDto
                {
                    Id = organization.Id,
                    Nombre = organization.Nombre,
                    Cuit = organization.Cuit,
                    Direccion = organization.Direccion,
                    Localidad = organization.Localidad,
                    Provincia = organization.Provincia,
                    Telefono = organization.Telefono,
                    Latitud = organization.Latitud,
                    Longitud = organization.Longitud,
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
                _logger.LogError(ex, "Error al obtener la organizacion por id");
                throw;
            }
        }

        public async Task<OrganizationDto> GetOrganizationByCuitAsync(string cuit)
        {
            try
            {
                var organization = await _context.Organizacions.Include(u => u.InfoOrganizacion).FirstOrDefaultAsync(u => u.Cuit == cuit);

                if (organization == null)
                {
                    return null;
                }

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

        public async Task<IEnumerable<OrganizationDto>> GetPaginatedOrganizationsAsync(int page, int pageSize, List<int> subcategoriaIds)
        {
            var query = _context.Organizacions
           .Include(o => o.InfoOrganizacion)
           .Where(o => o.InfoOrganizacion != null)
           .AsQueryable();

            if (subcategoriaIds != null && subcategoriaIds.Any())
            {
                query = query.Where(o => o.Subcategoria.Any(s => subcategoriaIds.Contains(s.Id)));
            }

             var organizations = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

            var organizationDtos = organizations.Select(o => new OrganizationDto
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
            }).ToList();           

            return organizationDtos;
        }
        
        public async Task AssignSubcategoriesAsync(int organizationId, List<SubcategoriesDto> subcategoriesDto)
        {
            try
            {
                var organization = await _context.Organizacions
                    .Include(o => o.Subcategoria)
                    .FirstOrDefaultAsync(o => o.Id == organizationId);

                if (organization == null)
                {
                    throw new Exception("Organización no encontrada");
                }

                // Eliminar las relaciones existentes
                organization.Subcategoria.Clear();

                // Obtener las nuevas subcategorías desde la base de datos
                var newSubcategories = await _context.Subcategoria
                    .Where(s => subcategoriesDto.Select(dto => dto.Id).Contains(s.Id))
                    .ToListAsync();

                // Asignar las nuevas subcategorías
                foreach (var subcategory in newSubcategories)
                {
                    organization.Subcategoria.Add(subcategory);
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al asignar subcategorías: " + ex.Message);
            }
        }



        public async Task<List<SubcategoriesDto>> GetAssignedSubcategoriesAsync(int organizationId)
        {
            var subcategories = await _context.Subcategoria
       .Where(s => s.Organizacions.Any(o => o.Id == organizationId))
       .Select(s => new SubcategoriesDto
       {
           Id = s.Id,
           Nombre = s.Nombre,
           NecesidadId = s.NecesidadId
       })
       .ToListAsync();

            return subcategories;
        }

        public async Task<List<NeedDto>> GetAssignedSubcategoriesGroupedAsync(int organizationId)
        {
            var subcategories = await _context.Subcategoria
                .Where(s => s.Organizacions.Any(o => o.Id == organizationId))
                .Select(s => new SubcategoriesDto
                {
                    Id = s.Id,
                    Nombre = s.Nombre,
                    NecesidadId = s.NecesidadId,
                    NecesidadNombre = s.Necesidad.Nombre,
                    NecesidadIcono = s.Necesidad.Icono
                })
                .ToListAsync();

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

    }
}
