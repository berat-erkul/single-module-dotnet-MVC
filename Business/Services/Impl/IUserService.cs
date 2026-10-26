using FirmaPortal.Entities;

namespace FirmaPortal.Business.Services;

public interface IUserService
{
    Task<List<User>> GetAllAsync();
    Task<User?> GetByIdAsync(int id);
    Task CreateAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(int id);

    Task<User?> FindByCredentialsAsync(string email, string password);
}
