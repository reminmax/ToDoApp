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
    public class SignupPageModel : FreshBasePageModelExtended
    {
        protected new IAnalyticsService AnalyticsService { get; private set; }
        private FreshBasePageModel _previousPageModel;

        public SignupPageModel(IAnalyticsService analyticsService) : base(analyticsService)
        {
            AnalyticsService = analyticsService;

            // Commands
            SignUpCommand = new FreshAwaitCommand(SignUpAsync);
            NavigateToLoginPageCommand = CommandFactory.Create(NavigateToLoginPageCommandHandler);

            Title = "Sign up";

            AddValidationRules();
        }

        public ValidatableObject<string> UserName { get; set; }
        public ValidatableObject<string> Email { get; set; }
        public ValidatablePair<string> Password { get; set; }

        public FreshAwaitCommand SignUpCommand { get; }
        public ICommand NavigateToLoginPageCommand { get; }

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
            UserName = new ValidatableObject<string>();
            Email = new ValidatableObject<string>();
            Password = new ValidatablePair<string>();

            UserName.Validations.Add(new IsNotNullOrEmptyRule<string> {ValidationMessage = "Username is required."});

            // Email Validation Rules
            Email.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "Email is required" });
            Email.Validations.Add(new IsValidEmailRule<string> { ValidationMessage = "Invalid Email" });

            // Password Validation Rules
            Password.Item1.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "Password is required" });
            Password.Item1.Validations.Add(new IsValidPasswordRule<string> { ValidationMessage = "Password minimum 6 characters; must contain at least one uppercase letter" });
            Password.Item2.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "Confirm password is required" });
            Password.Validations.Add(new MatchPairValidationRule<string> { ValidationMessage = "Password and confirm password don't match" });
        }

        private bool AreFieldsValid()
        {
            bool isUserNameValid = UserName.Validate();
            bool isEmailValid = Email.Validate();
            bool isPasswordValid = Password.Validate();

            return isUserNameValid && isEmailValid && isPasswordValid;
        }

        private async void SignUpAsync(TaskCompletionSource<bool> obj)
        {
            await SignUpCommandHandler();
            obj.SetResult(true);
        }

        private async ValueTask NavigateToLoginPageCommandHandler()
        {
            if (_previousPageModel is LoginPageModel)
                await CoreMethods.PopPageModel();
            else
                await CoreMethods.PushPageModel<LoginPageModel>();
        }

        private async ValueTask SignUpCommandHandler()
        {
            try
            {
                MainState = LayoutState.Loading;
                if (AreFieldsValid())
                {
                    var auth = DependencyService.Get<IFirebaseAuthentication>();
                    var userCreated =
                        await auth.RegisterWithEmailAndPassword(UserName.Value, Email.Value, Password.Item1.Value);

                    if (userCreated)
                    {
                        ClearAuthData();
                        await CoreMethods.PushPageModel<LoginPageModel>();
                    }
                    else
                    {
                        await CoreMethods.DisplayAlert("Error", ConstantValues.GeneralError, "Ok");
                    }
                }
            }
            catch (Exception ex)
            {
                AnalyticsService.TrackError(ex, new Dictionary<string, string>
                {
                    { "Method", "SignupPageModel.SignupCommandHandler()" }
                });

                await CoreMethods.DisplayAlert("Error", ConstantValues.GeneralError, "Ok");
            }
            finally
            {
                MainState = LayoutState.None;
            }
        }

        private void ClearAuthData()
        {
            UserName.Value = Email.Value = Password.Item1.Value = Password.Item2.Value = string.Empty;
        }
    }
}