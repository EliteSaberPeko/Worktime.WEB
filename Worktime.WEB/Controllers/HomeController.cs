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
            var task = _startup.ReadAsIEnumerable(user.WorktimeId).FirstOrDefault(x => x.Name == model.Name);

            if (ModelState.IsValid)
            {
                model.BeginTime = new(model.Date.Year, model.Date.Month, model.Date.Day, model.BeginTime.Hour, model.BeginTime.Minute, model.BeginTime.Second);
                model.EndTime = new(model.Date.Year, model.Date.Month, model.Date.Day, model.EndTime.Hour, model.EndTime.Minute, model.EndTime.Second);
                if (task != null)
                {
                    //task.Name = model.Name;
                    task.Description = model.Description;
                    task.Completed = model.IsCompleted;
                    var result = _startup.Update(task);
                    if (!result.Success)
                        return StatusCode((int)HttpStatusCode.InternalServerError, result.Message);
                    task = result.Items.First();
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
                    if (!result.Success)
                        return StatusCode((int)HttpStatusCode.InternalServerError, result.Message);
                    task = result.Items.First();
                }
                var line = _startup.ReadAsIEnumerable(model.TaskId).FirstOrDefault(x => x.Id == model.LineId);
                line ??= _startup.ReadAsIEnumerable(task.Id).FirstOrDefault(x => x.Id == model.LineId);
                if (line != null)
                {
                    line.WTTaskId = task.Id;
                    line.Task = task;
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
                List<ErrorJsonModel> errors = new();
                var state = ModelState.Where(x => x.Value?.ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid);
                foreach(var item in state)
                {
                    string name = item.Key;
                    string message = item.Value?.Errors.FirstOrDefault()?.ErrorMessage ?? string.Empty;
                    errors.Add(new() { Message = message, Name = name });
                }
                Response.StatusCode = (int)HttpStatusCode.PartialContent;
                string json = JsonSerializer.Serialize(errors);
                return new JsonResult(json);
            }
        }

        [HttpGet]
        public async Task<IActionResult> ShowTasks(string name)
        {
            name ??= string.Empty;
            var user = await GetCurrentUserAsync();
            var tasks = _startup.ReadAsIEnumerable(user.WorktimeId).Where(x => x.Name.Contains(name));
            List<TaskJsonModel> result = new();
            foreach (var task in tasks)
                result.Add(new() { Name = task.Name, Description = task.Description ?? string.Empty });

            var json = JsonSerializer.Serialize(result);
            return new JsonResult(json);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(RowViewModel model)
        {
            var user = await GetCurrentUserAsync();
            var line = _startup.ReadAsIEnumerable(model.TaskId).FirstOrDefault(x => x.Id == model.LineId);
            if (line == null)
                return StatusCode((int)HttpStatusCode.InternalServerError, "Line was not found!");
            var result = _startup.Delete(line);
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

        #region Вспомогательные методы NonAction
        [NonAction]
        private List<RowViewModel> GetListRowViewModelOnDate(Guid userId, DateTime date)
        {
            Rows rowGetter = new(_startup);
            var tasks = _startup.ReadAsIEnumerable(userId);
            var lines = rowGetter.GetOnDate(userId, date.Date).ToList();
            List<RowViewModel> rows = new();
            int index = 1;
            foreach (var line in lines)
            {
                var task = tasks.First(x => x.Id == line.WTTaskId);
                rows.Add(new()
                {
                    Index = index,
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
                index++;
            }
            rows = rows.OrderByDescending(x => x.BeginTime).ToList();
            return rows;
        }
        [NonAction]
        private PageViewModel GetPageViewModelOnDate(Guid userId, DateTime date, RowViewModel? rowModel = null)
        {
            date = date.Date;
            Hours hoursGetter = new(_startup);
            List<RowViewModel> rows = GetListRowViewModelOnDate(userId, date);
            double totalTime = hoursGetter.GetOnThisDay(userId);
            return GetPageViewModel(rows, totalTime, date, rowModel);
        }
        [NonAction]
        private PageViewModel GetPageViewModel(List<RowViewModel> rows, double totalTime, DateTime date, RowViewModel? rowModel = null)
        {
            if (rowModel == null)
            {
                DateTime now = DateTime.Now;
                now = new(date.Year, date.Month, date.Day, now.Hour, now.Minute, 0, 0);
                rowModel = new()
                {
                    Index = 0,
                    Date = date,
                    BeginTime = now,
                    EndTime = now
                };
            }
            return new() { NewRow = rowModel, Rows = rows, TotalTime = totalTime, ViewDate = date };
        } 
        #endregion
    }
}