using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Worktime.Core;
using Worktime.Core.Models;
using Worktime.WEB.Models;
using Worktime.WEB.ViewModels;

namespace Worktime.WEB.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly AuthUserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthenticationController(ApplicationDbContext applicationDbContext, AuthUserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("Password", "Неправильный Email и (или) пароль");
                }
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Password) && (model.Password == model.Email))
                ModelState.AddModelError("Password", "Пароль не должен совпадать со значением в поле \"Email\"");
            if (ModelState.IsValid)
            {
                Startup startup = new();
                WTUser wtuser = new();
                do
                {
                    wtuser.Id = Guid.NewGuid();
                } while (startup.Read(wtuser.Id) != null);
                User user = new()
                {
                    Email = model.Email,
                    UserName = model.Email,
                    WorktimeId = wtuser.Id
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var wtresult = startup.Create(wtuser);
                    if (!wtresult.Success)
                    {
                        await _userManager.DeleteAsync(user);
                        ModelState.AddModelError("PasswordConfirm", wtresult.Message);
                        return View("Register", model);
                    }
                    var role = await _roleManager.FindByNameAsync("Пользователь");
                    if(role == null)
                    {
                        await _roleManager.CreateAsync(new IdentityRole("Пользователь"));
                        role = await _roleManager.FindByNameAsync("Пользователь");
                    }
                    await _userManager.AddToRolesAsync(user, new List<string> { role.Name });
                    await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("PasswordConfirm", error.Description);
                    }
                }
            }
            return View("Register", model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
