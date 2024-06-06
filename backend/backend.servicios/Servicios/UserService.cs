using backend.data.DataContext;
using backend.data.Models;
using backend.servicios.DTOs;
using backend.servicios.Helpers;
using backend.servicios.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace backend.servicios.Servicios
{
    public class UserService(ApplicationDbContext context, ILogger<UserService> logger, IMapsService mapsService) : IUserService
    {
        private readonly ApplicationDbContext _context = context ?? throw new ArgumentNullException(nameof(context));
        private readonly ILogger<UserService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IMapsService _mapsService = mapsService ?? throw new ArgumentNullException(nameof(mapsService));


        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            try
            {
                var users = await _context.Usuarios.Include(u => u.Rol)
                    .Select(u => new UserDto
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

                return users;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los usuarios");
                throw;
            }
        }

        public async Task<UserDto> GetUserByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentNullException(nameof(email), "El email no puede ser nulo o estar vacío.");

            try
            {
                var user =  await _context.Usuarios
                    .Include(u => u.Rol)
                    .Include(u => u.Organizacion)
                    .FirstOrDefaultAsync(u => u.Email == email);

                if (user == null)
                {
                    return null;
                }

                return new UserDto
                {
                    Id = user.Id,
                    Nombre = user.Nombre,
                    Apellido = user.Apellido,
                    Direccion = user.Direccion,
                    Email = user.Email,
                    Password = user.Contrasena,
                    Localidad = user.Localidad,
                    Provincia = user.Provincia,
                    Telefono = user.Telefono,
                    Rol = user.RolId,
                    RolNombre = user.Rol.Nombre,
                    Organizacion = user.Organizacion != null ? new OrganizationDto
                    {
                        Nombre = user.Organizacion.Nombre,
                        Cuit = user.Organizacion.Cuit,
                        Telefono = user.Organizacion.Telefono,
                        Direccion = user.Organizacion.Direccion,
                        Localidad = user.Organizacion.Localidad,
                        Provincia = user.Organizacion.Provincia
                    } : null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el usuario con email {UserEmail}", email);
                throw;
            }
        }

        public async Task CreateUserAsync(UserDto userDto)
        {
            if (userDto == null)
                throw new ArgumentNullException(nameof(userDto), "El usuario proporcionado no puede ser nulo.");

            var existingUser = await GetUserByEmailAsync(userDto.Email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("Ya existe un usuario con el email proporcionado.");
            }

            var hashedPassword = PasswordHasher.HashPassword(userDto.Password);

            var lat=0.0;
            var lng=0.0;

            if (userDto.Organizacion != null)
            {
               (lat, lng) = await _mapsService.GetCoordinates(userDto.Organizacion.Direccion, userDto.Organizacion.Localidad, userDto.Organizacion.Provincia);
            }

            var user = new Usuario
            {
                Nombre = userDto.Nombre,
                Apellido = userDto.Apellido,
                Email = userDto.Email,
                Contrasena = hashedPassword,
                Telefono = userDto.Telefono,
                Direccion = userDto.Direccion,
                Localidad = userDto.Localidad,
                Provincia = userDto.Provincia,
                RolId = userDto.Rol,
                Organizacion = userDto.Organizacion != null ? new Organizacion
                {
                    Nombre = userDto.Organizacion.Nombre,
                    Cuit = userDto.Organizacion.Cuit,
                    Direccion = userDto.Organizacion.Direccion,
                    Localidad = userDto.Organizacion.Localidad,
                    Provincia = userDto.Organizacion.Provincia,
                    Telefono = userDto.Organizacion.Telefono,
                    Latitud = lat,
                    Longitud = lng
                } : null
            };


            try
            {
                _context.Usuarios.Add(user);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear un nuevo usuario");
                throw;
            }
        }

        public async Task UpdateUserAsync(UserDto userDto)
        {
            if (userDto == null)
                throw new ArgumentNullException(nameof(userDto), "El usuario proporcionado no puede ser nulo.");

            var existingUser = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == userDto.Email);
            if (existingUser == null)
                throw new KeyNotFoundException("Usuario no encontrado para actualizar.");

            try
            {
                existingUser.Nombre = userDto.Nombre;
                existingUser.Apellido = userDto.Apellido;
                existingUser.Telefono = userDto.Telefono;
                existingUser.Direccion = userDto.Direccion;
                existingUser.Localidad = userDto.Localidad;
                existingUser.Provincia = userDto.Provincia;
                // Solo actualizar la contraseña si se proporciona una nueva
                if (!string.IsNullOrEmpty(userDto.Password))
                {
                    var passwordHasher = new PasswordHasher<Usuario>();
                    existingUser.Contrasena = passwordHasher.HashPassword(existingUser, userDto.Password);
                }

                _context.Usuarios.Update(existingUser);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el usuario con email {UserEmail}", userDto.Email);
                throw;
            }
        }

        public async Task DeleteUserAsync(string email)
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

        public async Task<UserDto> GetUserByIdAsync(int id)
        {
            try
            {
                var user = await _context.Usuarios.Include(u => u.Rol)
                    .Include(u => u.Organizacion)
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                {
                    return null;
                }

                return new UserDto
                {
                    Id = user.Id,
                    Nombre = user.Nombre,
                    Apellido = user.Apellido,
                    Direccion = user.Direccion,
                    Email = user.Email,
                    Password = user.Contrasena,
                    Localidad = user.Localidad,
                    Provincia = user.Provincia,
                    Telefono = user.Telefono,
                    Rol = user.RolId,
                    RolNombre = user.Rol.Nombre,
                    Organizacion = user.Organizacion != null ? new OrganizationDto
                    {
                        Nombre = user.Organizacion.Nombre,
                        Cuit = user.Organizacion.Cuit,
                        Telefono = user.Organizacion.Telefono,
                        Direccion = user.Organizacion.Direccion,
                        Localidad = user.Organizacion.Localidad,
                        Provincia = user.Organizacion.Provincia
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
