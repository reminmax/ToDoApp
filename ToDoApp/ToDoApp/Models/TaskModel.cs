using Plugin.CloudFirestore;
using Plugin.CloudFirestore.Attributes;
using PropertyChanged;

namespace ToDoApp.Models
{
    [AddINotifyPropertyChangedInterface]
    public class TaskModel
    {
        [Ignored] public static string CollectionPath = "taskCollection";

        public TaskModel()
        {
        }

        public TaskModel(TaskModel task)
        {
            Id = task.Id;
            Name = task.Name;
            UserId = task.UserId;
            Date = task.Date;
            Done = task.Done;
            Time = task.Time;

            // Category data
            CategoryId = task.CategoryId;
            CategoryName = task.CategoryName;
            CategoryColor = task.CategoryColor;
        }

        [Id] [MapTo("id")] public string Id { get; set; }

        [MapTo("done")] public bool Done { get; set; }

        [MapTo("name")] public string Name { get; set; }

        [MapTo("notes")] public string Notes { get; set; }

        [MapTo("date")] public string Date { get; set; }

        [MapTo("userId")] public string UserId { get; set; }

        [MapTo("time")] public Timestamp Time { get; set; }

        [Ignored] public string TimeAsString => Time.ToDateTime().ToString("hh:mm tt");

        [MapTo("createdAt")]
        [ServerTimestamp(CanReplace = false)]
        public Timestamp CreatedAt { get; set; }

        // Category
        [MapTo("categoryId")] public string CategoryId { get; set; }

        [MapTo("categoryName")] public string CategoryName { get; set; }

        [MapTo("categoryColor")] public string CategoryColor { get; set; }

        public void Update(TaskModel task)
        {
            Done = task.Done;
            Name = task.Name;
            Notes = task.Notes;
            Time = task.Time;

            // Category data
            CategoryId = task.CategoryId;
            CategoryName = task.CategoryName;
            CategoryColor = task.CategoryColor;
        }

        public override string ToString() => Name;
    }
}