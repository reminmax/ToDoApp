using System;
using System.Collections.Generic;
using ToDoApp.Models.Calendar;
using ToDoApp.Resources;

namespace ToDoApp.Services.DateService
{
    public class DateService : IDateService
    {
        public List<DateItemModel> GetDayList()
        {
            List<DateItemModel> dayList = new List<DateItemModel>();

            var dateInit = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var dateEnd = new DateTime(dateInit.Year, dateInit.Month,
                DateTime.DaysInMonth(dateInit.Year, dateInit.Month));

            string month = dateInit.ToString("MMM");

            for (int i = 1; i <= dateEnd.Day; i++)
            {
                DateTime date = new DateTime(dateInit.Year, dateInit.Month, i);

                dayList.Add(new DateItemModel()
                {
                    Date = date,
                    Day = string.Format("{0:00}", i),
                    Month = month,
                    //DayOfWeek = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedDayName(date.DayOfWeek),
                    DayOfWeek = AppResources.Culture.DateTimeFormat.GetAbbreviatedDayName(date.DayOfWeek),
                    IsSelected = i == DateTime.Today.Day,
                });
                ;
            }

            return dayList;
        }
    }
}