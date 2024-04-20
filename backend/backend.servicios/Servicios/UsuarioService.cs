using backend.data.DataContext;
using backend.data.Models;
using backend.servicios.DTOs;
using backend.servicios.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace backend.servicios.Servicios
{
    public class UsuarioService(ApplicationDbContext context, ILogger<UsuarioService> logger) : IUsuarioService
    {
        private readonly ApplicationDbContext _context = context ?? throw new ArgumentNullException(nameof(context));
        private readonly ILogger<UsuarioService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));


        public async Task<IEnumerable<UsuarioDto>> GetAllUsuariosAsync()
        {
            try
            {
                var usuarios = await _context.Usuarios
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
                        Rol = u.RolId
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
                var usuario =  await _context.Usuarios.Include(u => u.Rol).FirstOrDefaultAsync(u => u.Email == email);

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
                    Localidad = usuario.Localidad,
                    Provincia = usuario.Provincia,
                    Telefono = usuario.Telefono,
                    Rol = usuario.RolId
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

            var usuario = new Usuario
            {
                Nombre = usuarioDto.Nombre,
                Apellido = usuarioDto.Apellido,
                Email = usuarioDto.Email,
                Contrasena = usuarioDto.Password,
                Telefono = usuarioDto.Telefono,
                Direccion = usuarioDto.Direccion,
                Localidad = usuarioDto.Localidad,
                Provincia = usuarioDto.Provincia,
                RolId = 1
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

            try
            {
                existingUser.Nombre = usuarioDto.Nombre;
                existingUser.Apellido = usuarioDto.Apellido;
                existingUser.Telefono = usuarioDto.Telefono;
                existingUser.Direccion = usuarioDto.Direccion;
                existingUser.Localidad = usuarioDto.Localidad;
                existingUser.Provincia = usuarioDto.Provincia;
                existingUser.Contrasena = usuarioDto.Password;

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
    }
}
