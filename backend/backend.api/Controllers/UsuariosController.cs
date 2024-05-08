using backend.api.Models;
using backend.servicios.DTOs;
using backend.servicios.Helpers;
using backend.servicios.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace backend.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController(IUsuarioService usuarioService, ILogger<UsuariosController> logger) : ControllerBase
    {
        private readonly IUsuarioService _usuarioService = usuarioService ?? throw new ArgumentNullException(nameof(usuarioService));
        private readonly ILogger<UsuariosController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        [HttpGet]
        public async Task<IActionResult> GetAllUsuarios()
        {
            try
            {
                var usuarios = await _usuarioService.GetAllUsuariosAsync();

                var usuariosResponse = new List<UsuarioResponseModel>();

                foreach (var usuario in usuarios)
                {
                    usuariosResponse.Add(new UsuarioResponseModel
                    {
                        Id = usuario.Id,
                        Apellido = usuario.Apellido,
                        Direccion = usuario.Direccion,
                        Email = usuario.Email,
                        Localidad = usuario.Localidad,
                        Nombre = usuario.Nombre,
                        Provincia = usuario.Provincia,
                        RolId = usuario.Rol,
                        Telefono = usuario.Telefono
                    });
                }

                return Ok(usuariosResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los usuarios");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{email}")]
        public async Task<IActionResult> GetUsuarioByEmail(string email)
        {
            try
            {
                var usuario = await _usuarioService.GetUsuarioByEmailAsync(email);
                if (usuario == null)
                {
                    return NotFound("Usuario no encontrado");
                }

                var usuarioResponse = new UsuarioResponseModel
                {
                    Telefono = usuario.Telefono,
                    RolId=usuario.Rol,
                    Provincia=usuario.Provincia,
                    Nombre=usuario.Nombre,
                    Localidad=usuario.Localidad,
                    Apellido = usuario.Apellido,
                    Direccion = usuario.Direccion,
                    Email= usuario.Email,
                    Id = usuario.Id
                };

                return Ok(usuarioResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el usuario con email {Email}", email);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateUsuario([FromBody] UsuarioRequestModel usuarioRequest)
        {
            if (usuarioRequest == null)
            {
                return BadRequest("Datos de usuario inválidos");
            }

            var nuevoUsuario = new UsuarioDto
            {
                Nombre = usuarioRequest.Nombre,
                Apellido = usuarioRequest.Apellido,
                Email = usuarioRequest.Email,
                Telefono = usuarioRequest.Telefono,
                Direccion = usuarioRequest.Direccion,
                Localidad = usuarioRequest.Localidad,
                Provincia = usuarioRequest.Provincia,
                Password = usuarioRequest.Password,
                Rol = usuarioRequest.RolId,
            };

            try
            {
                await _usuarioService.CreateUsuarioAsync(nuevoUsuario);
                return CreatedAtAction(nameof(CreateUsuario), new { email = usuarioRequest.Email }, usuarioRequest);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al crear al usuario con email {Email}", usuarioRequest.Email);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear al usuario con email {Email}", usuarioRequest.Email);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] UsuarioLogInModel usuarioLogIn)
        {
            if (usuarioLogIn == null)
            {
                return BadRequest();
            }

            var user = await _usuarioService.GetUsuarioByEmailAsync(usuarioLogIn.Email);

            // TODO ver de pasar toda esta verificacion en el service
            if (user == null || !PasswordHasher.VerifyPassword(usuarioLogIn.Password, user.Password))
            {
                return BadRequest(new { Message = "usuario o contraseña incorrectos" });
            }

            return Ok(new
            {
                Message = "Login exitoso"
            });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUsuario([FromBody] UsuarioRequestModel usuarioRequest)
        {
            if (usuarioRequest == null)
            {
                return BadRequest("Datos de usuario inválidos");
            }

            var usuarioParaActualizar = new UsuarioDto
            {
                Nombre = usuarioRequest.Nombre,
                Apellido = usuarioRequest.Apellido,
                Telefono = usuarioRequest.Telefono,
                Direccion = usuarioRequest.Direccion,
                Localidad = usuarioRequest.Localidad,
                Provincia = usuarioRequest.Provincia,
                Password = usuarioRequest.Password,
                Email = usuarioRequest.Email
            };

            try
            {
                await _usuarioService.UpdateUsuarioAsync(usuarioParaActualizar);
                return NoContent();

            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, "El usuario con email {Email} no existe", usuarioRequest.Email);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar al usuario con email {Email}", usuarioRequest.Email);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{email}")]
        public async Task<IActionResult> DeleteUsuario(string email)
        {
            var usuario = await _usuarioService.GetUsuarioByEmailAsync(email);
            if (usuario == null)
            {
                return NotFound("Usuario a eliminar no encontrado");
            }

            try
            {
                await _usuarioService.DeleteUsuarioAsync(email);
                return NoContent();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al borrar al usuario con email {Email}", usuario.Email);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
