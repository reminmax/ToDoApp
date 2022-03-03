using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using ToDoApp.Models.Calendar;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ToDoApp.Controls
{
    /// <summary>
    /// This CollectionView has the ability to scroll to the current date
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CalendarCollectionView : ContentView
    {
        public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty
                .Create(
                    propertyName: "ItemsSource",
                    returnType: typeof(ObservableCollection<DateItemModel>),
                    declaringType: typeof(CalendarCollectionView),
                    defaultValue: null,
                    defaultBindingMode: BindingMode.OneWay,
                    propertyChanged: ItemsSourcePropertyChanged);

        public CalendarCollectionView()
        {
            InitializeComponent();
        }

        public ObservableCollection<DateItemModel> ItemsSource { get; set; }

        private static async void ItemsSourcePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var items = newValue as ObservableCollection<DateItemModel>;
            var control = (CalendarCollectionView) bindable;

            await Task.Delay(250);

            //var index = items.ToList().FindIndex(p => p.IsSelected);
            //if (index > -1)
            //    control.daysList.ScrollTo(index, -1, ScrollToPosition.MakeVisible, true);

            if (items != null)
            {
                var item = items.ToList().Find(p => p.IsSelected);
                if (item is not null)
                {
                    control.daysList.ScrollTo(item, -1, ScrollToPosition.MakeVisible, true);
                }
            }
        }
    }
}