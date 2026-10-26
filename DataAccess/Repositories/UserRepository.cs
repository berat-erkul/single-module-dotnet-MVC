using FirmaPortal.Entities;
using Microsoft.EntityFrameworkCore;

namespace FirmaPortal.DataAccess.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;

    public UserRepository(AppDbContext db) => _db = db;

    public async Task<List<User>> GetAllAsync() =>
        await _db.Users.OrderBy(u => u.Role).ThenBy(u => u.FullName).ToListAsync();

    public async Task<User?> GetByIdAsync(int id) => await _db.Users.FindAsync(id);

    public async Task<User?> GetByEmailAsync(string email) =>
        await _db.Users.FirstOrDefaultAsync(u => u.Email == email);

    public async Task<bool> EmailExistsAsync(string email, int? exceptUserId = null) =>
        await _db.Users.AnyAsync(u => u.Email == email && u.Id != exceptUserId);

    public async Task AddAsync(User user)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
        _db.Users.Update(user);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(User user)
    {
        _db.Users.Remove(user);
        await _db.SaveChangesAsync();
    }
}
