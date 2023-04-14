namespace Worktime.WEB.JsonModels
{
    public class TaskJsonModel
    {
        public string Text { get; set; }
        public string DataValue { get; set; }
        public TaskJsonModel(string text, string dataValue)
        {
            Text = text;
            DataValue = dataValue;
        }
    }
}
