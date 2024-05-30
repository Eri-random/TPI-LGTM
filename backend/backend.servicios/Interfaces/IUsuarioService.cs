using backend.servicios.DTOs;

namespace backend.servicios.Interfaces
{
    public interface IUsuarioService
    {
        /// <summary>
        /// Retrieves all users with their roles.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of users.</returns>
        Task<IEnumerable<UsuarioDto>> GetAllUsuariosAsync();
        /// <summary>
        /// Retrieves a user by email.
        /// </summary>
        /// <param name="email">The user email.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the user if found; otherwise, null.</returns>
        Task<UsuarioDto> GetUsuarioByEmailAsync(string email);
        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="usuario">The user to create.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the created user.</returns>
        Task CreateUsuarioAsync(UsuarioDto usuarioDto);
        /// <summary>
        /// Updates an existing user.
        /// </summary>
        /// <param name="usuario">The user to update.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task UpdateUsuarioAsync(UsuarioDto usuario);
        /// <summary>
        /// Deletes a user by email.
        /// </summary>
        /// <param name="email">The email of the user to delete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task DeleteUsuarioAsync(string email);

        Task<UsuarioDto> GetUsuarioByIdAsync(int id);
    }
}
