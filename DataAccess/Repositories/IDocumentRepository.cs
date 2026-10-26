using FirmaPortal.Entities;

namespace FirmaPortal.DataAccess.Repositories;

public interface IDocumentRepository
{
    Task<Document?> GetByIdAsync(int id);
    Task<List<Document>> GetByUploaderAsync(int userId);
    Task<List<Document>> GetByStatusAsync(DocumentStatus status);
    Task<bool> AnyByUploaderAsync(int userId);
    Task AddAsync(Document document);
    Task SaveChangesAsync();
    Task DeleteAsync(Document document);
}
