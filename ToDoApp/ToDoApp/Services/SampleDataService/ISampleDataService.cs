using System.Collections.Generic;
using ToDoApp.Models;

namespace ToDoApp.Services.SampleDataService
{
    public interface ISampleDataService
    {
        public List<WelcomePageSlideModel> GetDataList();
        public List<string> GetColorList();
    }
}