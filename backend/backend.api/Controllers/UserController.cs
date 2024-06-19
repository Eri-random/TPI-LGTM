using AutoMapper;
using backend.api.Models.RequestModels;
using backend.api.Models.ResponseModels;
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

        /// <summary>
        /// Get all users.
        /// </summary>
        /// <response code="200">Returns the list of users.</response>
        /// <response code="500">If there is an internal server error.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UserResponseModel>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                return Ok(_mapper.Map<IEnumerable<UserResponseModel>>(users));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all users");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get a user by email.
        /// </summary>
        /// <param name="email">The email of the user.</param>
        /// <response code="200">Returns the user.</response>
        /// <response code="404">If the user is not found.</response>
        /// <response code="500">If there is an internal server error.</response>
        [HttpGet("{email}")]
        [ProducesResponseType(typeof(UserResponseModel), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            try
            {
                var user = await _userService.GetUserByEmailAsync(email);

                if (user == null)
                    return NotFound("User not found");

                return Ok(_mapper.Map<UserResponseModel>(user));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user with email {Email}", email);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Create a new user.
        /// </summary>
        /// <param name="userRequest">The user request model.</param>
        /// <response code="201">Returns the newly created user.</response>
        /// <response code="400">If the user data is invalid.</response>
        /// <response code="500">If there is an internal server error.</response>
        [HttpPost]
        [ProducesResponseType(typeof(UserRequestModel), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateUser([FromBody] UserRequestModel userRequest)
        {
            if (userRequest == null)
                return BadRequest("Invalid user data");

            try
            {
                var newUser = _mapper.Map<UserDto>(userRequest);
                await _userService.CreateUserAsync(newUser);
                return CreatedAtAction(nameof(CreateUser), new { email = userRequest.Email }, userRequest);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user with email {Email}", userRequest.Email);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Authenticate a user.
        /// </summary>
        /// <param name="userLogIn">The user login request model.</param>
        /// <response code="200">Returns the authentication token.</response>
        /// <response code="400">If the login data is invalid or authentication fails.</response>
        /// <response code="500">If there is an internal server error.</response>
        [HttpPost("authenticate")]
        [ProducesResponseType(typeof(UserLogInResponseModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Authenticate([FromBody] UserLogInRequestModel userLogIn)
        {
            if (userLogIn == null)
                return BadRequest();

            try
            {
                var user = await _userService.GetUserByEmailAsync(userLogIn.Email);
                if (user == null || !PasswordHasher.VerifyPassword(userLogIn.Password, user.Password))
                    return BadRequest("Invalid username or password");

                var token = Token.CreateJwtToken(user.RolNombre, user.Nombre, user.Email, user.Organizacion?.Cuit, user.Organizacion?.Nombre);

                return Ok(new UserLogInResponseModel { Token = token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error authenticating user with email {Email}", userLogIn.Email);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Update a user.
        /// </summary>
        /// <param name="userRequest">The user request model.</param>
        /// <response code="200">Returns a success message.</response>
        /// <response code="400">If the user data is invalid or the user is not found.</response>
        /// <response code="500">If there is an internal server error.</response>
        [HttpPut]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateUser([FromBody] UserRequestModel userRequest)
        {
            if (userRequest == null)
                return BadRequest("Invalid user data");

            try
            {
                var userToUpdate = _mapper.Map<UserDto>(userRequest);
                await _userService.UpdateUserAsync(userToUpdate);

                return Ok(new { message = "User successfully updated" });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, "User with email {Email} not found", userRequest.Email);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user with email {Email}", userRequest.Email);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Delete a user by email.
        /// </summary>
        /// <param name="email">The email of the user to delete.</param>
        /// <response code="204">If the user was successfully deleted.</response>
        /// <response code="404">If the user is not found.</response>
        /// <response code="500">If there is an internal server error.</response>
        [HttpDelete("{email}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteUser(string email)
        {
            try
            {
                var user = await _userService.GetUserByEmailAsync(email);
                if (user == null)
                    return NotFound("User to delete not found");

                await _userService.DeleteUserAsync(email);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user with email {Email}", email);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
