using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ToDoApp.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TaskCategoriesView : CollectionView
    {
        public TaskCategoriesView()
        {
            InitializeComponent();
        }
    }
}