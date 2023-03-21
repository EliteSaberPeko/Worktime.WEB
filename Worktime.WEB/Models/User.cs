using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Worktime.WEB.Models
{
    public class User : IdentityUser
    {
        [Required]
        public Guid WorktimeId { get; set; }
        public User() { }
        public User(Guid worktimeId)
        {
            WorktimeId = worktimeId;
        }
    }
}
