using System.Collections.Generic;
using ToDoApp.Models;

namespace ToDoApp.Services.SampleDataService
{
    public class SampleDataService : ISampleDataService
    {
        public List<string> GetColorList()
        {
            return new List<string>()
            {
                "#F9371C",
                "#F97C1C",
                "#F9C81C",
                "#41D0B6",
                "#2CADF6",
                "#6562FC"
            };
        }

        public List<WelcomePageSlideModel> GetDataList()
        {
            return new List<WelcomePageSlideModel>()
            {
                new WelcomePageSlideModel
                {
                    ImageSource = "slide1.png",
                    Header = "Manage your time",
                    Description =
                        "It is very difficults to manage everything. Find your virtual helper for manage your work & time."
                },
                new WelcomePageSlideModel
                {
                    ImageSource = "slide2.png",
                    Header = "Your virtual helper",
                    Description =
                        "It is very difficults to manage everything. Find your virtual helper for manage your work & time."
                },
                new WelcomePageSlideModel
                {
                    ImageSource = "slide3.png",
                    Header = "Work on time",
                    Description =
                        "It is very difficults to manage everything. Find your virtual helper for manage your work & time."
                }
            };
        }
    }
}