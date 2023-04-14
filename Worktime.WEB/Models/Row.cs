using Worktime.Core;
using Worktime.Core.Models;
using Worktime.WEB.ViewModels;

namespace Worktime.WEB.Models
{
    public static class Row
    {
        public static Result<WTLine> Save(Startup startup, Guid userId, RowViewModel model)
        {
            var resultTask = Models.Task.Save(startup, userId, model);
            if (!resultTask.Success)
                return new Result<WTLine>() { Success = resultTask.Success, Message = resultTask.Message, Items = new()};
            
            var task = resultTask.Items.First();
            var resultLine = Models.Line.Save(startup, task, model);
            return resultLine;
        }
        public static Result<WTLine> Delete(Startup startup, int taskId, int lineId)
        {
            return Models.Line.Delete(startup, taskId, lineId);
        }
    }
}
