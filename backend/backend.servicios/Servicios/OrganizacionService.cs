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
    public class OrganizacionService(ApplicationDbContext context, ILogger<OrganizacionService> logger) : IOrganizacionService
    {
        private readonly ApplicationDbContext _context = context ?? throw new ArgumentNullException(nameof(context));
        private readonly ILogger<OrganizacionService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        public async Task<IEnumerable<OrganizacionDto>> GetAllOrganizacionAsync()
        {
            try
            {
                var organizacion = await _context.Organizacions.Include(u => u.InfoOrganizacion)
                    .Select(u => new OrganizacionDto
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
                        InfoOrganizacion = u.InfoOrganizacion != null ? new InfoOrganizacionDto
                        {
                            Organizacion = u.InfoOrganizacion.Organizacion,
                            DescripcionBreve = u.InfoOrganizacion.DescripcionBreve,
                            DescripcionCompleta = u.InfoOrganizacion.DescripcionCompleta,
                            Img = u.InfoOrganizacion.Img,
                            OrganizacionId = u.InfoOrganizacion.OrganizacionId
                        } : null
                    }).ToListAsync();

                return organizacion;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las organizaciones");
                throw;
            }
        }

        public async Task SaveOrganizacionAsync(OrganizacionDto organizacionDto)
        {
            if (organizacionDto == null)
                throw new ArgumentNullException(nameof(organizacionDto), "La organizacion proporcionado no puede ser nula.");

            var organizacion = new Organizacion
            {
                Nombre = organizacionDto.Nombre,
                Cuit = organizacionDto.Cuit,
                Direccion = organizacionDto.Direccion,
                Localidad = organizacionDto.Localidad,
                Provincia = organizacionDto.Provincia,
                Telefono = organizacionDto.Telefono,
            };

            try
            {
                _context.Organizacions.Add(organizacion);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear una nueva organizacion");
                throw;
            }
        }

        public async Task<OrganizacionDto> GetOrganizacionByIdAsync(int id)
        {
            try
            {
                var organizacion = await _context.Organizacions.Include(x => x.InfoOrganizacion).FirstOrDefaultAsync(u => u.Id == id);

                if (organizacion == null)
                {
                    return null;
                }

                return new OrganizacionDto
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
                    InfoOrganizacion = organizacion.InfoOrganizacion != null ? new InfoOrganizacionDto
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

        public async Task<OrganizacionDto> GetOrganizacionByCuitAsync(string cuit)
        {
            try
            {
                var organizacion = await _context.Organizacions.Include(u => u.InfoOrganizacion).FirstOrDefaultAsync(u => u.Cuit == cuit);

                if (organizacion == null)
                {
                    return null;
                }

                return new OrganizacionDto
                {
                    Id = organizacion.Id,
                    Nombre = organizacion.Nombre,
                    Cuit = organizacion.Cuit,
                    Direccion = organizacion.Direccion,
                    Localidad = organizacion.Localidad,
                    Provincia = organizacion.Provincia,
                    Telefono = organizacion.Telefono,
                    InfoOrganizacion = organizacion.InfoOrganizacion != null ? new InfoOrganizacionDto
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
                _logger.LogError(ex, "Error al obtener la organizacion por cuit");
                throw;
            }
        }

        public async Task<IEnumerable<Organizacion>> GetOrganizacionesPaginadasAsync(int page, int pageSize)
        {
            return await _context.Organizacions
                 .Include(o => o.InfoOrganizacion)
                 .Where(o => o.InfoOrganizacion != null) 
                 .Skip((page - 1) * pageSize)
                 .Take(pageSize)
                 .ToListAsync();
        }
        
        public async Task AsignarSubcategoriasAsync(int organizacionId, List<SubcategoriaDto> subcategoriasDto)
        {
            try
            {
                var organizacion = await _context.Organizacions
                    .Include(o => o.Subcategoria)
                    .FirstOrDefaultAsync(o => o.Id == organizacionId);

                if (organizacion == null)
                {
                    throw new Exception("Organización no encontrada");
                }

                // Eliminar las relaciones existentes
                organizacion.Subcategoria.Clear();

                // Obtener las nuevas subcategorías desde la base de datos
                var nuevasSubcategorias = await _context.Subcategoria
                    .Where(s => subcategoriasDto.Select(dto => dto.Id).Contains(s.Id))
                    .ToListAsync();

                // Asignar las nuevas subcategorías
                foreach (var subcategoria in nuevasSubcategorias)
                {
                    organizacion.Subcategoria.Add(subcategoria);
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al asignar subcategorías: " + ex.Message);
            }
        }



        public async Task<List<SubcategoriaDto>> GetSubcategoriasAsignadasAsync(int organizacionId)
        {
            var subcategorias = await _context.Subcategoria
                .Where(s => s.Organizacions.Any(o => o.Id == organizacionId))
                .Select(s => new SubcategoriaDto
                {
                    Id = s.Id,
                    Nombre = s.Nombre,
                    NecesidadId = s.NecesidadId
                })
                .ToListAsync();

            return subcategorias;
        }
    }
}
