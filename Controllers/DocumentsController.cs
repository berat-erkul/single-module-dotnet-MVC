using FirmaPortal.Business;
using FirmaPortal.Business.Services;
using FirmaPortal.Entities;
using FirmaPortal.Infrastructure;
using FirmaPortal.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FirmaPortal.Controllers;

[Authorize]
public class DocumentsController : Controller
{
    private readonly IDocumentService _documentService;

    public DocumentsController(IDocumentService documentService) => _documentService = documentService;

    public async Task<IActionResult> Index()
    {
        var documents = await _documentService.GetVisibleForAsync(User.GetUserId(), User.GetRole());
        return View(documents);
    }

    [HttpGet]
    [Authorize(Roles = nameof(Role.Calisan))]
    public IActionResult Upload() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = nameof(Role.Calisan))]
    public async Task<IActionResult> Upload(IFormFile? file)
    {
        if (file is null || file.Length == 0)
        {
            ModelState.AddModelError(string.Empty, "Bir dosya seçin.");
            return View();
        }

        try
        {
            var id = await _documentService.UploadAsync(file.FileName, await ReadBytesAsync(file), User.GetUserId(), User.GetRole());
            TempData["Success"] = "Belge yüklendi, müdür onayına gönderildi.";
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (BusinessException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View();
        }
    }

    public async Task<IActionResult> Details(int id)
    {
        var userId = User.GetUserId();
        var role = User.GetRole();
        Document document;
        try
        {
            document = await _documentService.GetForViewAsync(id, userId, role);
        }
        catch (BusinessException ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }

        return View(new DocumentDetailsViewModel
        {
            Document = document,
            Preview = ExcelPreviewReader.Read(document.Content),
            CanEdit = document.IsEditableBy(userId),
            CanDecide = (role == Role.Mudur && document.Status == DocumentStatus.MudurOnayiBekliyor)
                     || (role == Role.Patron && document.Status == DocumentStatus.PatronOnayiBekliyor)
        });
    }

    public async Task<IActionResult> Download(int id)
    {
        try
        {
            var document = await _documentService.GetForViewAsync(id, User.GetUserId(), User.GetRole());
            return File(document.Content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                document.FileName);
        }
        catch (BusinessException ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = nameof(Role.Calisan))]
    public async Task<IActionResult> Replace(int id, IFormFile? file)
    {
        if (file is null || file.Length == 0)
        {
            TempData["Error"] = "Bir dosya seçin.";
            return RedirectToAction(nameof(Details), new { id });
        }

        await RunAsync(async () => await _documentService.ReplaceAsync(id, file.FileName, await ReadBytesAsync(file), User.GetUserId()),
            "Dosya değiştirildi.");
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = nameof(Role.Calisan))]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await RunAsync(() => _documentService.DeleteAsync(id, User.GetUserId()), "Belge silindi.");
        return ok ? RedirectToAction(nameof(Index)) : RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = $"{nameof(Role.Mudur)},{nameof(Role.Patron)}")]
    public async Task<IActionResult> Approve(int id)
    {
        await RunAsync(() => _documentService.ApproveAsync(id, User.GetUserId(), User.GetRole()), "Belge onaylandı.");
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = $"{nameof(Role.Mudur)},{nameof(Role.Patron)}")]
    public async Task<IActionResult> Reject(int id, string reason)
    {
        await RunAsync(() => _documentService.RejectAsync(id, User.GetUserId(), User.GetRole(), reason), "Belge reddedildi.");
        return RedirectToAction(nameof(Details), new { id });
    }

    private async Task<bool> RunAsync(Func<Task> action, string successMessage)
    {
        try
        {
            await action();
            TempData["Success"] = successMessage;
            return true;
        }
        catch (BusinessException ex)
        {
            TempData["Error"] = ex.Message;
            return false;
        }
    }

    private static async Task<byte[]> ReadBytesAsync(IFormFile file)
    {
        using var ms = new MemoryStream();
        await file.CopyToAsync(ms);
        return ms.ToArray();
    }
}
