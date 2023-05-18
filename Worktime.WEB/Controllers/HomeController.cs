using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net;
using System.Text.Json;
using Worktime.WEB.Models;
using Worktime.Core;
using Worktime.WEB.ViewModels;
using Worktime.WEB.JsonModels;

namespace Worktime.WEB.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly AuthUserManager<User> _userManager;
        private readonly Startup _startup;
        private Task<User> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext applicationDbContext, AuthUserManager<User> userManager, Startup startup)
        {
            _logger = logger;
            _db = applicationDbContext;
            _userManager = userManager;
            _startup = startup;
        }

        public async Task<IActionResult> Index(DateTime? ViewDate = null, string ViewTaskIdentifier = "")
        {
            var user = await GetCurrentUserAsync();
            PageViewModel model = string.IsNullOrWhiteSpace(ViewTaskIdentifier) ?
                new ViewModels.PageViewModelGetter.OnDate(_startup, user.WorktimeId, ViewDate ?? DateTime.Today).Get() :
                new ViewModels.PageViewModelGetter.FromTask(_startup, user.WorktimeId, ViewTaskIdentifier, ViewDate).Get();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(RowViewModel model)
        {
            var user = await GetCurrentUserAsync();

            if (ModelState.IsValid)
            {
                model.BeginTime = new(model.Date.Year, model.Date.Month, model.Date.Day, model.BeginTime.Hour, model.BeginTime.Minute, model.BeginTime.Second);
                model.EndTime = new(model.Date.Year, model.Date.Month, model.Date.Day, model.EndTime.Hour, model.EndTime.Minute, model.EndTime.Second);

                var result = Models.Row.Save(_startup, user.WorktimeId, model);
                if (!result.Success)
                    return StatusCode((int)HttpStatusCode.InternalServerError, result.Message);

                return StatusCode((int)HttpStatusCode.OK);
            }
            else
            {
                List<ErrorJsonModel> errors = new();
                var state = ModelState.Where(x => x.Value?.ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid);
                foreach(var (name, message) in state.Select(x => (Name: x.Key, Message: x.Value?.Errors.FirstOrDefault()?.ErrorMessage ?? string.Empty)))
                {
                    if (name == "Date")
                        errors.Add(new ErrorJsonModel(name, "Необходима корректная дата!"));
                    else
                        errors.Add(new ErrorJsonModel(name, message));
                }
                Response.StatusCode = (int)HttpStatusCode.PartialContent;
                string json = JsonSerializer.Serialize(errors);
                return new JsonResult(json);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(RowViewModel model)
        {
            var user = await GetCurrentUserAsync();
            var result = Models.Row.Delete(_startup, model.TaskId, model.LineId);
            if(!result.Success)
                return StatusCode((int)HttpStatusCode.InternalServerError, result.Message);

            return StatusCode((int)HttpStatusCode.OK);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}