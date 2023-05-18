using Worktime.Core;

namespace Worktime.WEB.JsonModels
{
    public static class TaskJsonModelGetter
    {
        public static List<TaskJsonModel> GetListByIdentifier(Startup startup, Guid userId, string identifier, bool isDataValueTitle = false)
        {
            List<TaskJsonModel> result = new();
            var tasks = Models.Task.GetListByIdentifier(startup, userId, identifier);
            foreach (var task in tasks)
            {
                string value = isDataValueTitle ?
                    task.Title :
                    task.Id.ToString();
                result.Add(new TaskJsonModel(task.Identifier, value));
            }
            return result;
        }
        public static List<TaskJsonModel> GetListByTitle(Startup startup, Guid userId, string title)
        {
            List<TaskJsonModel> result = new();
            var tasks = Models.Task.GetListByTitle(startup, userId, title);
            foreach (var task in tasks)
                result.Add(new TaskJsonModel(task.Title, task.Id.ToString()));
            return result;
        }
    }
}
