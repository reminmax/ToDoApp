using PropertyChanged;
using ToDoApp.Helpers.Extensions;
using ToDoApp.Services.AnalyticsService;

namespace ToDoApp.PageModels
{
    [AddINotifyPropertyChangedInterface]
    public class NoConnectionPageModel : FreshBasePageModelExtended
    {
        public NoConnectionPageModel(IAnalyticsService analyticsService) : base(analyticsService)
        {
        }
    }
}