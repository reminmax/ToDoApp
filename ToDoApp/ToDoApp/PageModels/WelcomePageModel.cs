using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using PropertyChanged;
using ToDoApp.Helpers.Extensions;
using ToDoApp.Models;
using ToDoApp.Services.AnalyticsService;
using ToDoApp.Services.SampleDataService;
using Xamarin.CommunityToolkit.ObjectModel;

namespace ToDoApp.PageModels
{
    [AddINotifyPropertyChangedInterface]
    public class WelcomePageModel : FreshBasePageModelExtended
    {
        protected new IAnalyticsService AnalyticsService { get; private set; }
        private readonly ISampleDataService _sampleDataService;

        public WelcomePageModel(ISampleDataService sampleDataService, IAnalyticsService analyticsService) : base(analyticsService)
        {
            _sampleDataService = sampleDataService;
            AnalyticsService = analyticsService;

            GetStartedCommand = CommandFactory.Create(ExecuteGetStartedCommandAsync);

            Title = "Welcome";
        }

        public ObservableCollection<WelcomePageSlideModel> WelcomePageSlideList { get; private set; }

        public ICommand GetStartedCommand { get; }

        public override void Init(object initData)
        {
            base.Init(initData);

            WelcomePageSlideList = new ObservableCollection<WelcomePageSlideModel>(_sampleDataService.GetDataList());
        }

        private async ValueTask ExecuteGetStartedCommandAsync() =>
            await CoreMethods.PushPageModel<SignupPageModel>();
    }
}