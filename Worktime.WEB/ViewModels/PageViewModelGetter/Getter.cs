using Worktime.Core.Models;
using Worktime.Core;

namespace Worktime.WEB.ViewModels.PageViewModelGetter
{
    public abstract class Getter
    {
        protected Startup _startup;
        protected DateTime _date;
        protected Guid _userId;
        protected int _taskId;
        protected readonly string _taskIdentifier = string.Empty;
        public Getter(Startup startup, Guid userId, DateTime? date = null, string taskIdentifier = "")
        {
            _startup = startup;
            _userId = userId;
            _date = date == null ? DateTime.Today : (DateTime)date;
            if (!string.IsNullOrWhiteSpace(taskIdentifier))
            {
                var task = startup.ReadAsIEnumerable(userId).FirstOrDefault(x => x.Identifier.ToLower().Equals(taskIdentifier.ToLower()));
                if(task != null)
                {
                    _taskIdentifier = task.Identifier;
                    _taskId = task.Id;
                }
            }
        }
        private List<RowViewModel> GetListRowViewModel(List<WTTask> tasks, List<WTLine> lines)
        {
            List<RowViewModel> rows = new();
            int index = 1;
            foreach (var line in lines)
            {
                var task = tasks.First(x => x.Id == line.WTTaskId);
                rows.Add(new()
                {
                    Index = index,
                    TaskId = task.Id,
                    Identifier = task.Identifier,
                    Title = task.Title,
                    TotalTime = task.TotalTime,
                    IsCompleted = task.Completed,
                    LineId = line.Id,
                    Date = line.Date.ToLocalTime().Date,
                    BeginTime = line.BeginTime.ToLocalTime(),
                    EndTime = line.EndTime.ToLocalTime(),
                    Time = line.Time,
                    Description = line.Description
                });
                index++;
            }
            rows = rows.OrderByDescending(x => x.BeginTime).ToList();
            return rows;
        }
        public ViewModels.PageViewModel Get(RowViewModel? newRow = null)
        {
            var date = _date.Date;
            var tasks = GetTasks();
            var lines = GetLines();
            List<RowViewModel> rows = GetListRowViewModel(tasks, lines);

            double totalTime = GetTotalTime();
            newRow = GetNewRow(date, newRow);
            return new ViewModels.PageViewModel(newRow, rows, totalTime, date, _taskIdentifier);
        }
        private RowViewModel GetNewRow(DateTime date, RowViewModel? newRow)
        {
            if (newRow == null)
            {
                DateTime now = DateTime.Now;
                now = new(date.Year, date.Month, date.Day, now.Hour, now.Minute, 0, 0);
                newRow = new()
                {
                    Index = 0,
                    Date = date,
                    BeginTime = now,
                    EndTime = now
                };
            }
            return newRow;
        }
        public abstract List<WTTask> GetTasks();
        public abstract List<WTLine> GetLines();
        public abstract double GetTotalTime();
    }
}
