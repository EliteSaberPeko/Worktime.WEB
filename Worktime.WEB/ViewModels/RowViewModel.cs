using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Worktime.Core.Models;
using Worktime.WEB.Models;

namespace Worktime.WEB.ViewModels
{
    public class RowViewModel
    {
        public int TaskId { get; set; }
        [Required]
        public string Name { get; set; }
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
