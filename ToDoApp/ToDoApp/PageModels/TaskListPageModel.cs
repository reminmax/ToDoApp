using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Plugin.CloudFirestore;
using Plugin.CloudFirestore.Reactive;
using PropertyChanged;
using Reactive.Bindings;
using ToDoApp.Authentication;
using ToDoApp.Helpers;
using ToDoApp.Helpers.Extensions;
using ToDoApp.Models;
using ToDoApp.Models.Calendar;
using ToDoApp.Repositories.FirestoreRepository;
using ToDoApp.Services.AnalyticsService;
using ToDoApp.Services.DateService;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;

namespace ToDoApp.PageModels
{
    [AddINotifyPropertyChangedInterface]
    public class TaskListPageModel : FreshBasePageModelExtended
    {
        //protected new IAnalyticsService AnalyticsService { get; private set; }
        private new IAnalyticsService AnalyticsService { get; }
        private readonly IDateService _dateService;

        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private readonly IFirebaseAuthentication _firebaseAuth = DependencyService.Get<IFirebaseAuthentication>();
        private readonly IFirestoreRepository<TaskModel> _taskRepository;
        private readonly string _userId;
        private bool _disposedValue;
        private DateItemModel _selectedCalendarItem;
        private TaskCategoryModel _selectedCategoryItem;


        public TaskListPageModel(IFirestoreRepository<TaskModel> tasksRepository, IDateService dateService, 
            IAnalyticsService analyticsService) : base(analyticsService)
        {
            Title = "Task list";

            _taskRepository = tasksRepository;
            _dateService = dateService;
            AnalyticsService = analyticsService;

            TaskList = new ReactiveCollection<TaskModel>();
            DisplayedTaskList = new ReactiveCollection<TaskModel>();
            CategoriesList = new ReactiveCollection<TaskCategoryModel>();
            TaskList.CollectionChanged += TaskList_CollectionChanged;

            // Commands
            AddNewTaskCommand = CommandFactory.Create(AddNewTaskCommandHandler);
            EditTaskCommand = CommandFactory.Create<TaskModel>(EditTaskCommandHandler);
            CheckTaskCommand = CommandFactory.Create<TaskModel>(CheckTaskCommandHandler);
            DeleteTaskCommand = CommandFactory.Create<TaskModel>(DeleteTaskCommandHandler);
            NavigateToProfilePageCommand = CommandFactory.Create(NavigateToProfilePageCommandHandler);

            UserName = _firebaseAuth.GetUsername();
            _userId = _firebaseAuth.GetUserId();
        }

        public LayoutState TaskListState { get; private set; }

        // Full task list for the selected day
        public ReactiveCollection<TaskModel> TaskList { get; }

        // task list based on the selected category
        public ReactiveCollection<TaskModel> DisplayedTaskList { get; }

        public ReactiveCollection<TaskCategoryModel> CategoriesList { get; private set; }
        public ObservableCollection<DateItemModel> CalendarItemsList { get; private set; }

        public string UserName { get; }

        public DateItemModel SelectedCalendarItem
        {
            get => _selectedCalendarItem;
            set
            {
                _selectedCalendarItem = value;
                RaisePropertyChanged();

                ExecuteSelectCalendarItem();
            }
        }

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

        public ICommand AddNewTaskCommand { get; }
        public ICommand CheckTaskCommand { get; }
        public ICommand DeleteTaskCommand { get; }
        public ICommand EditTaskCommand { get; }
        public ICommand NavigateToProfilePageCommand { get; }

        /// <summary>
        /// This methods is called when the View is appearing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void ViewIsAppearing(object sender, EventArgs e)
        {
            base.ViewIsAppearing(sender, e);

            // Calendar
            CalendarItemsList = new ObservableCollection<DateItemModel>(_dateService.GetDayList());
            SelectedCalendarItem = CalendarItemsList.FirstOrDefault(p => p.IsSelected);

            // Task list
            UpdateTaskList(DateTime.Today);
        }

        private async ValueTask AddNewTaskCommandHandler() =>
            await CoreMethods.PushPageModel<TaskPageModel>();

        private async ValueTask NavigateToProfilePageCommandHandler() =>
            await CoreMethods.PushPageModel<UserProfilePageModel>();

        private async ValueTask EditTaskCommandHandler(TaskModel task) =>
            await CoreMethods.PushPageModel<TaskPageModel>(task);

        private async ValueTask CheckTaskCommandHandler(TaskModel task)
        {
            task.Done = !task.Done;
            await _taskRepository.Update(task);
        }

        private async ValueTask DeleteTaskCommandHandler(TaskModel task) =>
            await _taskRepository.Delete(task);

