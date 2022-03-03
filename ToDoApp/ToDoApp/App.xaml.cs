using System.Globalization;
using FreshMvvm;
using ToDoApp.Authentication;
using ToDoApp.Models;
using ToDoApp.PageModels;
using ToDoApp.Repositories.FirestoreRepository;
using ToDoApp.Resources;
using ToDoApp.Services.DateService;
using ToDoApp.Services.SampleDataService;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.Forms;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using ToDoApp.Services.AnalyticsService;

[assembly: ExportFont("FontAwesomeLight.ttf", Alias = "FontAwesomeLight")]
[assembly: ExportFont("FontAwesomeRegular.ttf", Alias = "FontAwesomeRegular")]
[assembly: ExportFont("FontAwesomeSolid.ttf", Alias = "FontAwesomeSolid")]
[assembly: ExportFont("OpenSansBold.ttf", Alias = "OpenSansBold")]
[assembly: ExportFont("OpenSansLight.ttf", Alias = "OpenSansLight")]
[assembly: ExportFont("OpenSansRegular.ttf", Alias = "OpenSansRegular")]
[assembly: ExportFont("OpenSansSemiBold.ttf", Alias = "OpenSansSemiBold")]

namespace ToDoApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Localization
            LocalizationResourceManager.Current.Init(AppResources.ResourceManager, CultureInfo.CurrentCulture);
            AppResources.Culture = LocalizationResourceManager.Current.CurrentCulture;

            LocalizationResourceManager.Current.PropertyChanged += (sender, e) =>
                AppResources.Culture = LocalizationResourceManager.Current.CurrentCulture;

            // Register services for dependency injection
            FreshIOC.Container.Register<IFirestoreRepository<TaskModel>, TasksRepository>();
            FreshIOC.Container.Register<IFirestoreRepository<TaskCategoryModel>, TaskCategoriesRepository>();
            FreshIOC.Container.Register<IDateService, DateService>();
            FreshIOC.Container.Register<ISampleDataService, SampleDataService>();
            FreshIOC.Container.Register<IAnalyticsService, AppCenterAnalyticsService>();

            // App theme
            Current.UserAppTheme = OSAppTheme.Light;

            // Respond to the theme change
            Current.RequestedThemeChanged += (sender, args) =>
                Current.UserAppTheme = args.RequestedTheme;

            // Set main page
            IFirebaseAuthentication auth = DependencyService.Get<IFirebaseAuthentication>();
            bool isLoggedIn = auth.IsLoggedIn();
            if (isLoggedIn)
            {
                MainPage = new FreshNavigationContainer(
                    FreshPageModelResolver.ResolvePageModel<TaskListPageModel>());
            }
            else
            {
                MainPage = new FreshNavigationContainer(
                    FreshPageModelResolver.ResolvePageModel<WelcomePageModel>());
            }
        }

        /// <summary>
        /// Called when the application starts.
        /// </summary>
        protected override void OnStart()
        {
            // TODO: add you MS App Center secrets below
            // MS App Center initialization
            AppCenter.Start("android={Your app secret here}" +
                            "ios={Your app secret here}",
                typeof(Analytics), typeof(Crashes));
        }

        /// <summary>
        /// Called each time the application goes to the background.
        /// </summary>
        protected override void OnSleep()
        {
        }

        /// <summary>
        /// Called when the application is resumed, after being sent to the background.
        /// </summary>
        protected override void OnResume()
        {
        }
    }
}