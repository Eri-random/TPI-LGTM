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
    public class InfoOrganizacionService(ApplicationDbContext context, ILogger<OrganizacionService> logger): IOrganizacionInfoService
    {
        private readonly ApplicationDbContext _context = context ?? throw new ArgumentNullException(nameof(context));
        private readonly ILogger<OrganizacionService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));


        public async Task SaveDataInfoOrganizacion(InfoOrganizacionDto infoOrganizacionDto)
        {
            if(infoOrganizacionDto == null)
                throw new ArgumentNullException(nameof(infoOrganizacionDto), "La informacion de la organizacion proporcionada no puede ser nula.");

            var infoOrganizacion = new InfoOrganizacion
            {
                Organizacion = infoOrganizacionDto.Organizacion,
                DescripcionBreve = infoOrganizacionDto.DescripcionBreve,
                DescripcionCompleta = infoOrganizacionDto.DescripcionCompleta,
                Img = infoOrganizacionDto.Img,
                OrganizacionId = infoOrganizacionDto.OrganizacionId,
            };

            try
            {
                _context.Add(infoOrganizacion);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar la informacion de la organizacion");
                throw;
            }
        }

        public async Task UpdateInfoOrganizacionAsync(InfoOrganizacionDto infoOrganizacionDto)
        {
            if (infoOrganizacionDto == null)
                throw new ArgumentNullException(nameof(infoOrganizacionDto), "La informacion de la organizacion proporcionada no puede ser nula.");

            var existingInfoOrganizacion = await _context.InfoOrganizacions.FirstOrDefaultAsync(i => i.OrganizacionId == infoOrganizacionDto.OrganizacionId);

            if (existingInfoOrganizacion == null)
                throw new InvalidOperationException("La informacion de la organizacion no existe.");

            existingInfoOrganizacion.Organizacion = infoOrganizacionDto.Organizacion;
            existingInfoOrganizacion.DescripcionBreve = infoOrganizacionDto.DescripcionBreve;
            existingInfoOrganizacion.DescripcionCompleta = infoOrganizacionDto.DescripcionCompleta;
            existingInfoOrganizacion.Img = infoOrganizacionDto.Img;

            try
            {
                _context.Update(existingInfoOrganizacion);
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
