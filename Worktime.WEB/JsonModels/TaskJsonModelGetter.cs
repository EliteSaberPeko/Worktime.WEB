using Worktime.Core;

namespace Worktime.WEB.JsonModels
{
    public static class TaskJsonModelGetter
    {
        public static List<TaskJsonModel> GetListByName(Startup startup, Guid userId, string name, bool isDataValueDescription = false)
        {
            List<TaskJsonModel> result = new();
            var tasks = Models.Task.GetListByName(startup, userId, name);
            foreach (var task in tasks)
            {
                string value = isDataValueDescription ?
                    task.Description ?? string.Empty :
                    task.Id.ToString();
                result.Add(new TaskJsonModel(task.Name, value));
            }
            return result;
        }
        public static List<TaskJsonModel> GetListByDescription(Startup startup, Guid userId, string description)
        {
            List<TaskJsonModel> result = new();
            var tasks = Models.Task.GetListByDescription(startup, userId, description);
            foreach (var task in tasks)
                result.Add(new TaskJsonModel(task.Description ?? task.Name, task.Id.ToString()));
            return result;
        }
    }
}
