using System;

namespace ToDoApp.Models.Calendar
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public class DateItemModel
    {
        public DateTime Date { get; set; }
        public string Month { get; set; }
        public string Day { get; set; }
        public string DayOfWeek { get; set; }
        public bool IsSelected { get; set; }
    }
}