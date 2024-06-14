namespace backend.repositories.interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task UpdateRangeAsync(IEnumerable<T> entity);
        Task DeleteAsync(int id);
    }
}
