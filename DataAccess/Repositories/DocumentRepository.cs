using FirmaPortal.Entities;
using Microsoft.EntityFrameworkCore;

namespace FirmaPortal.DataAccess.Repositories;

public class DocumentRepository : IDocumentRepository
{
    private readonly AppDbContext _db;

    public DocumentRepository(AppDbContext db) => _db = db;

    public async Task<Document?> GetByIdAsync(int id) =>
        await _db.Documents.Include(d => d.UploadedBy).FirstOrDefaultAsync(d => d.Id == id);

    public async Task<List<Document>> GetByUploaderAsync(int userId) =>
        await _db.Documents.Include(d => d.UploadedBy)
            .Where(d => d.UploadedById == userId)
            .OrderByDescending(d => d.UploadedAt)
            .ToListAsync();

    public async Task<List<Document>> GetByStatusAsync(DocumentStatus status) =>
        await _db.Documents.Include(d => d.UploadedBy)
            .Where(d => d.Status == status)
            .OrderByDescending(d => d.UploadedAt)
            .ToListAsync();

    public async Task<bool> AnyByUploaderAsync(int userId) =>
        await _db.Documents.AnyAsync(d => d.UploadedById == userId);

    public async Task AddAsync(Document document)
    {
        _db.Documents.Add(document);
        await _db.SaveChangesAsync();
    }

    public async Task SaveChangesAsync() => await _db.SaveChangesAsync();

    public async Task DeleteAsync(Document document)
    {
        _db.Documents.Remove(document);
        await _db.SaveChangesAsync();
    }
}
