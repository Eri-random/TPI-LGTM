﻿using backend.data.DataContext;
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
                        Nombre = u.Nombre,
                        Cuit = u.Cuit,
                        Direccion = u.Direccion,
                        Localidad = u.Localidad,
                        Provincia = u.Provincia,
                        Telefono = u.Telefono,
                        InfoOrganizacion = u.InfoOrganizacion != null ? new InfoOrganizacionDto
                        {
                            Organizacion = u.InfoOrganizacion.Organizacion,
                            DescripcionBreve = u.InfoOrganizacion.DescripcionBreve,
                            DescripcionCompleta = u.InfoOrganizacion.DescripcionCompleta,
                            Img = u.InfoOrganizacion.Img,
                        }: null
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
                var organizacion = await _context.Organizacions.FirstOrDefaultAsync(u => u.Id == id);

                if (organizacion == null)
                {
                    return null;
                }

                return new OrganizacionDto
                {
                    Nombre = organizacion.Nombre,
                    Cuit = organizacion.Cuit,
                    Direccion = organizacion.Direccion,
                    Localidad = organizacion.Localidad,
                    Provincia = organizacion.Provincia,
                    Telefono = organizacion.Telefono,
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
                var organizacion = await _context.Organizacions.FirstOrDefaultAsync(u => u.Cuit == cuit);

                if (organizacion == null)
                {
                    return null;
                }

                return new OrganizacionDto
                {
                    Nombre = organizacion.Nombre,
                    Cuit = organizacion.Cuit,
                    Direccion = organizacion.Direccion,
                    Localidad = organizacion.Localidad,
                    Provincia = organizacion.Provincia,
                    Telefono = organizacion.Telefono,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la organizacion por cuit");
                throw;
            }
        }
    }
}
