using Plugin.CloudFirestore.Attributes;

namespace ToDoApp.Models
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public class TaskCategoryModel
    {
        [Ignored]
        public static string CollectionPath = "categoryCollection";

        public TaskCategoryModel()
        {
        }

        public TaskCategoryModel(TaskCategoryModel category)
        {
            Id = category.Id;
            Name = category.Name;
            Color = category.Color;
            UserId = category.UserId;
        }

        [Id] [MapTo("id")] public string Id { get; set; }

        [MapTo("name")] public string Name { get; set; }

        [MapTo("color")] public string Color { get; set; }

        [MapTo("userId")] public string UserId { get; set; }

        [Ignored] public bool IsSelected { get; set; }

        public void Update(TaskCategoryModel category)
        {
            Name = category.Name;
            Color = category.Color;
            UserId = category.UserId;
        }

        public override string ToString() => Name;
    }
}