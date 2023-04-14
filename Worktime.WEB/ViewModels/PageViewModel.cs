namespace Worktime.WEB.ViewModels
{
    public class PageViewModel
    {
        public RowViewModel NewRow { get; set; }
        public List<RowViewModel> Rows { get; set; }
        public double TotalTime { get; set; }
        public DateTime ViewDate { get; set; }
        public PageViewModel(RowViewModel newRow, List<RowViewModel> rows, double totalTime, DateTime viewDate)
        {
            NewRow = newRow; 
            Rows = rows;
            TotalTime = totalTime;
            ViewDate = viewDate;
        }
    }
}
