using backend.data.DataContext;
using backend.data.Models;
using backend.servicios.DTOs;
using backend.servicios.Helpers;
using backend.servicios.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;



namespace backend.servicios.Servicios
{
    public class headquartersService (ApplicationDbContext context, ILogger<headquartersService> logger, IMapsService mapsService) : IHeadquartersService
    {
        private readonly ApplicationDbContext _context = context ?? throw new ArgumentNullException(nameof(context));
        private readonly ILogger<headquartersService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IMapsService _mapsService = mapsService ?? throw new ArgumentNullException(nameof(mapsService));

        public async Task createHeadquartersAsync(List<HeadquartersDto> headquartersDtos)
        {
            if (headquartersDtos == null || !headquartersDtos.Any())
                throw new ArgumentNullException(nameof(headquartersDtos), "Las sedes no pueden ser nulas o vacías.");

            var headquarters = new List<Sede>();

            foreach (var headquartersDto in headquartersDtos)
            {
                var (lat, lng) = await _mapsService.GetCoordinates(headquartersDto.Direccion, headquartersDto.Localidad, headquartersDto.Provincia);

                var headquarter = new Sede
                {
                    Nombre = headquartersDto.Nombre,
                    Direccion = headquartersDto.Direccion,
                    Localidad = headquartersDto.Localidad,
                    Telefono = headquartersDto.Telefono,
                    Provincia = headquartersDto.Provincia,
                    Latitud = lat,
                    Longitud = lng,
                    OrganizacionId = headquartersDto.OrganizacionId
                };

                headquarters.Add(headquarter);
            }

            try
            {
                await _context.Sedes.AddRangeAsync(headquarters);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear las sedes");
                throw;
            }
        }
        public async Task<IEnumerable<HeadquartersDto>> GetAllHeadquartersAsync()
        {
            var headquarters = await _context.Sedes.ToListAsync();
            return headquarters.Select(s => new HeadquartersDto
            {
                Nombre = s.Nombre,
                Direccion = s.Direccion,
                Localidad = s.Localidad,
                Provincia = s.Provincia,
                Latitud = s.Latitud,
                Longitud = s.Longitud
            });
        }

        public async Task<IEnumerable<HeadquartersDto>> GetHeadquartersByOrganizationIdAsync(int organizationId)
        {
            var headquarters = await _context.Sedes.Where(s => s.OrganizacionId == organizationId).ToListAsync();
            return headquarters.Select(s => new HeadquartersDto
            {
                Id = s.Id,
                Nombre = s.Nombre,
                Direccion = s.Direccion,
                Localidad = s.Localidad,
                Provincia = s.Provincia,
                Telefono = s.Telefono,
                OrganizacionId = s.OrganizacionId,
                Latitud = s.Latitud,
                Longitud = s.Longitud
            });
        }

        public async Task updateHeadquartersAsync(HeadquartersDto headquartersDto)
        {
            var headquarter = await _context.Sedes.FirstOrDefaultAsync(s => s.Id == headquartersDto.Id);

            if (headquarter == null)
            {
                throw new ArgumentNullException(nameof(headquartersDto), "Las sedes no pueden ser nulas o vacías.");
            }

            var (lat, lng) = await _mapsService.GetCoordinates(headquartersDto.Direccion, headquartersDto.Localidad, headquartersDto.Provincia);

            headquarter.Nombre = headquartersDto.Nombre;
            headquarter.Direccion = headquartersDto.Direccion;
            headquarter.Localidad = headquartersDto.Localidad;
            headquarter.Telefono = headquartersDto.Telefono;
            headquarter.Provincia = headquartersDto.Provincia;
            headquarter.Latitud = lat;
            headquarter.Longitud = lng;

            try
            {
                _context.Sedes.Update(headquarter);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la sede");
                throw;
            }
        }

        public async Task deleteHeadquartersAsync(int headquartersId)
        {
            var headquarters = await _context.Sedes.FirstOrDefaultAsync(s => s.Id == headquartersId);

            if (headquarters == null)
            {
                throw new ArgumentNullException(nameof(headquartersId), "La sede no puede ser nula o vacía.");
            }

            try
            {
                _context.Sedes.Remove(headquarters);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar la sede");
                throw;
            }
        }

        public async Task<HeadquartersDto> GetHeadquarterByIdAsync(int headquartersId)
        {
            var headquarters = await _context.Sedes.FirstOrDefaultAsync(s => s.Id == headquartersId);

            if (headquarters == null)
            {
                throw new ArgumentNullException(nameof(headquartersId), "La sede no puede ser nula o vacía.");
            }

            return new HeadquartersDto
            {
                Id = headquarters.Id,
                Nombre = headquarters.Nombre,
                Direccion = headquarters.Direccion,
                Localidad = headquarters.Localidad,
                Provincia = headquarters.Provincia,
                Telefono = headquarters.Telefono,
                OrganizacionId = headquarters.OrganizacionId,
                Latitud = headquarters.Latitud,
                Longitud = headquarters.Longitud
            };
        }


        public double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            var R = 6371e3; // Radio de la tierra en metros
            var φ1 = lat1 * Math.PI / 180; // φ, λ en radianes
            var φ2 = lat2 * Math.PI / 180;
            var Δφ = (lat2 - lat1) * Math.PI / 180;
            var Δλ = (lon2 - lon1) * Math.PI / 180;

            var a = Math.Sin(Δφ / 2) * Math.Sin(Δφ / 2) +
                    Math.Cos(φ1) * Math.Cos(φ2) *
                    Math.Sin(Δλ / 2) * Math.Sin(Δλ / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            var d = R * c; // En metros
            return d;
        }
    }
}
