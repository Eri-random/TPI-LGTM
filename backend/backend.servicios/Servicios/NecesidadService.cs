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
    public class NecesidadService(ApplicationDbContext context, ILogger<NecesidadService> logger) : INeedService
    {
        private readonly ApplicationDbContext _context = context ?? throw new ArgumentNullException(nameof(context));
        private readonly ILogger<NecesidadService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        public async Task<IEnumerable<NeedDto>> GetAllNeedAsync()
        {
            try
            {
                var needs = await _context.Necesidads.Include(u => u.Subcategoria)
                    .Select(u => new NeedDto
                    {
                        Id = u.Id,
                        Nombre = u.Nombre,
                        Icono = u.Icono,
                        Subcategoria = u.Subcategoria.Select(p => new SubcategoriesDto
                        {
                            Id = p.Id,
                            Nombre = p.Nombre,
                            NecesidadId = p.NecesidadId
                        }).ToList()

                    }).ToListAsync();

                return needs;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las necesidades");
                throw;
            }
        }
    }
}
