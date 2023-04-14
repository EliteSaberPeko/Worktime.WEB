namespace Worktime.WEB.JsonModels
{
    public class ErrorJsonModel
    {
        public string Name { get; set; } = null!;
        public string Message { get; set; } = null!;
        public ErrorJsonModel(string name, string message)
        {
            Name = name;
            Message = message;
        }
    }
}
