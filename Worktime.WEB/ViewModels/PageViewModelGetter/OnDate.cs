using Worktime.Core.Models;
using Worktime.Core;

namespace Worktime.WEB.ViewModels.PageViewModelGetter
{
    public class OnDate : Getter
    {
        public OnDate(Startup startup, Guid userId, DateTime? date = null) : base(startup, userId, date)
        {
        }

        public override List<WTLine> GetLines()
        {
            Rows rowGetter = new(_startup);
            return rowGetter.GetOnDate(_userId, _date.Date).ToList();
        }

        public override List<WTTask> GetTasks()
        {
            return _startup.ReadAsIEnumerable(_userId).ToList();
        }

        public override double GetTotalTime()
        {
            Hours hoursGetter = new(_startup);
            return hoursGetter.GetOnDay(_userId, _date);
        }
    }
}
