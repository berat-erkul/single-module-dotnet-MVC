namespace FirmaPortal.Entities;

public class Document
{
    public int Id { get; private set; }

    public string FileName { get; private set; } = string.Empty;

    public byte[] Content { get; private set; } = Array.Empty<byte>();

    public int UploadedById { get; private set; }

    public User UploadedBy { get; private set; } = null!;

    public DocumentStatus Status { get; private set; }

    public DateTime UploadedAt { get; private set; }
    public DateTime? ManagerDecisionAt { get; private set; }
    public int? ManagerDecisionById { get; private set; }
    public DateTime? BossDecisionAt { get; private set; }
    public int? BossDecisionById { get; private set; }
    public string? RejectReason { get; private set; }

    private Document() { }

    public Document(string fileName, byte[] content, int uploadedById)
    {
        FileName = fileName;
        Content = content;
        UploadedById = uploadedById;
        Status = DocumentStatus.MudurOnayiBekliyor;
        UploadedAt = DateTime.UtcNow;
    }

    public void ReplaceFile(string fileName, byte[] content, int userId)
    {
        EnsureEditableBy(userId);
        FileName = fileName;
        Content = content;
        UploadedAt = DateTime.UtcNow;
    }

    public void EnsureDeletableBy(int userId) => EnsureEditableBy(userId);

    public void ApproveByManager(int managerId)
    {
        EnsureStatus(DocumentStatus.MudurOnayiBekliyor, "Sadece müdür onayı bekleyen belge onaylanabilir.");
        Status = DocumentStatus.PatronOnayiBekliyor;
        ManagerDecisionAt = DateTime.UtcNow;
        ManagerDecisionById = managerId;
    }

    public void RejectByManager(int managerId, string reason)
    {
        EnsureStatus(DocumentStatus.MudurOnayiBekliyor, "Sadece müdür onayı bekleyen belge reddedilebilir.");
        Status = DocumentStatus.Reddedildi;
        ManagerDecisionAt = DateTime.UtcNow;
        ManagerDecisionById = managerId;
        RejectReason = reason;
    }

    public void ApproveByBoss(int bossId)
    {
        EnsureStatus(DocumentStatus.PatronOnayiBekliyor, "Sadece patron onayı bekleyen belge onaylanabilir.");
        Status = DocumentStatus.Onaylandi;
        BossDecisionAt = DateTime.UtcNow;
        BossDecisionById = bossId;
    }

    public void RejectByBoss(int bossId, string reason)
    {
        EnsureStatus(DocumentStatus.PatronOnayiBekliyor, "Sadece patron onayı bekleyen belge reddedilebilir.");
        Status = DocumentStatus.Reddedildi;
        BossDecisionAt = DateTime.UtcNow;
        BossDecisionById = bossId;
        RejectReason = reason;
    }

    public bool IsEditableBy(int userId) =>
        Status == DocumentStatus.MudurOnayiBekliyor && UploadedById == userId;

    private void EnsureEditableBy(int userId)
    {
        if (UploadedById != userId)
            throw new BusinessException("Sadece kendi yüklediğin belgeyi değiştirebilir/silebilirsin.");
        EnsureStatus(DocumentStatus.MudurOnayiBekliyor, "Müdür karar verdikten sonra belge değiştirilemez/silinemez.");
    }

    private void EnsureStatus(DocumentStatus expected, string message)
    {
        if (Status != expected)
            throw new BusinessException($"{message} (Şu anki durum: {Status})");
    }
}
