using FirmaPortal.Entities;

namespace FirmaPortal.DataAccess;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (db.Users.Any()) return;

        db.Users.AddRange(
            new User { FullName = "Ali Çalışkan", Email = "calisan@firma.com", Password = "1234", Role = Role.Calisan },
            new User { FullName = "Mehmet Müdür", Email = "mudur@firma.com", Password = "1234", Role = Role.Mudur },
            new User { FullName = "Pınar Patron", Email = "patron@firma.com", Password = "1234", Role = Role.Patron });

        await db.SaveChangesAsync();
    }
}
