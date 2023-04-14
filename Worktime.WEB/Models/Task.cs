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
            var task = startup.ReadAsIEnumerable(userId).FirstOrDefault(x => x.Name == model.Name);
            if (task != null)
            {
                result = Update(startup, task, model.Description ?? string.Empty, model.IsCompleted);
            }
            else
            {
                result = Create(startup, userId, model);
            }
            return result;
        }
        public static List<WTTask> GetListByName(Startup startup, Guid userId, string name)
        {
            return startup.ReadAsIEnumerable(userId).Where(x => x.Name.ToLower().Contains(name.ToLower())).ToList();
        }
        public static List<WTTask> GetListByDescription(Startup startup, Guid userId, string description)
        {
            return startup.ReadAsIEnumerable(userId).Where(x => x.Description?.ToLower().Contains(description.ToLower()) ?? false).ToList();
        }
        private static Result<WTTask> Update(Startup startup, WTTask task, string newDescription, bool completed)
        {
            task.Description = newDescription;
            task.Completed = completed;
            var result = startup.Update(task);
            return result;
        }
        private static Result<WTTask> Create(Startup startup, Guid userId, RowViewModel newTask)
        {
            WTTask task = new()
            {
                Name = newTask.Name,
                Description = newTask.Description,
                Completed = newTask.IsCompleted,
                WTUserId = userId
            };
            var result = startup.Create(task);
            return result;
        }
    }
}
