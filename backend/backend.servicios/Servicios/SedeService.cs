using backend.data.DataContext;
using backend.data.Models;
using backend.servicios.DTOs;
using backend.servicios.Helpers;
using backend.servicios.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;



namespace backend.servicios.Servicios
{
    public class SedeService (ApplicationDbContext context, ILogger<SedeService> logger, IMapsService mapsService) : ISedeService
    {
        private readonly ApplicationDbContext _context = context ?? throw new ArgumentNullException(nameof(context));
        private readonly ILogger<SedeService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IMapsService _mapsService = mapsService ?? throw new ArgumentNullException(nameof(mapsService));

        public async Task createSedeAsync(List<SedeDto> sedesDto)
        {
            if (sedesDto == null || !sedesDto.Any())
                throw new ArgumentNullException(nameof(sedesDto), "Las sedes no pueden ser nulas o vacías.");

            var sedes = new List<Sede>();

            foreach (var sedeDto in sedesDto)
            {
                var (lat, lng) = await _mapsService.GetCoordinates(sedeDto.Direccion, sedeDto.Localidad, sedeDto.Provincia);

                var sede = new Sede
                {
                    Nombre = sedeDto.Nombre,
                    Direccion = sedeDto.Direccion,
                    Localidad = sedeDto.Localidad,
                    Telefono = sedeDto.Telefono,
                    Provincia = sedeDto.Provincia,
                    Latitud = lat,
                    Longitud = lng,
                    OrganizacionId = sedeDto.OrganizacionId
                };

                sedes.Add(sede);
            }

            try
            {
                await _context.Sedes.AddRangeAsync(sedes);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear las sedes");
                throw;
            }
        }
        public async Task<IEnumerable<SedeDto>> GetAllSedesAsync()
        {
            var sedes = await _context.Sedes.ToListAsync();
            return sedes.Select(s => new SedeDto
            {
                Nombre = s.Nombre,
                Direccion = s.Direccion,
                Localidad = s.Localidad,
                Provincia = s.Provincia,
                Latitud = s.Latitud,
                Longitud = s.Longitud
            });
        }

        public async Task<IEnumerable<SedeDto>> GetSedesByOrganizacionIdAsync(int organizacionId)
        {
            var sedes = await _context.Sedes.Where(s => s.OrganizacionId == organizacionId).ToListAsync();
            return sedes.Select(s => new SedeDto
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

        public async Task updateSedeAsync(SedeDto sedeDto)
        {
            var sede = await _context.Sedes.FirstOrDefaultAsync(s => s.Id == sedeDto.Id);

            if (sede == null)
            {
                throw new ArgumentNullException(nameof(sedeDto), "Las sedes no pueden ser nulas o vacías.");
            }

            var (lat, lng) = await _mapsService.GetCoordinates(sedeDto.Direccion, sedeDto.Localidad, sedeDto.Provincia);

            sede.Nombre = sedeDto.Nombre;
            sede.Direccion = sedeDto.Direccion;
            sede.Localidad = sedeDto.Localidad;
            sede.Telefono = sedeDto.Telefono;
            sede.Provincia = sedeDto.Provincia;
            sede.Latitud = lat;
            sede.Longitud = lng;

            try
            {
                _context.Sedes.Update(sede);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la sede");
                throw;
            }
        }

        public async Task deleteSedeAsync(int sedeId)
        {
            var sede = await _context.Sedes.FirstOrDefaultAsync(s => s.Id == sedeId);

            if (sede == null)
            {
                throw new ArgumentNullException(nameof(sedeId), "La sede no puede ser nula o vacía.");
            }

            try
            {
                _context.Sedes.Remove(sede);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar la sede");
                throw;
            }
        }

        public async Task<SedeDto> GetSedeByIdAsync(int sedeId)
        {
            var sede = await _context.Sedes.FirstOrDefaultAsync(s => s.Id == sedeId);

            if (sede == null)
            {
                throw new ArgumentNullException(nameof(sedeId), "La sede no puede ser nula o vacía.");
            }

            return new SedeDto
            {
                Id = sede.Id,
                Nombre = sede.Nombre,
                Direccion = sede.Direccion,
                Localidad = sede.Localidad,
                Provincia = sede.Provincia,
                Telefono = sede.Telefono,
                OrganizacionId = sede.OrganizacionId,
                Latitud = sede.Latitud,
                Longitud = sede.Longitud
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
