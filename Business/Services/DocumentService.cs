using FirmaPortal.DataAccess.Repositories;
using FirmaPortal.Entities;

namespace FirmaPortal.Business.Services;

public class DocumentService : IDocumentService
{
    private readonly IDocumentRepository _documents;

    public DocumentService(IDocumentRepository documents) => _documents = documents;

    public async Task<List<Document>> GetVisibleForAsync(int userId, Role role)
    {
        return role switch
        {
            Role.Calisan => await _documents.GetByUploaderAsync(userId),
            Role.Mudur => await _documents.GetByStatusAsync(DocumentStatus.MudurOnayiBekliyor),
            Role.Patron => (await _documents.GetByStatusAsync(DocumentStatus.PatronOnayiBekliyor))
                .Concat(await _documents.GetByStatusAsync(DocumentStatus.Onaylandi))
                .ToList(),
            _ => new List<Document>()
        };
    }

    public async Task<Document> GetForViewAsync(int id, int userId, Role role)
    {
        var document = await FindAsync(id);
        if (role == Role.Calisan && document.UploadedById != userId)
            throw new BusinessException("Bu belgeyi görme yetkin yok.");
        return document;
    }

    public async Task<int> UploadAsync(string fileName, byte[] content, int userId, Role role)
    {
        if (role != Role.Calisan)
            throw new BusinessException("Belgeyi sadece çalışan yükleyebilir.");
        EnsureValidExcel(fileName, content);

        var document = new Document(fileName, content, userId);
        await _documents.AddAsync(document);
        return document.Id;
    }

    public async Task ReplaceAsync(int id, string fileName, byte[] content, int userId)
    {
        EnsureValidExcel(fileName, content);
        var document = await FindAsync(id);
        document.ReplaceFile(fileName, content, userId);
        await _documents.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id, int userId)
    {
        var document = await FindAsync(id);
        document.EnsureDeletableBy(userId);
        await _documents.DeleteAsync(document);
    }

    public async Task ApproveAsync(int id, int userId, Role role)
    {
        var document = await FindAsync(id);
        switch (role)
        {
            case Role.Mudur: document.ApproveByManager(userId); break;
            case Role.Patron: document.ApproveByBoss(userId); break;
            default: throw new BusinessException("Onaylama yetkin yok.");
        }
        await _documents.SaveChangesAsync();
    }

    public async Task RejectAsync(int id, int userId, Role role, string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new BusinessException("Red sebebi yazmalısın.");

        var document = await FindAsync(id);
        switch (role)
        {
            case Role.Mudur: document.RejectByManager(userId, reason); break;
            case Role.Patron: document.RejectByBoss(userId, reason); break;
            default: throw new BusinessException("Reddetme yetkin yok.");
        }
        await _documents.SaveChangesAsync();
    }

    private async Task<Document> FindAsync(int id) =>
        await _documents.GetByIdAsync(id) ?? throw new BusinessException("Belge bulunamadı.");

    private static void EnsureValidExcel(string fileName, byte[] content)
    {
        if (!fileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
            throw new BusinessException("Sadece .xlsx dosyası yüklenebilir.");
        ExcelPreviewReader.Read(content, maxRows: 1);
    }
}
