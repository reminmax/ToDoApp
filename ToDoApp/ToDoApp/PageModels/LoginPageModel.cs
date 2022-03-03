using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using FreshMvvm;
using PropertyChanged;
using ToDoApp.Authentication;
using ToDoApp.Helpers;
using ToDoApp.Helpers.Extensions;
using ToDoApp.Helpers.Validations;
using ToDoApp.Helpers.Validations.Rules;
using ToDoApp.Services.AnalyticsService;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;

namespace ToDoApp.PageModels
{
    [AddINotifyPropertyChangedInterface]
    public class LoginPageModel : FreshBasePageModelExtended
    {
        private FreshBasePageModel _previousPageModel;
        protected new IAnalyticsService AnalyticsService { get; private set; }

        public LoginPageModel(IAnalyticsService analyticsService) : base(analyticsService)
        {
            AnalyticsService = analyticsService;

            // Commands
            NavigateToSignupPageCommand = CommandFactory.Create(NavigateToSignupPageAsync);
            LoginCommand = new FreshAwaitCommand(LoginAsync);

            Title = "Login";

            AddValidationRules();
        }

        public ValidatableObject<string> Email { get; set; }
        public ValidatableObject<string> Password { get; set; }

        public FreshAwaitCommand LoginCommand { get; }
        public ICommand NavigateToSignupPageCommand { get; }

        /// <summary>
        /// This methods is called when the View is appearing
        /// </summary>
        protected override void ViewIsAppearing(object sender, EventArgs e)
        {
            base.ViewIsAppearing(sender, e);

            _previousPageModel = PreviousPageModel;
        }

        private void AddValidationRules()
        {
            Email = new ValidatableObject<string>();
            Password = new ValidatableObject<string>();

            Email.Validations.Add(new IsNotNullOrEmptyRule<string> {ValidationMessage = "A email is required."});
            Email.Validations.Add(new IsValidEmailRule<string> {ValidationMessage = "Email format is not correct"});
            Password.Validations.Add(new IsNotNullOrEmptyRule<string> {ValidationMessage = "A password is required."});
        }

        private bool AreFieldsValid()
        {
            bool isEmailValid = Email.Validate();
            bool isPasswordValid = Password.Validate();

            return isEmailValid && isPasswordValid;
        }

        private async void LoginAsync(TaskCompletionSource<bool> obj)
        {
            await LoginCommandHandler();
            obj.SetResult(true);
        }

        private async ValueTask LoginCommandHandler()
        {
            try
            {
                MainState = LayoutState.Loading;
                if (AreFieldsValid())
                {
                    var auth = DependencyService.Get<IFirebaseAuthentication>();
                    var user = await auth.LoginWithEmailAndPassword(Email.Value, Password.Value);

                    if (user != null)
                    {
                        ClearAuthData();
                        await CoreMethods.PushPageModel<TaskListPageModel>();
                    }
                    else
                    {
                        await CoreMethods.DisplayAlert("Error", ConstantValues.WrongUserOrPasswordError, "Ok");
                    }
                }
            }
            catch (Exception ex)
            {
                AnalyticsService.TrackError(ex, new Dictionary<string, string>
                {
                    { "Method", "LoginPageModel.LoginCommandHandler()" }
                });

                await CoreMethods.DisplayAlert("Error", ConstantValues.GeneralError, "Ok");
            }
            finally
            {
                MainState = LayoutState.None;
            }
        }

        private async ValueTask NavigateToSignupPageAsync()
        {
            if (_previousPageModel is SignupPageModel)
                await CoreMethods.PopPageModel();
            else
                await CoreMethods.PushPageModel<SignupPageModel>();
        }

        private void ClearAuthData()
        {
            Email.Value = Password.Value = string.Empty;
        }
    }
}