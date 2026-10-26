using FirmaPortal.Entities;

namespace FirmaPortal.ViewModels;

public class DocumentDetailsViewModel
{
    public required Document Document { get; init; }
    public required List<List<string>> Preview { get; init; }
    public bool CanEdit { get; init; }
    public bool CanDecide { get; init; }
}
