using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using FreshMvvm;
using Plugin.CloudFirestore;
using Plugin.CloudFirestore.Reactive;
using PropertyChanged;
using Reactive.Bindings;
using ToDoApp.Authentication;
using ToDoApp.Helpers;
using ToDoApp.Helpers.Extensions;
using ToDoApp.Helpers.Validations;
using ToDoApp.Helpers.Validations.Rules;
using ToDoApp.Models;
using ToDoApp.Repositories.FirestoreRepository;
using ToDoApp.Services.AnalyticsService;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ToDoApp.PageModels
{
    [AddINotifyPropertyChangedInterface]
    public class TaskPageModel : FreshBasePageModelExtended
    {
        //protected new IAnalyticsService AnalyticsService { get; private set; }
        private new IAnalyticsService AnalyticsService { get; }
        private readonly IFirebaseAuthentication _firebaseAuth = DependencyService.Get<IFirebaseAuthentication>();
        private readonly IFirestoreRepository<TaskCategoryModel> _taskCategoryRepository;
        private readonly IFirestoreRepository<TaskModel> _taskRepository;

        public TaskPageModel(IFirestoreRepository<TaskModel> taskRepository, IFirestoreRepository<TaskCategoryModel> categoriesRepository, 
            IAnalyticsService analyticsService) : base(analyticsService)
        {
            _taskRepository = taskRepository;
            _taskCategoryRepository = categoriesRepository;
            AnalyticsService = analyticsService;

            CategoriesList = new ReactiveCollection<TaskCategoryModel>();

            // Commands
            AddNewCategoryCommand = CommandFactory.Create(AddNewCategoryCommandHandler);
            CreateTaskCommand = new FreshAwaitCommand(CreateTaskCommandHandler);

            Title = "Task";

            AddValidationRules();
        }

        private TaskCategoryModel _selectedCategoryItem { get; set; }
        private IQuery _query { get; set; }

        private bool _isExistingTaskBeingEdited { get; set; }

        // Existing task Id
        private string _taskId { get; set; }

        // Existing task category Id
        private string _taskCategoryId { get; set; }

        public ReactiveCollection<TaskCategoryModel> CategoriesList { get; }
        public ValidatableObject<string> Name { get; set; }
        public ValidatableObject<string> Notes { get; set; }
        public ValidatableObject<DateTime> Date { get; set; }
        public ValidatableObject<TimeSpan> Time { get; set; }
        public DateTime MinDate { get; } = DateTime.Now;

        public TaskCategoryModel SelectedCategoryItem
        {
            get => _selectedCategoryItem;
            set
            {
                _selectedCategoryItem = value;
                RaisePropertyChanged();

                ExecuteSelectCategoryItem();
            }
        }

        public FreshAwaitCommand CreateTaskCommand { get; }
        public ICommand AddNewCategoryCommand { get; }

        /// <summary>
        /// This method is called when the PageModel is loaded, the initData is the data that's sent from pagemodel before
        /// </summary>
        /// <param name="initData">Data that's sent to this PageModel from the pusher</param>        
        public override void Init(object initData)
        {
            if (initData is null)
            {
                return;
            }

            if (initData is TaskModel taskModel)
            {
                _isExistingTaskBeingEdited = true;
                _taskId = taskModel.Id;

                Name.Value = taskModel.Name;
                Notes.Value = taskModel.Notes;
                //Date.Value = DateTime.ParseExact(taskModel.Date, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                Date.Value = DateTime.Parse(taskModel.Date, CultureInfo.CurrentCulture.DateTimeFormat);

                var dateTime = taskModel.Time.ToDateTime();
                Time.Value = new TimeSpan(dateTime.Hour, dateTime.Minute, dateTime.Second);

                // save task category Id to make it active
                _taskCategoryId = taskModel.CategoryId;
            }

            base.Init(initData);
        }

        /// <summary>
        /// This methods is called when the View is appearing
        /// </summary>
        protected override async void ViewIsAppearing(object sender, EventArgs e)
        {
            base.ViewIsAppearing(sender, e);

            // Categories list
            await UpdateCategoriesList();
        }

        private async void CreateTaskCommandHandler(TaskCompletionSource<bool> obj)
        {
            await CreateTaskCommandHandler();
            obj.SetResult(true);
        }

        private async ValueTask AddNewCategoryCommandHandler() =>
            await CoreMethods.PushPageModel<TaskCategoryPageModel>();

        private async ValueTask CreateTaskCommandHandler()
        {
            if (!AreFieldsValid()) return;

            try
            {
                MainState = LayoutState.Loading;

                IFirebaseAuthentication auth = DependencyService.Get<IFirebaseAuthentication>();
                string userId = auth.GetUserId();

                if (_isExistingTaskBeingEdited)
                {
                    var model = new TaskModel()
                    {
                        Done = false,
                        Name = Name.Value,
                        Notes = Notes.Value,
                        UserId = userId,
                        Date = Date.Value.ToString("dd/MM/yyyy"),
                        Time = new Timestamp(new DateTime(Time.Value.Ticks)),
                        Id = _taskId,

                        // Category
                        CategoryId = SelectedCategoryItem?.Id,
                        CategoryName = SelectedCategoryItem?.Name,
                        CategoryColor = SelectedCategoryItem?.Color
                    };

                    bool wasUpdated = await _taskRepository.Update(model);
                    if (wasUpdated)
                    {
                        AnalyticsService.TrackEvent("Task has been changed",
                            new Dictionary<string, string>
                            {
                                { "Id", _taskId },
                                { "Name", Name.Value },
                                { "Date", Date.Value.ToString("dd/MM/yyyy") },
                                { "CategoryName", SelectedCategoryItem?.Name }
                            });

                        await CoreMethods.PopPageModel();
                    }
                    else
                    {
                        await CoreMethods.DisplayAlert("Error", ConstantValues.GeneralError, "OK");
                    }
                }
                else
                {
                    var model = new TaskModel()
                    {
                        Done = false,
                        Name = Name.Value,
                        Notes = Notes.Value,
                        UserId = userId,
                        Date = Date.Value.ToString("dd/MM/yyyy"),
                        Time = new Timestamp(new DateTime(Time.Value.Ticks)),

                        // Category
                        CategoryId = SelectedCategoryItem?.Id,
                        CategoryName = SelectedCategoryItem?.Name,
                        CategoryColor = SelectedCategoryItem?.Color
                    };

                    bool wasAdded = await _taskRepository.Add(model);
                    if (wasAdded)
                    {
                        AnalyticsService.TrackEvent("New task created", 
                            new Dictionary<string, string>
                            {
                                { "Name", Name.Value },
                                { "Date", Date.Value.ToString("dd/MM/yyyy") },
                                { "CategoryName", SelectedCategoryItem?.Name }
                            });

                        await CoreMethods.PopPageModel();
                    }
                    else
                    {
                        await CoreMethods.DisplayAlert("Error", ConstantValues.GeneralError, "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                AnalyticsService.TrackError(ex, new Dictionary<string, string>
                {
                    { "Method", "TaskPageModel.CreateTaskCommandHandler()" }
                });

                await CoreMethods.DisplayAlert("Error", ConstantValues.GeneralError, "OK");
            }
            finally
            {
                MainState = LayoutState.None;
            }
        }

        private async ValueTask UpdateCategoriesList()
        {
            try
            {
                string userId = _firebaseAuth.GetUserId();

                CategoriesList.Clear();

                _query = _taskCategoryRepository.GetAll(userId);

                _query.ObserveAdded()
                    .Select(change => (
                        Object: change.Document.ToObject<TaskCategoryModel>(ServerTimestampBehavior.Estimate),
                        Index: change.NewIndex))
                    .Select(t => (ViewModel: new TaskCategoryModel(t.Object), Index: t.Index))
                    .Where(t => t.ViewModel != null)
                    //.Subscribe(t => CategoriesList.InsertOnScheduler(t.Index, t.ViewModel));
                    .Subscribe(t =>
                    {
                        CategoriesList.Insert(t.Index, t.ViewModel);

                        if (!_isExistingTaskBeingEdited)
                        {
                            SelectedCategoryItem = CategoriesList.FirstOrDefault();
                        }
                        else
                        {
                            // make category active
                            SelectedCategoryItem = CategoriesList.FirstOrDefault(p => p.Id == _taskCategoryId);
                        }
                    });

                _query.ObserveModified()
                    .Select(change => change.Document.ToObject<TaskCategoryModel>(ServerTimestampBehavior.Estimate))
                    .Select(categoryItem => (CategoryItem: categoryItem,
                        ViewModel: CategoriesList.FirstOrDefault(x => x.Id == categoryItem.Id)))
                    .Where(t => t.ViewModel != null)
                    .Subscribe(t => t.ViewModel.Update(t.CategoryItem));

                _query.ObserveRemoved()
                    .Select(change => CategoriesList.FirstOrDefault(x => x.Id == change.Document.Id))
                    .Where(viewModel => viewModel != null)
                    .Subscribe(viewModel => CategoriesList.RemoveOnScheduler(viewModel));
            }
            catch (Exception)
            {
                await CoreMethods.PushPageModel<ErrorPageModel>();
            }
        }

        private void ExecuteSelectCategoryItem()
        {
            // Reset previously selected item
            ResetCategorySelectedItem();

            // Set selected item
            SetCategorySelectedItem();
        }

        private void SetCategorySelectedItem()
        {
            // Mark selected item
            var selectedItem = CategoriesList.FirstOrDefault(p => p == SelectedCategoryItem);
            if (selectedItem is not null)
            {
                selectedItem.IsSelected = true;
            }
        }

        private void ResetCategorySelectedItem()
        {
            // Unmark a previously selected item
            var previouslySelectedItem = CategoriesList.FirstOrDefault(p => p.IsSelected);
            if (previouslySelectedItem is not null)
            {
                previouslySelectedItem.IsSelected = false;
            }
        }

        private void AddValidationRules()
        {
            Name = new ValidatableObject<string>();
            Notes = new ValidatableObject<string>();
            Date = new ValidatableObject<DateTime>();
            Time = new ValidatableObject<TimeSpan>();

            Name.Validations.Add(new IsNotNullOrEmptyRule<string> {ValidationMessage = "A name is required."});
            Notes.Validations.Add(new IsNotNullOrEmptyRule<string> {ValidationMessage = "Notes is required."});
            Date.Validations.Add(new IsNotNullOrEmptyRule<DateTime> {ValidationMessage = "A date is required."});
            Time.Validations.Add(new IsNotNullOrEmptyRule<TimeSpan> {ValidationMessage = "A time is required."});
        }

        private bool AreFieldsValid()
        {
            bool isNameValid = Name.Validate();
            bool isNotesValid = Notes.Validate();
            bool isDateValid = Date.Validate();
            bool isTimeValid = Time.Validate();

            return isNameValid && isNotesValid && isDateValid && isTimeValid;
        }
    }
}