using Worktime.Core;
using Worktime.Core.Models;

namespace Worktime.WEB.ViewModels.PageViewModelGetter
{
    public class FromTask : Getter
    {
        //private readonly int _taskId;
        private readonly bool _onDate;
        private double _totalTime;
        public FromTask(Startup startup, Guid userId, string taskIdentifier, DateTime? date = null) :base(startup, userId, date, taskIdentifier)
        {
            //_taskId = taskId;
            _onDate = date != null;
        }
        public override List<WTLine> GetLines()
        {
            Rows rowGetter = new(_startup);
            var lines = rowGetter.GetFromTask(_userId, _taskId).ToList();
            if (_onDate)
            {
                lines = lines.Where(x => x.Date == _date.ToUniversalTime()).ToList();
                _totalTime = lines.Sum(x => x.Time);
            }
            return lines;
        }

        public override List<WTTask> GetTasks()
        {
            return _startup.ReadAsIEnumerable(_userId).Where(x => x.Id == _taskId).ToList();
        }

        public override double GetTotalTime()
        {
            if (_onDate)
                return _totalTime;
            Hours hoursGetter = new(_startup);
            return hoursGetter.GetFromTask(_userId, _taskId);
        }
    }
}
