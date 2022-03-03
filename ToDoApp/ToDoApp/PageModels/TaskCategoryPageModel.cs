using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using FreshMvvm;
using PropertyChanged;
using ToDoApp.Authentication;
using ToDoApp.Helpers;
using ToDoApp.Helpers.Extensions;
using ToDoApp.Helpers.Validations;
using ToDoApp.Helpers.Validations.Rules;
using ToDoApp.Models;
using ToDoApp.Repositories.FirestoreRepository;
using ToDoApp.Services.AnalyticsService;
using ToDoApp.Services.SampleDataService;
using Xamarin.Forms;

namespace ToDoApp.PageModels
{
    [AddINotifyPropertyChangedInterface]
    public class TaskCategoryPageModel : FreshBasePageModelExtended
    {
        //protected new IAnalyticsService AnalyticsService { get; private set; }
        protected new IAnalyticsService AnalyticsService { get; }
        private readonly IFirestoreRepository<TaskCategoryModel> _categoriesRepository;
        private readonly ISampleDataService _sampleDataService;

        public TaskCategoryPageModel(IFirestoreRepository<TaskCategoryModel> categoriesRepository, ISampleDataService sampleDataService,
            IAnalyticsService analyticsService) : base(analyticsService)
        {
            _categoriesRepository = categoriesRepository;
            _sampleDataService = sampleDataService;
            AnalyticsService = analyticsService;

            // Commands
            CreateCategoryCommand = new FreshAwaitCommand(CreateCategoryAsync);

            Title = "Task category";

            // Validations
            AddValidationRules();
        }

        public ValidatableObject<string> Name { get; set; }

        public ObservableCollection<string> ColorList { get; set; }

        public string SelectedColor { get; set; }

        public FreshAwaitCommand CreateCategoryCommand { get; }

        private async void CreateCategoryAsync(TaskCompletionSource<bool> obj)
        {
            await CreateCategoryCommandHandler();
            obj.SetResult(true);
        }

        /// <summary>
        /// This method is called when the PageModel is loaded, the initData is the data that's sent from pagemodel before
        /// </summary>
        /// <param name="initData">Data that's sent to this PageModel from the pusher</param>        
        public override void Init(object initData)
        {
            base.Init(initData);

            ColorList = new ObservableCollection<string>(_sampleDataService.GetColorList());
            if (ColorList.Any())
                SelectedColor = ColorList.FirstOrDefault();
        }

        private void AddValidationRules()
        {
            Name = new ValidatableObject<string>();

            Name.Validations.Add(new IsNotNullOrEmptyRule<string> {ValidationMessage = "A name is required."});
        }

        private bool AreFieldsValid() => Name.Validate();

        private async Task CreateCategoryCommandHandler()
        {
            if (!AreFieldsValid())
                return;

            try
            {
                IFirebaseAuthentication auth = DependencyService.Get<IFirebaseAuthentication>();
                string userId = auth.GetUserId();
                TaskCategoryModel taskCategory = new TaskCategoryModel()
                {
                    Name = Name.Value,
                    Color = SelectedColor,
                    UserId = userId
                };

                bool wasAdded = await _categoriesRepository.Add(taskCategory);
                if (wasAdded)
                    await CoreMethods.PopPageModel();
                else
                    await CoreMethods.DisplayAlert("Error", ConstantValues.GeneralError, "OK");
            }
            catch (Exception ex)
            {
                AnalyticsService.TrackError(ex, new Dictionary<string, string>
                {
                    { "Method", "TaskListPageModel.UpdateTaskList()" }
                });

                await CoreMethods.DisplayAlert("Error", ConstantValues.GeneralError, "OK");
            }
        }
    }
}