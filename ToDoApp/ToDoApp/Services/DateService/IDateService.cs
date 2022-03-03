using System.Collections.Generic;
using ToDoApp.Models.Calendar;

namespace ToDoApp.Services.DateService
{
    public interface IDateService
    {
        List<DateItemModel> GetDayList();
    }
}