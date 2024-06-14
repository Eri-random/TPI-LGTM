using backend.data.Models;
using backend.repositories.interfaces;
using backend.servicios.DTOs;
using backend.servicios.Interfaces;
using Microsoft.Extensions.Logging;

namespace backend.servicios.Servicios
{
    public class InfoOrganizationService(IRepository<InfoOrganizacion> repository, ILogger<OrganizationService> logger): IOrganizationInfoService
    {
        private readonly IRepository<InfoOrganizacion> _organizacionRepository = repository ?? throw new ArgumentNullException(nameof(repository));
        private readonly ILogger<OrganizationService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public async Task SaveInfoOrganizationDataAsync(InfoOrganizationDto infoOrganizationDto)
        {
            if (infoOrganizationDto == null)
                throw new ArgumentNullException(nameof(infoOrganizationDto), "La informacion de la organizacion proporcionada no puede ser nula.");

            var infoOrganization = new InfoOrganizacion
            {
                Organizacion = infoOrganizationDto.Organizacion,
                DescripcionBreve = infoOrganizationDto.DescripcionBreve,
                DescripcionCompleta = infoOrganizationDto.DescripcionCompleta,
                Img = infoOrganizationDto.Img,
                OrganizacionId = infoOrganizationDto.OrganizacionId,
            };

            try
            {
                await _organizacionRepository.AddAsync(infoOrganization);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar la informacion de la organizacion");
                throw;
            }
        }

        public async Task UpdateInfoOrganizationAsync(InfoOrganizationDto infoOrganizationDto)
        {
            if (infoOrganizationDto == null)
                throw new ArgumentNullException(nameof(infoOrganizationDto), "La informacion de la organizacion proporcionada no puede ser nula.");

            var existingInfoOrganization = await _organizacionRepository.GetByIdAsync(infoOrganizationDto.OrganizacionId);

            if (existingInfoOrganization == null)
                throw new InvalidOperationException("La informacion de la organizacion no existe.");

            existingInfoOrganization.Organizacion = infoOrganizationDto.Organizacion;
            existingInfoOrganization.DescripcionBreve = infoOrganizationDto.DescripcionBreve;
            existingInfoOrganization.DescripcionCompleta = infoOrganizationDto.DescripcionCompleta;
            existingInfoOrganization.Img = infoOrganizationDto.Img;

            try
            {
                await _organizacionRepository.UpdateAsync(existingInfoOrganization);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la informacion de la organizacion");
                throw;
            }
        }

    }
}
