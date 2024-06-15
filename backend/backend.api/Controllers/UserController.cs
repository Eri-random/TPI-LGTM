using AutoMapper;
using backend.api.Models;
using backend.servicios.DTOs;
using backend.servicios.Helpers;
using backend.servicios.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace backend.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController(IUserService userService, ILogger<UserController> logger, IMapper mapper) : ControllerBase
    {
        private readonly IUserService _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        private readonly ILogger<UserController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(_mapper));

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();

                var usersResponse = new List<UserResponseModel>();

                foreach (var user in users)
                {
                    usersResponse.Add(_mapper.Map<UserResponseModel>(user));
                }

                return Ok(usersResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los usuarios");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{email}")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            try
            {
                var user = await _userService.GetUserByEmailAsync(email);
                if (user == null)
                {
                    return NotFound("Usuario no encontrado");
                }

                var userResponse = _mapper.Map<UserResponseModel>(user);

                return Ok(userResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el usuario con email {Email}", email);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserRequestModel userRequest)
        {
            if (userRequest == null)
            {
                return BadRequest("Datos de usuario inválidos");
            }

            var newUser = _mapper.Map<UserDto>(userRequest);

            try
            {
                await _userService.CreateUserAsync(newUser);
                return CreatedAtAction(nameof(CreateUser), new { email = userRequest.Email }, userRequest);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al crear al usuario con email {Email}", userRequest.Email);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear al usuario con email {Email}", userRequest.Email);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] UserLogInModel userLogIn)
        {
            if (userLogIn == null)
            {
                return BadRequest();
            }

            var user = await _userService.GetUserByEmailAsync(userLogIn.Email);

            if (user == null || !PasswordHasher.VerifyPassword(userLogIn.Password, user.Password))
            {
                return BadRequest("usuario y/o contraseña incorrectos");
            }

            var token = Token.CreateJwtToken(user.RolNombre, user.Nombre,user.Email,user.Organizacion?.Cuit,user.Organizacion?.Nombre);

            return Ok(new
            {
                token,
                Message = "Login exitoso"
            });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] UserRequestModel userRequest)
        {
            if (userRequest == null)
            {
                return BadRequest("Datos de usuario inválidos");
            }

            var userToUpdate = _mapper.Map<UserDto>(userRequest);

            try
            {
                await _userService.UpdateUserAsync(userToUpdate);
                return Ok(new { message = "Usuario actualizado con éxito" });

            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, "El usuario con email {Email} no existe", userRequest.Email);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar al usuario con email {Email}", userRequest.Email);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{email}")]
        public async Task<IActionResult> DeleteUser(string email)
        {
            var user = await _userService.GetUserByEmailAsync(email);
            if (user == null)
            {
                return NotFound("Usuario a eliminar no encontrado");
            }

            try
            {
                await _userService.DeleteUserAsync(email);
                return NoContent();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al borrar al usuario con email {Email}", user.Email);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
