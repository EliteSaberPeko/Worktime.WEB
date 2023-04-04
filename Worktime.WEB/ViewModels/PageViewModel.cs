namespace Worktime.WEB.ViewModels
{
    public class PageViewModel
    {
        public RowViewModel NewRow { get; set; } = null!;
        public List<RowViewModel> Rows { get; set; } = null!;
        public double TotalTime { get; set; }
        public DateTime ViewDate { get; set; }
    }
}
