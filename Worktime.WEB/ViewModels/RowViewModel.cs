using System.ComponentModel.DataAnnotations;

namespace Worktime.WEB.ViewModels
{
    public class RowViewModel
    {
        public int Index { get; set; }
        public int TaskId { get; set; }

        [Required(ErrorMessage = "Задача необходима!")]
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public double TotalTime { get; set; }
        public bool IsCompleted { get; set; }
        public int LineId { get; set; }
        public DateTime Date { get; set; }
        public DateTime BeginTime { get; set; }
        public DateTime EndTime { get; set; }
        public double Time { get; set; }
    }
}
