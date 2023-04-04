using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Worktime.WEB.Models;
using Worktime.Core;
using Worktime.WEB.ViewModels;
using System.Net;

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

        private List<RowViewModel> GetListRowViewModelOnDate(Guid userId, DateTime date)
        {
            Rows rowGetter = new(_startup);
            var tasks = _startup.ReadAsIEnumerable(userId);
            var lines = rowGetter.GetOnDate(userId, date.Date).ToList();
            List<RowViewModel> rows = new();
            foreach (var line in lines)
            {
                var task = tasks.First(x => x.Id == line.WTTaskId);
                rows.Add(new()
                {
                    TaskId = task.Id,
                    Name = task.Name,
                    Description = task.Description,
                    TotalTime = task.TotalTime,
                    IsCompleted = task.Completed,
                    LineId = line.Id,
                    Date = line.Date.ToLocalTime().Date,
                    BeginTime = line.BeginTime.ToLocalTime(),
                    EndTime = line.EndTime.ToLocalTime(),
                    Time = line.Time
                });
            }
            rows = rows.OrderByDescending(x => x.BeginTime).ToList();
            return rows;
        }
        private PageViewModel GetPageViewModelOnDate(Guid userId, DateTime date, RowViewModel? rowModel = null)
        {
            date = date.Date;
            Hours hoursGetter = new(_startup);
            List<RowViewModel> rows = GetListRowViewModelOnDate(userId, date);
            double totalTime = hoursGetter.GetOnThisDay(userId);
            return GetPageViewModel(rows, totalTime, date, rowModel);
        }
        private PageViewModel GetPageViewModel(List<RowViewModel> rows, double totalTime, DateTime date, RowViewModel? rowModel = null)
        {
            if (rowModel == null)
            {
                DateTime now = DateTime.Now;
                now = now.AddSeconds(-now.Second);
                now = now.AddMilliseconds(-now.Millisecond);
                rowModel = new()
                {
                    Date = date,
                    BeginTime = now,
                    EndTime = now
                };
            }
            return new() { NewRow = rowModel, Rows = rows, TotalTime = totalTime, ViewDate = date };
        }

        public async Task<IActionResult> Index(DateTime? ViewDate = null)
        {
            ViewDate ??= DateTime.Today;
            var user = await GetCurrentUserAsync();
            var model = GetPageViewModelOnDate(user.WorktimeId, (DateTime)ViewDate);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(RowViewModel model)
        {
            var user = await GetCurrentUserAsync();
            var task = _startup.ReadAsIEnumerable(user.WorktimeId).FirstOrDefault(x => x.Id == model.TaskId || x.Name == model.Name);
            if (task != null)
                model.TaskId = task.Id;

            if (ModelState.IsValid)
            {
                if (task != null)
                {
                    task.Name = model.Name;
                    task.Description = model.Description;
                    task.Completed = model.IsCompleted;
                    var result = _startup.Update(task);
                    if (!result.Success)
                        return StatusCode((int)HttpStatusCode.InternalServerError, result.Message);
                }
                else
                {
                    task = new()
                    {
                        Name = model.Name,
                        Description = model.Description,
                        Completed = model.IsCompleted,
                        WTUserId = user.WorktimeId
                    };
                    var result = _startup.Create(task);
                    if (result.Success)
                        task = result.Items.First();
                    else
                        return StatusCode((int)HttpStatusCode.InternalServerError, result.Message);
                }
                var line = _startup.ReadAsIEnumerable(task.Id).FirstOrDefault(x => x.Id == model.LineId);
                if (line != null)
                {
                    line.BeginTime = model.BeginTime;
                    line.EndTime = model.EndTime;
                    line.Date = model.Date;
                    var result = _startup.Update(line);
                    if (!result.Success)
                        return StatusCode((int)HttpStatusCode.InternalServerError, result.Message);
                }
                else
                {
                    line = new()
                    {
                        WTTaskId = task.Id,
                        Task = task,
                        Date = model.Date,
                        BeginTime = model.BeginTime,
                        EndTime = model.EndTime
                    };
                    var result = _startup.Create(line);
                    if (!result.Success)
                        return StatusCode((int)HttpStatusCode.InternalServerError, result.Message);
                }
                return StatusCode((int)HttpStatusCode.OK);
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.PartialContent;
                return PartialView("RowPartial", model);
            }
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