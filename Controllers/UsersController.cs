using FirmaPortal.Business.Services;
using FirmaPortal.Entities;
using FirmaPortal.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FirmaPortal.Controllers;

public class UsersController : Controller
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService) => _userService = userService;

    public async Task<IActionResult> Index()
    {
        var users = await _userService.GetAllAsync();
        return View(users);
    }

    [HttpGet]
    public IActionResult Create() => View(new UserFormViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UserFormViewModel form)
    {
        if (string.IsNullOrWhiteSpace(form.Password))
            ModelState.AddModelError(nameof(form.Password), "Şifre zorunlu.");

        if (!ModelState.IsValid)
            return View(form);

        try
        {
            await _userService.CreateAsync(new User
            {
                FullName = form.FullName,
                Email = form.Email,
                Password = form.Password!,
                Role = form.Role
            });
        }
        catch (BusinessException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(form);
        }

        TempData["Success"] = "Kullanıcı eklendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user is null) return NotFound();

        return View(new UserFormViewModel
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UserFormViewModel form)
    {
        if (!ModelState.IsValid) return View(form);

        var user = await _userService.GetByIdAsync(id);
        if (user is null) return NotFound();

        user.FullName = form.FullName;
        user.Email = form.Email;
        user.Role = form.Role;
        if (!string.IsNullOrWhiteSpace(form.Password))
            user.Password = form.Password;

        try
        {
            await _userService.UpdateAsync(user);
        }
        catch (BusinessException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(form);
        }

        TempData["Success"] = "Kullanıcı güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _userService.DeleteAsync(id);
            TempData["Success"] = "Kullanıcı silindi.";
        }
        catch (BusinessException ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction(nameof(Index));
    }
}
