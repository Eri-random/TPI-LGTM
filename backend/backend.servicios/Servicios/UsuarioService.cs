using backend.data.DataContext;
using backend.data.Models;
using backend.servicios.DTOs;
using backend.servicios.Helpers;
using backend.servicios.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace backend.servicios.Servicios
{
    public class UsuarioService(ApplicationDbContext context, ILogger<UsuarioService> logger, IMapsService mapsService) : IUsuarioService
    {
        private readonly ApplicationDbContext _context = context ?? throw new ArgumentNullException(nameof(context));
        private readonly ILogger<UsuarioService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IMapsService _mapsService = mapsService ?? throw new ArgumentNullException(nameof(mapsService));


        public async Task<IEnumerable<UsuarioDto>> GetAllUsuariosAsync()
        {
            try
            {
                var usuarios = await _context.Usuarios.Include(u => u.Rol)
                    .Select(u => new UsuarioDto
                    {
                        Id = u.Id,
                        Nombre = u.Nombre,
                        Apellido = u.Apellido,
                        Direccion = u.Direccion,
                        Email = u.Email,
                        Localidad = u.Localidad,
                        Provincia = u.Provincia,
                        Telefono = u.Telefono,
                        Rol = u.RolId,
                        RolNombre = u.Rol.Nombre,
                    }).ToListAsync();

                return usuarios;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los usuarios");
                throw;
            }
        }

        public async Task<UsuarioDto> GetUsuarioByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentNullException(nameof(email), "El email no puede ser nulo o estar vacío.");

            try
            {
                var usuario =  await _context.Usuarios
                    .Include(u => u.Rol)
                    .Include(u => u.Organizacion)
                    .FirstOrDefaultAsync(u => u.Email == email);

                if (usuario == null)
                {
                    return null;
                }

                return new UsuarioDto
                {
                    Id = usuario.Id,
                    Nombre = usuario.Nombre,
                    Apellido = usuario.Apellido,
                    Direccion = usuario.Direccion,
                    Email = usuario.Email,
                    Password = usuario.Contrasena,
                    Localidad = usuario.Localidad,
                    Provincia = usuario.Provincia,
                    Telefono = usuario.Telefono,
                    Rol = usuario.RolId,
                    RolNombre = usuario.Rol.Nombre,
                    Organizacion = usuario.Organizacion != null ? new OrganizacionDto
                    {
                        Nombre = usuario.Organizacion.Nombre,
                        Cuit = usuario.Organizacion.Cuit,
                        Telefono = usuario.Organizacion.Telefono,
                        Direccion = usuario.Organizacion.Direccion,
                        Localidad = usuario.Organizacion.Localidad,
                        Provincia = usuario.Organizacion.Provincia
                    } : null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el usuario con email {UserEmail}", email);
                throw;
            }
        }

        public async Task CreateUsuarioAsync(UsuarioDto usuarioDto)
        {
            if (usuarioDto == null)
                throw new ArgumentNullException(nameof(usuarioDto), "El usuario proporcionado no puede ser nulo.");

            var existingUser = await GetUsuarioByEmailAsync(usuarioDto.Email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("Ya existe un usuario con el email proporcionado.");
            }

            var hashedPassword = PasswordHasher.HashPassword(usuarioDto.Password);

            var lat=0.0;
            var lng=0.0;

            if (usuarioDto.Organizacion != null)
            {
               (lat, lng) = await _mapsService.GetCoordinates(usuarioDto.Organizacion.Direccion, usuarioDto.Organizacion.Localidad, usuarioDto.Organizacion.Provincia);
            }

            var usuario = new Usuario
            {
                Nombre = usuarioDto.Nombre,
                Apellido = usuarioDto.Apellido,
                Email = usuarioDto.Email,
                Contrasena = hashedPassword,
                Telefono = usuarioDto.Telefono,
                Direccion = usuarioDto.Direccion,
                Localidad = usuarioDto.Localidad,
                Provincia = usuarioDto.Provincia,
                RolId = usuarioDto.Rol,
                Organizacion = usuarioDto.Organizacion != null ? new Organizacion
                {
                    Nombre = usuarioDto.Organizacion.Nombre,
                    Cuit = usuarioDto.Organizacion.Cuit,
                    Direccion = usuarioDto.Organizacion.Direccion,
                    Localidad = usuarioDto.Organizacion.Localidad,
                    Provincia = usuarioDto.Organizacion.Provincia,
                    Telefono = usuarioDto.Organizacion.Telefono,
                    Latitud = lat,
                    Longitud = lng
                } : null
            };


            try
            {
                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear un nuevo usuario");
                throw;
            }
        }

        public async Task UpdateUsuarioAsync(UsuarioDto usuarioDto)
        {
            if (usuarioDto == null)
                throw new ArgumentNullException(nameof(usuarioDto), "El usuario proporcionado no puede ser nulo.");

            var existingUser = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == usuarioDto.Email);
            if (existingUser == null)
                throw new KeyNotFoundException("Usuario no encontrado para actualizar.");

            var hashedPassword = PasswordHasher.HashPassword(usuarioDto.Password);

            try
            {
                existingUser.Nombre = usuarioDto.Nombre;
                existingUser.Apellido = usuarioDto.Apellido;
                existingUser.Telefono = usuarioDto.Telefono;
                existingUser.Direccion = usuarioDto.Direccion;
                existingUser.Localidad = usuarioDto.Localidad;
                existingUser.Provincia = usuarioDto.Provincia;
                existingUser.Contrasena = hashedPassword;

                _context.Usuarios.Update(existingUser);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el usuario con email {UserEmail}", usuarioDto.Email);
                throw;
            }
        }

        public async Task DeleteUsuarioAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentNullException(nameof(email), "El email proporcionado no puede ser nulo o estar vacío.");

            var existingUser = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
            if (existingUser == null)
                throw new KeyNotFoundException("Usuario no encontrado para eliminar.");

            try
            {
                _context.Usuarios.Remove(existingUser);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el usuario con email {UserEmail}", email);
                throw;
            }
        }

        public async Task<UsuarioDto> GetUsuarioByIdAsync(int id)
        {
            try
            {
                var usuario = await _context.Usuarios.Include(u => u.Rol)
                    .Include(u => u.Organizacion)
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (usuario == null)
                {
                    return null;
                }

                return new UsuarioDto
                {
                    Id = usuario.Id,
                    Nombre = usuario.Nombre,
                    Apellido = usuario.Apellido,
                    Direccion = usuario.Direccion,
                    Email = usuario.Email,
                    Password = usuario.Contrasena,
                    Localidad = usuario.Localidad,
                    Provincia = usuario.Provincia,
                    Telefono = usuario.Telefono,
                    Rol = usuario.RolId,
                    RolNombre = usuario.Rol.Nombre,
                    Organizacion = usuario.Organizacion != null ? new OrganizacionDto
                    {
                        Nombre = usuario.Organizacion.Nombre,
                        Cuit = usuario.Organizacion.Cuit,
                        Telefono = usuario.Organizacion.Telefono,
                        Direccion = usuario.Organizacion.Direccion,
                        Localidad = usuario.Organizacion.Localidad,
                        Provincia = usuario.Organizacion.Provincia
                    } : null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el usuario con id {UserId}", id);
                throw;
            }
        }
    }
}
