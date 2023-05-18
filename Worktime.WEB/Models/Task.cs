using Worktime.Core;
using Worktime.Core.Models;
using Worktime.WEB.ViewModels;

namespace Worktime.WEB.Models
{
    public static class Task
    {
        public static Result<WTTask> Save(Startup startup, Guid userId, RowViewModel model)
        {
            Result<WTTask> result;
            var task = startup.ReadAsIEnumerable(userId).FirstOrDefault(x => x.Identifier == model.Identifier);
            if (task != null)
            {
                result = Update(startup, task, model.Title, model.IsCompleted);
            }
            else
            {
                result = Create(startup, userId, model);
            }
            return result;
        }
        public static List<WTTask> GetListByIdentifier(Startup startup, Guid userId, string identifier)
        {
            return startup.ReadAsIEnumerable(userId).Where(x => x.Identifier.ToLower().Contains(identifier.ToLower())).ToList();
        }
        public static List<WTTask> GetListByTitle(Startup startup, Guid userId, string title)
        {
            return startup.ReadAsIEnumerable(userId).Where(x => x.Title.ToLower().Contains(title.ToLower())).ToList();
        }
        private static Result<WTTask> Update(Startup startup, WTTask task, string newTitle, bool completed)
        {
            task.Title = newTitle;
            task.Completed = completed;
            var result = startup.Update(task);
            return result;
        }
        private static Result<WTTask> Create(Startup startup, Guid userId, RowViewModel newTask)
        {
            WTTask task = new()
            {
                Identifier = newTask.Identifier,
                Title = newTask.Title,
                Completed = newTask.IsCompleted,
                WTUserId = userId
            };
            var result = startup.Create(task);
            return result;
        }
    }
}
