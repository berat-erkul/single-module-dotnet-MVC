using FirmaPortal.Entities;

namespace FirmaPortal.Business.Services;

public interface IDocumentService
{
    Task<List<Document>> GetVisibleForAsync(int userId, Role role);

    Task<Document> GetForViewAsync(int id, int userId, Role role);

    Task<int> UploadAsync(string fileName, byte[] content, int userId, Role role);
    Task ReplaceAsync(int id, string fileName, byte[] content, int userId);
    Task DeleteAsync(int id, int userId);

    Task ApproveAsync(int id, int userId, Role role);
    Task RejectAsync(int id, int userId, Role role, string reason);
}
