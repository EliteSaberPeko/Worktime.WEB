namespace Worktime.WEB.ViewModels
{
    public class SearchTaskViewModel
    {
        public int Id { get; set; }
        public string Identifier { get; set; }
        public string Title { get; set; }
        public SearchTaskViewModel(int id, string identifier, string title)
        {
            Id = id;
            Identifier = identifier;
            Title = title;
        }
    }
}
