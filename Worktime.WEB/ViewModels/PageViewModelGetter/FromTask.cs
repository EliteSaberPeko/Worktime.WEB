using Worktime.Core;
using Worktime.Core.Models;

namespace Worktime.WEB.ViewModels.PageViewModelGetter
{
    public class FromTask : Getter
    {
        private readonly int _taskId;
        public FromTask(Startup startup, Guid userId, int taskId) :base(startup, userId)
        {
            _taskId = taskId;
        }
        public override List<WTLine> GetLines()
        {
            Rows rowGetter = new(_startup);
            return rowGetter.GetFromTask(_userId, _taskId).ToList();
        }

        public override List<WTTask> GetTasks()
        {
            return _startup.ReadAsIEnumerable(_userId).Where(x => x.Id == _taskId).ToList();
        }

        public override double GetTotalTime()
        {
            Hours hoursGetter = new(_startup);
            return hoursGetter.GetFromTask(_userId, _taskId);
        }
    }
}
