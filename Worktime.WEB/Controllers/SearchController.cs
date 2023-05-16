using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;
using Worktime.Core;
using Worktime.WEB.Enums;
using Worktime.WEB.JsonModels;
using Worktime.WEB.Models;
using Worktime.WEB.ViewModels;

namespace Worktime.WEB.Controllers
{
    [Authorize]
    public class SearchController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly AuthUserManager<User> _userManager;
        private readonly Startup _startup;
        private Task<User> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);
        public SearchController(ILogger<HomeController> logger, ApplicationDbContext applicationDbContext, AuthUserManager<User> userManager, Startup startup)
        {
            _logger = logger;
            _db = applicationDbContext;
            _userManager = userManager;
            _startup = startup;
        }

        [HttpPost]
        public async Task<IActionResult> List(SearchType searchType, string search)
        {
            var user = await GetCurrentUserAsync();
            search ??= string.Empty;
            SearchListViewModel model = new(_startup, user.WorktimeId, searchType, search);
            if (model.Tasks.Count == 1)
            {
                string ViewTaskName = model.Tasks.First().Name;
                return RedirectToAction("Index", "Home", new { ViewTaskName });
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Datalist(SearchType searchType, string search)
        {
            var user = await GetCurrentUserAsync();
            search ??= string.Empty;
            List<TaskJsonModel> result;
            switch (searchType)
            {
                case SearchType.Name:
                    result = TaskJsonModelGetter.GetListByName(_startup, user.WorktimeId, search);
                    break;
                case SearchType.Description:
                    result = TaskJsonModelGetter.GetListByDescription(_startup, user.WorktimeId, search);
                    break;
                default:
                    return StatusCode((int)HttpStatusCode.InternalServerError, "Search type was not defined!");
            }
            var json = JsonSerializer.Serialize(result);
            return new JsonResult(json);
        }

        [HttpGet]
        public async Task<IActionResult> Tasks(string name)
        {
            name ??= string.Empty;
            var user = await GetCurrentUserAsync();

            var result = TaskJsonModelGetter.GetListByName(_startup, user.WorktimeId, name, true);

            var json = JsonSerializer.Serialize(result);
            return new JsonResult(json);
        }
    }
}
