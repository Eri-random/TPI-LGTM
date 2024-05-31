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
    public class InfoOrganizationService(ApplicationDbContext context, ILogger<OrganizationService> logger): IOrganizationInfoService
    {
        private readonly ApplicationDbContext _context = context ?? throw new ArgumentNullException(nameof(context));
        private readonly ILogger<OrganizationService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));


        public async Task SaveDataInfoOrganization(InfoOrganizationDto infoOrganizationDto)
        {
            if(infoOrganizationDto == null)
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
                _context.Add(infoOrganization);
                await _context.SaveChangesAsync();

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

            var existingInfoOrganization = await _context.InfoOrganizacions.FirstOrDefaultAsync(i => i.OrganizacionId == infoOrganizationDto.OrganizacionId);

            if (existingInfoOrganization == null)
                throw new InvalidOperationException("La informacion de la organizacion no existe.");

            existingInfoOrganization.Organizacion = infoOrganizationDto.Organizacion;
            existingInfoOrganization.DescripcionBreve = infoOrganizationDto.DescripcionBreve;
            existingInfoOrganization.DescripcionCompleta = infoOrganizationDto.DescripcionCompleta;
            existingInfoOrganization.Img = infoOrganizationDto.Img;

            try
            {
                _context.Update(existingInfoOrganization);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la informacion de la organizacion");
                throw;
            }
        }

    }
}
