using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using PropertyChanged;
using ToDoApp.Authentication;
using ToDoApp.Helpers;
using ToDoApp.Helpers.Extensions;
using ToDoApp.Models;
using ToDoApp.Repositories.FirestoreRepository;
using ToDoApp.Services.AnalyticsService;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;

namespace ToDoApp.PageModels
{
    [AddINotifyPropertyChangedInterface]
    public class UserProfilePageModel : FreshBasePageModelExtended
    {
        //protected new IAnalyticsService AnalyticsService { get; private set; }
        protected new IAnalyticsService AnalyticsService { get; }
        private readonly IFirebaseAuthentication _firebaseAuth = DependencyService.Get<IFirebaseAuthentication>();
        private readonly IList<Language> _supportedLanguages = Enumerable.Empty<Language>().ToList();
        private readonly IFirestoreRepository<TaskModel> _taskRepository;
        private bool _isDarkModeEnabled = AppSettings.IsDarkModeEnabled;
        private bool _isHidingDoneTasksEnabled = AppSettings.IsHidingDoneTasksEnabled;
        private Language _selectedLanguage;

        public UserProfilePageModel(IFirestoreRepository<TaskModel> taskRepository, IAnalyticsService analyticsService) : base(analyticsService)
        {
            _taskRepository = taskRepository;
            AnalyticsService = analyticsService;

            LogoutCommand = CommandFactory.Create(LogoutAsync);

            SupportedLanguages = Enumerable.Empty<Language>().ToList();
            LoadLanguages();

            Title = "User profile";
        }

        public IList<Language> SupportedLanguages { get; set; }

        public string UserName { get; set; }

        public Language SelectedLanguage
        {
            get => _selectedLanguage;
            set
            {
                _selectedLanguage = value;
                RaisePropertyChanged();

                LocalizationResourceManager.Current.CurrentCulture = CultureInfo.GetCultureInfo(SelectedLanguage.Code);
            }
        }

        public bool IsDarkModeEnabled
        {
            get => _isDarkModeEnabled;
            set
            {
                AppSettings.IsDarkModeEnabled = _isDarkModeEnabled = value;
                RaisePropertyChanged();

                Application.Current.UserAppTheme = _isDarkModeEnabled ? OSAppTheme.Dark : OSAppTheme.Light;
            }
        }

        public bool IsHidingDoneTasksEnabled
        {
            get => _isHidingDoneTasksEnabled;
            set
            {
                _isHidingDoneTasksEnabled = value;
                RaisePropertyChanged();

                AppSettings.IsHidingDoneTasksEnabled = value;
            }
        }

        public UserProfileModel ProfileDetails { get; set; }

        public ICommand LogoutCommand { get; }

        /// <summary>
        /// This methods is called when the View is appearing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override async void ViewIsAppearing(object sender, EventArgs e)
        {
            base.ViewIsAppearing(sender, e);

            MainState = LayoutState.Loading;

            await GetProfileDetails();

            //IsDarkMode = Application.Current.UserAppTheme.Equals(OSAppTheme.Dark);
            //IsHideEnabled = Preferences.Get("hideDoneTasks", false);

            IFirebaseAuthentication auth = DependencyService.Get<IFirebaseAuthentication>();
            UserName = auth.GetUsername();

            MainState = LayoutState.None;
        }

        private async ValueTask LogoutAsync()
        {
            IFirebaseAuthentication auth = DependencyService.Get<IFirebaseAuthentication>();
            bool response = auth.LogOut();
            if (response)
            {
                await CoreMethods.PushPageModel<LoginPageModel>();
            }
            else
            {
                await CoreMethods.DisplayAlert("Error",
                    ConstantValues.FailedToLogOut,
                    "Ok");
            }
        }

        private async Task GetProfileDetails()
        {
            IFirebaseAuthentication auth = DependencyService.Get<IFirebaseAuthentication>();
            string userId = auth.GetUserId();
            var tasks = await _taskRepository.GetAll(userId).GetAsync();

            ProfileDetails = new UserProfileModel()
            {
                TotalTasks = tasks.Count,
                DoneTasks = tasks.ToObjects<TaskModel>().Count(t => t.Done)
            };
        }

        private void LoadLanguages()
        {
            SupportedLanguages = new List<Language>()
            {
                new Language("English", "en"),
                new Language("Русский", "ru")
            };
            SelectedLanguage = SupportedLanguages.FirstOrDefault(p =>
                p.Code == LocalizationResourceManager.Current.CurrentCulture.TwoLetterISOLanguageName);
        }
    }
}