using Worktime.Core;
using Worktime.Core.Models;
using Worktime.WEB.ViewModels;

namespace Worktime.WEB.Models
{
    public static class Line
    {
        public static Result<WTLine> Save(Startup startup, WTTask task, RowViewModel model)
        {
            Result<WTLine> result;
            var line = startup.ReadAsIEnumerable(task.Id).FirstOrDefault(x => x.Id == model.LineId);
            if (line != null)
            {
                result = Update(startup, task, line, model);
            }
            else
            {
                result = Create(startup, task, model);
            }
            return result;
        }
        public static Result<WTLine> Delete(Startup startup, int taskId, int lineId)
        {
            var line = startup.ReadAsIEnumerable(taskId).FirstOrDefault(x => x.Id == lineId);
            if (line == null)
                return new() { Success = false, Message = "Line was not found!", Items = new() };
            var result = startup.Delete(line);
            return result;
        }
        private static Result<WTLine> Update(Startup startup, WTTask task, WTLine line, RowViewModel model)
        {
            line.WTTaskId = task.Id;
            line.Task = task;
            line.BeginTime = model.BeginTime;
            line.EndTime = model.EndTime;
            line.Date = model.Date;
            line.Description = model.Description;
            var result = startup.Update(line);
            return result;
        }
        private static Result<WTLine> Create(Startup startup, WTTask task, RowViewModel model)
        {
            WTLine line = new()
            {
                WTTaskId = task.Id,
                Task = task,
                Description = model.Description,
                Date = model.Date,
                BeginTime = model.BeginTime,
                EndTime = model.EndTime
            };
            var result = startup.Create(line);
            return result;
        }
    }
}
