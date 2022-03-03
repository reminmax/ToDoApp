using PropertyChanged;

namespace ToDoApp.Models
{
    [AddINotifyPropertyChangedInterface]
    public class UserProfileModel
    {
        public int TotalTasks { get; set; }

        public int DoneTasks { get; set; }
    }
}