        private void ExecuteSelectCategoryItem()
        {
            // Reset previously selected item
            ResetCategorySelectedItem();

            // Set selected item
            SetCategorySelectedItem();

            UpdateDisplayedTaskList();
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

        private void ExecuteSelectCalendarItem()
        {
            // Reset previously selected item
            ResetCalendarSelectedItem();

            // Set selected item
            SetCalendarSelectedItem();

            //UpdateTaskList(SelectedCalendarItem.Date, SelectedCategoryItem?.Id);
            UpdateTaskList(SelectedCalendarItem.Date);
        }

        private void SetCalendarSelectedItem()
        {
            // Mark selected item
            DateItemModel selectedItem = CalendarItemsList.ToList().Find(p => p == SelectedCalendarItem);
            if (selectedItem is not null)
            {
                selectedItem.IsSelected = true;
            }
        }

        private void ResetCalendarSelectedItem()
        {
            // Unmark a previously selected item
            DateItemModel previouslySelectedItem = CalendarItemsList.ToList().Find(p => p.IsSelected);
            if (previouslySelectedItem is not null)
            {
                previouslySelectedItem.IsSelected = false;
            }
        }

        private void TaskList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (!TaskList.Any())
            {
                CategoriesList.Clear();
                DisplayedTaskList.Clear();
                return;
            }

            UpdateCategoriesList();

            // Update the displayed list based on the selected category
            UpdateDisplayedTaskList();
        }

        private void UpdateDisplayedTaskList()
        {
            DisplayedTaskList.Clear();

            if (string.IsNullOrEmpty(SelectedCategoryItem?.Id))
            {
                // Category "All" is selected
                foreach (var item in TaskList)
                {
                    DisplayedTaskList.Add(item);
                }
            }
            else
            {
                if (TaskList != null)
                {
                    var taskItems = TaskList.Where(i => i.CategoryId == SelectedCategoryItem?.Id).ToList();
                    foreach (var item in taskItems)
                    {
                        DisplayedTaskList.Add(item);
                    }
                }
            }
        }

        private void UpdateCategoriesList()
        {
            CategoriesList.Clear();

            // Select distinct category items
            var distinctCategoryItemsArray = TaskList
                .Select(i => new {i.CategoryId, i.CategoryName, i.CategoryColor})
                .Distinct()
                .OrderByDescending(i => i.CategoryName)
                .ToArray();

            foreach (var item in distinctCategoryItemsArray)
            {
                CategoriesList.Add(
                    new TaskCategoryModel()
                    {
                        Id = item.CategoryId,
                        Name = item.CategoryName,
                        Color = item.CategoryColor,
                        UserId = _userId
                    });
            }

            switch (CategoriesList.Count())
            {
                case >= 2:
                    // Add "All" category item
                    CategoriesList.Insert(0,
                        new TaskCategoryModel()
                        {
                            Name = ConstantValues.AllTaskListName,
                            UserId = _userId,
                        });

                    SelectedCategoryItem = CategoriesList[0];
                    break;
                default:
                    if (CategoriesList.Count == 1)
                    {
                        SelectedCategoryItem = CategoriesList[0];
                    }

                    break;
            }
        }

        private void UpdateTaskList(DateTime date)
        {
            try
            {
                TaskListState = LayoutState.Loading;

                _disposables.Clear();
                TaskList.Clear();

                bool hideDoneTasks = AppSettings.IsHidingDoneTasksEnabled;

                // All tasks for current user by selected date
                var query = _taskRepository.GetAllContains(_userId, "date", date.ToString("dd/MM/yyyy"));

                _disposables.Add(query.ObserveAdded()
                    .Select(change => (Object: change.Document.ToObject<TaskModel>(ServerTimestampBehavior.Estimate),
                        Index: change.NewIndex))
                    .Select(t => (ViewModel: new TaskModel(t.Object), t.Index))
                    .Where(t => t.ViewModel != null)
                    .Subscribe(t => TaskList.InsertOnScheduler(t.Index, t.ViewModel)));

                _disposables.Add(query.ObserveModified()
                    .Select(change => change.Document.ToObject<TaskModel>(ServerTimestampBehavior.Estimate))
                    .Select(taskItem =>
                        (TaskItem: taskItem, ViewModel: TaskList.FirstOrDefault(x => x.Id == taskItem.Id)))
                    .Where(t => t.ViewModel != null)
                    .Subscribe(t => t.ViewModel.Update(t.TaskItem)));

                _disposables.Add(query.ObserveRemoved()
                    .Select(change => TaskList.FirstOrDefault(x => x.Id == change.Document.Id))
                    .Where(viewModel => viewModel != null)
                    .Subscribe(viewModel => TaskList.RemoveOnScheduler(viewModel)));

                _disposables.Add(query.AsObservable()
                    .Subscribe(list => TaskListState = list.Count == 0 ? LayoutState.Empty : LayoutState.None));

                TaskListState = LayoutState.None;
            }
            catch (Exception ex)
            {
                AnalyticsService.TrackError(ex, new Dictionary<string, string>
                {
                    { "Method", "TaskListPageModel.UpdateTaskList()" }
                });

                CoreMethods.PushPageModel<ErrorPageModel>();
            }
            finally
            {
                TaskListState = LayoutState.None;
            }
        }

        // Dispose pattern Implementation
        protected override void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _disposables.Dispose();
                }

                _disposedValue = true;
            }

            // Call base class implementation.
            base.Dispose(disposing);
        }
    }
}