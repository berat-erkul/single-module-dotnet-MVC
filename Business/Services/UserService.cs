using FirmaPortal.DataAccess.Repositories;
using FirmaPortal.Entities;

namespace FirmaPortal.Business.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _users;
    private readonly IDocumentRepository _documents;

    public UserService(IUserRepository users, IDocumentRepository documents)
    {
        _users = users;
        _documents = documents;
    }

    public Task<List<User>> GetAllAsync() => _users.GetAllAsync();

    public Task<User?> GetByIdAsync(int id) => _users.GetByIdAsync(id);

    public async Task CreateAsync(User user)
    {
        if (await _users.EmailExistsAsync(user.Email))
            throw new BusinessException("Bu e-posta zaten kayıtlı.");
        await _users.AddAsync(user);
    }

    public async Task UpdateAsync(User user)
    {
        if (await _users.EmailExistsAsync(user.Email, exceptUserId: user.Id))
            throw new BusinessException("Bu e-posta başka bir kullanıcıda kayıtlı.");
        await _users.UpdateAsync(user);
    }

    public async Task DeleteAsync(int id)
    {
        var user = await _users.GetByIdAsync(id) ?? throw new BusinessException("Kullanıcı bulunamadı.");

        if (await _documents.AnyByUploaderAsync(id))
            throw new BusinessException("Bu kullanıcının yüklediği belgeler var, silinemez.");

        await _users.DeleteAsync(user);
    }

    public async Task<User?> FindByCredentialsAsync(string email, string password)
    {
        var user = await _users.GetByEmailAsync(email);
        return user is not null && user.Password == password ? user : null;
    }
}
