using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Models;
using RaceDay.Web.Services;

namespace RaceDay.Web.Controllers;

public class AccountController : Controller
{
    private readonly RaceDayApiClient _api;

    public AccountController(RaceDayApiClient api)
    {
        _api = api;
    }

    [HttpGet]
    public IActionResult Login() => View(new LoginRequest());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginRequest model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _api.LoginAsync(model.Email.Trim(), model.Password);

        if (!result.Success || result.Data is null)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Invalid email or password.");
            model.Password = string.Empty;
            return View(model);
        }

        HttpContext.Session.SetString(SessionKeys.Token, result.Data.Token);
        HttpContext.Session.SetString(SessionKeys.Role, result.Data.Role);
        HttpContext.Session.SetString(SessionKeys.Email, result.Data.Email);

        TempData["Success"] = "Welcome back!";
        return RedirectToAction("Index", "Races");
    }

    [HttpGet]
    public IActionResult Register() => View(new RegisterRequest());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterRequest model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _api.RegisterAsync(model);

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Registration failed.");
            model.Password = string.Empty;
            return View(model);
        }

        TempData["Success"] = "Account created. Please log in.";
        return RedirectToAction(nameof(Login));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        TempData["Success"] = "You have been logged out.";
        return RedirectToAction("Index", "Races");
    }
}
