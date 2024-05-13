﻿using backend.servicios.Interfaces;
using Microsoft.AspNetCore.Mvc;
using backend.servicios.Interfaces;
using backend.api.Models;

namespace backend.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MapsController(IUsuarioService usuarioService, IMapsService mapsService) : Controller
    {
        private readonly IUsuarioService _usuarioService = usuarioService ?? throw new ArgumentNullException(nameof(usuarioService));
        private readonly IMapsService _mapsService = mapsService ?? throw new ArgumentNullException(nameof(mapsService));
        [HttpGet]
        public async Task<IActionResult> GetOrganizationCoordinates()
        {

            var usuarios = await _usuarioService.GetAllUsuariosAsync();
            var organizaciones = usuarios.Where(x => x.RolNombre == "organizacion").ToList();

            var coordinates = new List<CoordinatesResponsetModel>();

            foreach (var org in organizaciones)
            {
                var (lat, lng) = await _mapsService.GetCoordinates(org.Direccion, org.Localidad, org.Provincia);
                var data = new CoordinatesResponsetModel
                {
                    Lat = lat,
                    Lng = lng,
                    Nombre = org.Nombre,
                    Direccion = org.Direccion,
                    Localidad = org.Localidad,
                    Provincia = org.Provincia,
                    Telefono = org.Telefono
                };

                coordinates.Add(data);
            }


            return Ok(coordinates);
        }
    }
}