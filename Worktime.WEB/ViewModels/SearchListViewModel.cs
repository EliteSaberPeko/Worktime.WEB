using Worktime.Core;
using Worktime.Core.Models;
using Worktime.WEB.Enums;
namespace Worktime.WEB.ViewModels
{
    public class SearchListViewModel
    {
        public List<SearchTaskViewModel> Tasks { get; set; }
        public SearchListViewModel(Startup startup, Guid userId, SearchType searchType, string search)
        {
            Tasks = Get(startup, userId, searchType, search);
        }
        private static List<SearchTaskViewModel> Get(Startup startup, Guid userId, SearchType searchType, string search)
        {
            List<SearchTaskViewModel> result = new();

            var tasks = GetTasks(startup, userId, searchType, search);
            foreach (var task in tasks)
                result.Add(new SearchTaskViewModel(task.Id, task.Identifier, task.Title));

            return result;
        }
        private static List<WTTask> GetTasks(Startup startup, Guid userId, SearchType searchType, string search)
        {
            List<WTTask> tasks = new();
            switch (searchType)
            {
                case SearchType.Identifier:
                    tasks = Models.Task.GetListByIdentifier(startup, userId, search);
                    break;
                case SearchType.Title:
                    tasks = Models.Task.GetListByTitle(startup, userId, search);
                    break;
            }
            return tasks;
        }
    }
}
