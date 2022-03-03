using ToDoApp.Helpers.Extensions;
using ToDoApp.Services.AnalyticsService;

namespace ToDoApp.PageModels
{
    public class ErrorPageModel : FreshBasePageModelExtended
    {
        public ErrorPageModel(IAnalyticsService analyticsService) : base(analyticsService)
        {
        }
    }
}