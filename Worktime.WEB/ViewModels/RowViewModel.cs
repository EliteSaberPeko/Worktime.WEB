using System.ComponentModel.DataAnnotations;

namespace Worktime.WEB.ViewModels
{
    public class RowViewModel
    {
        public int Index { get; set; }
        public int TaskId { get; set; }

        [Required(ErrorMessage = "Задача необходима!")]
        public string Identifier { get; set; } = null!;

        [Required(ErrorMessage = "Заголовок необходим!")]
        public string Title { get; set; } = null!;
        public double TotalTime { get; set; }
        public bool IsCompleted { get; set; }
        public int LineId { get; set; }
        public DateTime Date { get; set; }
        public DateTime BeginTime { get; set; }
        public DateTime EndTime { get; set; }
        public double Time { get; set; }
        public string? Description { get; set; }
    }
}
