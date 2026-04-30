namespace StarterApp.Repositories;

public interface IRepository<T>
{
    Task<IReadOnlyList<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
}
