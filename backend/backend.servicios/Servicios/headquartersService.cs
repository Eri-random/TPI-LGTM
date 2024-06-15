using backend.data.Models;
using backend.repositories.interfaces;
using backend.servicios.DTOs;
using backend.servicios.Interfaces;
using Microsoft.Extensions.Logging;

namespace backend.servicios.Servicios
{
    public class HeadquartersService(IRepository<Sede> repository, ILogger<HeadquartersService> logger, IMapsService mapsService) : IHeadquartersService
    {
        private readonly IRepository<Sede> _headquarterRepository = repository ?? throw new ArgumentNullException(nameof(repository));
        private readonly ILogger<HeadquartersService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IMapsService _mapsService = mapsService ?? throw new ArgumentNullException(nameof(mapsService));

        public async Task CreateHeadquartersAsync(List<HeadquartersDto> headquartersDto)
        {
            if (headquartersDto == null || !headquartersDto.Any())
                throw new ArgumentNullException(nameof(headquartersDto), "Las sedes no pueden ser nulas o vacías.");

            var headquarters = new List<Sede>();

            foreach (var headquarterDto in headquartersDto)
            {
                var (lat, lng) = await _mapsService.GetCoordinates(headquarterDto.Direccion, headquarterDto.Localidad, headquarterDto.Provincia);

                var headquarter = new Sede
                {
                    Nombre = headquarterDto.Nombre,
                    Direccion = headquarterDto.Direccion,
                    Localidad = headquarterDto.Localidad,
                    Telefono = headquarterDto.Telefono,
                    Provincia = headquarterDto.Provincia,
                    Latitud = lat,
                    Longitud = lng,
                    OrganizacionId = headquarterDto.OrganizacionId
                };

                headquarters.Add(headquarter);
            }

            try
            {
                await _headquarterRepository.AddRangeAsync(headquarters);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear las sedes");
                throw;
            }
        }

        public async Task<IEnumerable<HeadquartersDto>> GetAllHeadquartersAsync()
        {
            var headquarters = await _headquarterRepository.GetAllAsync();
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
            var headquarters = await _headquarterRepository.GetAllAsync();
            var organizationHeadquarters = headquarters.Where(x => x.OrganizacionId == organizationId);

            return organizationHeadquarters.Select(s => new HeadquartersDto
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

        public async Task UpdateHeadquartersAsync(HeadquartersDto headquartersDto)
        {
            var headquarters = await _headquarterRepository.GetAllAsync();
            var headquarter = headquarters.FirstOrDefault(x => x.Id == headquartersDto.Id);

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
                await _headquarterRepository.UpdateAsync(headquarter);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la sede");
                throw;
            }
        }

        public async Task DeleteHeadquartersAsync(int headquartersId)
        {
            var headquarters = await _headquarterRepository.GetAllAsync();
            var headquarter = headquarters.FirstOrDefault(x => x.Id == headquartersId);

            if (headquarter == null)
            {
                throw new ArgumentNullException(nameof(headquartersId), "La sede no puede ser nula o vacía.");
            }

            try
            {
                await _headquarterRepository.DeleteAsync(headquarter.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar la sede");
                throw;
            }
        }

        public async Task<HeadquartersDto> GetHeadquarterByIdAsync(int headquartersId)
        {
            var headquarters = await _headquarterRepository.GetAllAsync();
            var headquarter = headquarters.FirstOrDefault(x => x.Id == headquartersId);

            if (headquarter == null)
            {
                throw new ArgumentNullException(nameof(headquartersId), "La sede no puede ser nula o vacía.");
            }

            return new HeadquartersDto
            {
                Id = headquarter.Id,
                Nombre = headquarter.Nombre,
                Direccion = headquarter.Direccion,
                Localidad = headquarter.Localidad,
                Provincia = headquarter.Provincia,
                Telefono = headquarter.Telefono,
                OrganizacionId = headquarter.OrganizacionId,
                Latitud = headquarter.Latitud,
                Longitud = headquarter.Longitud
            };
        }
    }
}
