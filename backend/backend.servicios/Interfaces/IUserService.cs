using backend.servicios.DTOs;

namespace backend.servicios.Interfaces
{
    public interface IUserService
    {
        /// <summary>
        /// Retrieves all users with their roles.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of users.</returns>
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        /// <summary>
        /// Retrieves a user by email.
        /// </summary>
        /// <param name="email">The user email.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the user if found; otherwise, null.</returns>
        Task<UserDto> GetUserByEmailAsync(string email);
        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="usuario">The user to create.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the created user.</returns>
        Task CreateUserAsync(UserDto usuarioDto);
        /// <summary>
        /// Updates an existing user.
        /// </summary>
        /// <param name="usuario">The user to update.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task UpdateUserAsync(UserDto usuario);
        /// <summary>
        /// Deletes a user by email.
        /// </summary>
        /// <param name="email">The email of the user to delete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task DeleteUserAsync(string email);

        Task<UserDto> GetUserByIdAsync(int id);
    }
}
